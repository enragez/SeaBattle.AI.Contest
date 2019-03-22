namespace SeaBattle.Server.StateMachine.UpdateStrategy
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;
    using Telegram.Bot.Types;

    public class UpdateStrategyStateMachine : IStateMachine<UpdateStrategyState>
    {
        private readonly IBotService _botService;
        
        private readonly ApplicationContext _dbContext;
        
        public UpdateStrategyState State { get; private set; } = UpdateStrategyState.Started;

        private MemoryStream _newStrategy;

        public UpdateStrategyStateMachine(IBotService botService, ApplicationContext dbContext)
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
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Возникла неизвестная ошибка:
{ex.Message}
                
Обновление стратегии отменено.");
                State = UpdateStrategyState.Canceled;
            }
        }

        private async Task HandleStartedState(Update update)
        {
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                          @"Пришлите вашу стратегию. 
Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
            State = UpdateStrategyState.WaitingForStrategy;
        }

        private async Task HandleWaitingForStrategyState(Update update)
        {
            if (update.Message.Document == null)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Пришлите вашу стратегию. 
Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
                return;
            }
            
            var file = await _botService.Client.GetFileAsync(update.Message.Document.FileId);

            var extension = Path.GetExtension(file.FilePath);
            if (!extension.Equals(".zip"))
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              "Стратегии принимаются в формате .zip файла содержащего набор cs-файлов.");
                return;
            }

            _newStrategy = new MemoryStream();

            await _botService.Client.DownloadFileAsync(file.FilePath, _newStrategy);
            
            // TODO: check zip contains cs-files, try to compile, if success - ReadyToFinish, zip dll, else - compilation error message
            
            State = UpdateStrategyState.ReadyToFinish;
        }

        public async Task Finish(Update update)
        {
            if (State != UpdateStrategyState.ReadyToFinish)
            {
                return;
            }

            using (_newStrategy)
            {
                var participant = await _dbContext.Participants.FirstAsync(p => p.TelegramId == update.Message.From.Id);

                participant.Strategy = _newStrategy.ToArray();

                await _dbContext.SaveChangesAsync();
            }
            
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                          "Стратегия успешно обновлена");

            State = UpdateStrategyState.Updated;
        }
    }
}