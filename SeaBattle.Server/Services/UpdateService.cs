namespace SeaBattle.Server.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Logging;
    using Models.Commands;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class UpdateService : IUpdateService
    {
        private readonly IBotService _botService;
        
        private readonly ILogger<UpdateService> _logger;

        private readonly Dictionary<string, ICommand> _commands;

        private readonly IRegisterService _registerService;

        public UpdateService(IBotService botService, ILogger<UpdateService> logger, IEnumerable<ICommand> commands, IRegisterService registerService)
        {
            _botService = botService;
            _logger = logger;
            _registerService = registerService;

            _commands = commands.ToDictionary(cmd => $"/{cmd.Name}", cmd => cmd);
        }

        public async Task EchoAsync(Update update)
        {
            if (update.Type != UpdateType.Message)
            {
                return;
            }
            
            var message = update.Message;

            if (message.Chat.Type != ChatType.Private)
            {
                return;
            }
            
            if (_registerService.UserRegistering(update.Message.From.Id))
            {
                await _registerService.MoveMext(update);
                return;
            }
            
            if (IsCommandMessage(update))
            {
                if (_commands.TryGetValue(update.Message.Text, out var matchCommand))
                {
                    await matchCommand.Execute(update);
                }
                else
                {
                    // todo unknown command
                }
            }
            else
            {
                // todo simple message
            }
        }

        private bool IsCommandMessage(Update update)
        {
            return update.Message?.Entities?.Any(entity => entity?.Type == MessageEntityType.BotCommand) ?? false;
        }
    }
}