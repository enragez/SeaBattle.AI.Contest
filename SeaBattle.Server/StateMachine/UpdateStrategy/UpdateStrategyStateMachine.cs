namespace SeaBattle.Server.StateMachine.UpdateStrategy
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Dal;
    using Dal.Entities;
    using Exceptions;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using Services.Compile;
    using Telegram.Bot.Types;

    public class UpdateStrategyStateMachine : IStateMachine<UpdateStrategyState>
    {
        private readonly IBotService _botService;
        private readonly IStrategyCompiler _compiler;

        private readonly ApplicationContext _dbContext;
        
        public UpdateStrategyState State { get; private set; } = UpdateStrategyState.Started;

        private byte[] _newStrategyAssembly;

        private byte[] _newStrategySources;

        public UpdateStrategyStateMachine(IBotService botService, IStrategyCompiler compiler, ApplicationContext dbContext)
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
                    case UpdateStrategyState.Started:
                        await HandleStartedState(update);
                        break;
                    case UpdateStrategyState.WaitingForStrategy:
                        await HandleWaitingForStrategyState(update);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Возникла неизвестная ошибка:
{ex.Message}
                
Обновление стратегии отменено.");
                State = UpdateStrategyState.Canceled;
            }
        }

        private async Task HandleStartedState(Update update)
        {
            var participant = await _dbContext.Participants.FirstOrDefaultAsync(p => p.TelegramId == update.Message.From.Id);
            
            if (participant == null)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Вы не зарегистрированы.

Для участия необходимо использовать команду /register");
                State = UpdateStrategyState.Canceled;
                return;
            }
            
            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          @"Пришлите вашу стратегию. 
Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
            State = UpdateStrategyState.WaitingForStrategy;
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
                    _newStrategyAssembly = await _compiler.Compile(memoryStream);
                    _newStrategySources = memoryStream.ToArray();
                }
                catch (StrategyCompilationException ex)
                {
                    await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                                  $@"Ошибка компиляции стратегии: 

{ex.Message}");
                    return;
                }
                catch (Exception ex)
                {
                    await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                                  $@"Неизвестная ошибка при компиляции стратегии: 
{ex.Message}");
                }
            }
            
            State = UpdateStrategyState.ReadyToFinish;
        }

        public async Task Finish(Update update)
        {
            if (State != UpdateStrategyState.ReadyToFinish)
            {
                return;
            }

            var participant = await _dbContext.Participants.FirstAsync(p => p.TelegramId == update.Message.From.Id);

            participant.Strategy = _newStrategyAssembly;
            
            _dbContext.StrategySources.Add(new StrategySource
                                           {
                                               Participant = participant,
                                               LoadDate = DateTime.Now,
                                               Sources = _newStrategySources
                                           });

            await _dbContext.SaveChangesAsync();
            
            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          "Стратегия успешно обновлена");

            State = UpdateStrategyState.Updated;
        }
    }
}