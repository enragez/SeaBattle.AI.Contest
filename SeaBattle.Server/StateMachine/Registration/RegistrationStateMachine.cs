namespace SeaBattle.Server.StateMachine.Registration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dal;
    using Dal.Entities;
    using Exceptions;
    using Models;
    using Services;
    using Services.Compile;
    using Telegram.Bot.Types;
    using Utils;
    using Participant = Dal.Entities.Participant;

    public class RegistrationStateMachine : IStateMachine<RegistrationState>
    {
        private readonly IBotService _botService;
        private readonly IStrategyCompiler _compiler;
        private readonly ApplicationContext _dbContext;

        public RegistrationState State { get; private set; } = RegistrationState.Started;

        private RegistrationModel _registration;

        public RegistrationStateMachine(IBotService botService, IStrategyCompiler compiler, ApplicationContext dbContext)
        {
            _botService = botService;
            _compiler = compiler;
            _dbContext = dbContext;
        }

        public async Task MoveNext(Update update)
        {
            try
            {
                switch (State)
                {
                    case RegistrationState.Started:
                        await HandleStartedState(update);
                        break;
                    case RegistrationState.WaitingForName:
                        await HandleWaitingForNameState(update);
                        break;
                    case RegistrationState.WaitingForStrategy:
                        await HandleWaitingForStrategyState(update);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Возникла неизвестная ошибка:
{ex.Message}

Регистрация отменена.");
                State = RegistrationState.Canceled;
            }
        }

        private async Task HandleStartedState(Update update)
        {
            _registration = new RegistrationModel
                            {
                                TelegramId = update.Message.From.Id
                            };
            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          "Пожалуйста, укажите ваше имя");
            State = RegistrationState.WaitingForName;
        }

        private async Task HandleWaitingForNameState(Update update)
        {
            if (!Utils.IsUsernameValid(update.Message.Text, out var validationMessage))
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id, validationMessage);
                return;
            }
            
            _registration.Name = update.Message.Text.Trim();
            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          @"Пришлите вашу стратегию. 
Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
            
            State = RegistrationState.WaitingForStrategy;
        }

        private async Task HandleWaitingForStrategyState(Update update)
        {
            if (update.Message.Document == null)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Пришлите вашу стратегию. 
Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
                return;
            }
            
            var file = await _botService.Client.GetFileAsync(update.Message.Document.FileId);

            var extension = Path.GetExtension(file.FilePath);
            if (!extension.Equals(".zip"))
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              "Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
                return;
            }          
            
            var memoryStream = new MemoryStream();

            await _botService.Client.DownloadFileAsync(file.FilePath, memoryStream);

            using (memoryStream)
            {
                try
                {
                    _registration.StrategyAssembly = await _compiler.Compile(memoryStream);
                    _registration.StrategySources = memoryStream.ToArray();
                }
                catch (StrategyCompilationException ex)
                {
                    await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                                  $@"Ошибка компиляции стратегии: 

{ex.Message}");
                    return;
                }
            }
            
            State = RegistrationState.ReadyToFinish;
        }

        public async Task Finish(Update update)
        {
            if (State != RegistrationState.ReadyToFinish)
            {
                return;
            }

            var participant = new Participant
                              {
                                  Name = _registration.Name,
                                  TelegramId = _registration.TelegramId,
                                  Strategy = _registration.StrategyAssembly
                              };
            
            _dbContext.Participants.Add(participant);

            _dbContext.Statistic.Add(new Statistic
                                      {
                                          Wins = 0,
                                          Losses = 0,
                                          Rating = 1000,
                                          GamesPlayed = 0,
                                          Participant = participant
                                      });

            _dbContext.StrategySources.Add(new StrategySource
                                           {
                                               Participant = participant,
                                               LoadDate = DateTime.Now,
                                               Sources = _registration.StrategySources
                                           });

            await _dbContext.SaveChangesAsync();
            
            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          "Регистрация завершена");

            State = RegistrationState.Registered;
        }
    }
}