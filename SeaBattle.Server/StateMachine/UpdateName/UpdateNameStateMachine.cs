namespace SeaBattle.Server.StateMachine.UpdateName
{
    using System;
    using System.IO;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Services;
    using Telegram.Bot.Types;

    public class UpdateNameStateMachine : IStateMachine<UpdateNameState>
    {
        private readonly IBotService _botService;
        
        private readonly ApplicationContext _dbContext;
        
        public UpdateNameState State { get; private set; } = UpdateNameState.Started;

        private string _newName;

        public UpdateNameStateMachine(IBotService botService, ApplicationContext dbContext)
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
                    case UpdateNameState.Started:
                        await HandleStartedState(update);
                        break;
                    case UpdateNameState.WaitingForName:
                        await HandleWaitingForNameState(update);
                        break;
                }
            }
            catch (Exception ex)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Возникла неизвестная ошибка:
{ex.Message}
                
Обновление имени отменено.");
                State = UpdateNameState.Canceled;
            }
        }

        private async Task HandleStartedState(Update update)
        {
            var participant = await _dbContext.Participants.FirstAsync(p => p.TelegramId == update.Message.From.Id);
            
            if (participant == null)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Вы не зарегистрированы.

Для участия необходимо использовать команду /register");
                State = UpdateNameState.Canceled;
                return;
            }
            
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, "Пожалуйста, укажите новое имя");
            State = UpdateNameState.WaitingForName;
        }

        private async Task HandleWaitingForNameState(Update update)
        {
            if (!Utils.IsUsernameValid(update.Message.Text, out var validationMessage))
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, validationMessage);
                return;
            }
            
            _newName = update.Message.Text.Trim();
            
            State = UpdateNameState.ReadyToFinish;
        }

        public async Task Finish(Update update)
        {
            if (State != UpdateNameState.ReadyToFinish)
            {
                return;
            }

            var participant = await _dbContext.Participants.FirstAsync(p => p.TelegramId == update.Message.From.Id);

            participant.Name = _newName;

            await _dbContext.SaveChangesAsync();
            
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, "Ваше имя успешно изменено");

            State = UpdateNameState.Updated;
        }
    }
}