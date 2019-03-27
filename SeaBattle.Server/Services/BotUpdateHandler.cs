namespace SeaBattle.Server.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Models.Commands;
    using Telegram.Bot.Types;
    using Telegram.Bot.Types.Enums;

    public class BotUpdateHandler : IBotUpdateHandler
    {
        private readonly Dictionary<string, ICommand> _commands;

        private readonly IEnumerable<IServiceWithState> _statefullServices;

        public BotUpdateHandler(IEnumerable<ICommand> commands, 
                             IEnumerable<IServiceWithState> statefullServices)
        {
            _statefullServices = statefullServices;

            _commands = commands.ToDictionary(cmd => $"/{cmd.Name}", cmd => cmd);
        }

        public async Task Handle(Update update)
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

            foreach (var service in _statefullServices)
            {
                if (service.IsActive(update.Message.From.Id))
                {
                    await service.MoveMext(update);
                    return;
                }
            }
            
            if (Utils.IsCommand(update))
            {
                var cmdName = Utils.GetCommandName(update);
                
                if (_commands.TryGetValue(cmdName, out var matchCommand))
                {
                    await matchCommand.Execute(update);
                }
            }
        }
    }
}