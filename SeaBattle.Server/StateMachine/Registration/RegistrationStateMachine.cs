namespace SeaBattle.Server.StateMachine.Registration
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Models;
    using Services;
    using Telegram.Bot.Types;

    public class RegistrationStateMachine : IStateMachine<RegistrationState>
    {
        private readonly IBotService _botService;
        private readonly ApplicationContext _dbContext;

        public RegistrationState State { get; private set; } = RegistrationState.Started;

        private RegistrationModel _registration;

        public RegistrationStateMachine(IBotService botService, ApplicationContext dbContext)
        {
            _botService = botService;
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
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
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
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                          "Пожалуйста, укажите ваше имя");
            State = RegistrationState.WaitingForName;
        }

        private async Task HandleWaitingForNameState(Update update)
        {
            if (!Utils.IsUsernameValid(update.Message.Text, out var validationMessage))
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, validationMessage);
                return;
            }
            
            _registration.Name = update.Message.Text.Trim();
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                          @"Пришлите вашу стратегию. 
Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
            
            State = RegistrationState.WaitingForStrategy;
        }

        private async Task HandleWaitingForStrategyState(Update update)
        {
            var file = await _botService.Client.GetFileAsync(update.Message.Document.FileId);

            var extension = Path.GetExtension(file.FilePath);
            if (!extension.Equals(".zip"))
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              "Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
                return;
            }          
            
            var memoryStream = new MemoryStream();

            await _botService.Client.DownloadFileAsync(file.FilePath, memoryStream);
            
            // TODO: check zip contains cs-files, try to compile, if success - ReadyToFinish, zip dll, else - compilation error message
            
            _registration.StrategyStream = memoryStream;
            
            State = RegistrationState.ReadyToFinish;
        }

        public async Task Finish(Update update)
        {
            if (State != RegistrationState.ReadyToFinish)
            {
                return;
            }

            using (_registration.StrategyStream)
            {
                _dbContext.Participants.Add(new Entities.Participant
                                            {
                                                Name = _registration.Name,
                                                TelegramId = _registration.TelegramId,
                                                Strategy = _registration.StrategyStream.ToArray()
                                            });

                await _dbContext.SaveChangesAsync();
            }
            
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                          "Регистрация завершена");

            State = RegistrationState.Registered;
        }
    }
}