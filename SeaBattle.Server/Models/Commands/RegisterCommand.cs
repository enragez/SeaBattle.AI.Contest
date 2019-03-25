namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using StateMachine.Registration;
    using Telegram.Bot.Types;

    public class RegisterCommand : ICommand
    {
        private readonly IServiceWithState<RegistrationState> _registerService;
        private readonly IBotService _botService;

        private readonly ApplicationContext _dbContext;
        
        public string Name => "register";

        public RegisterCommand(IServiceWithState<RegistrationState> registerService, IBotService botService, ApplicationContext dbContext)
        {
            _registerService = registerService;
            _botService = botService;
            _dbContext = dbContext;
        }
        
        public async Task Execute(Update update)
        {
            var registered = await _dbContext.Participants.AnyAsync(p => p.TelegramId == update.Message.From.Id);

            if (!registered)
            {
                await _registerService.MoveMext(update);
            }
            else
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Вы уже зарегистрированы.

Используйте команды:
/updatename - если хотите изменить имя
/updatestrategy - если хотите обновить стратегию");
            }
        }
    }
}