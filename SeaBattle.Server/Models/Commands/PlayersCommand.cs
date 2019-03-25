namespace SeaBattle.Server.Models.Commands
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using Telegram.Bot.Types;

    public class PlayersCommand : ICommand
    {
        private readonly IBotService _botService;
        
        private readonly ApplicationContext _dbContext;
        
        public string Name => "players";

        public PlayersCommand(IBotService botService, ApplicationContext dbContext)
        {
            _botService = botService;
            _dbContext = dbContext;
        }
        
        public async Task Execute(Update update)
        {
            var playersInfo = await _dbContext.Participants.Select(p => new
                                                                        {
                                                                            p.Id,
                                                                            p.Name
                                                                        })
                                           .ToListAsync();

            var message = new StringBuilder("Зарегистрированные участники:");
            message.AppendLine();
            message.AppendLine();

            foreach (var pInfo in playersInfo.OrderBy(p => p.Id))
            {
                message.AppendLine($"Имя: {pInfo.Name}, Id: {pInfo.Id}");
            }

            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, message.ToString());
        }
    }
}