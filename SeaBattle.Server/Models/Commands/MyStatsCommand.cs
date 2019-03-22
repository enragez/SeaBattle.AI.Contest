namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using Services.Stats;
    using Telegram.Bot.Types;

    public class MyStatsCommand : ICommand
    {
        private readonly IStatisticsService _statsService;
        
        private readonly IBotService _botService;
        private readonly ApplicationContext _dbContext;

        public string Name => "mystats";

        public MyStatsCommand(IStatisticsService statsService, IBotService botService, ApplicationContext dbContext)
        {
            _statsService = statsService;
            _botService = botService;
            _dbContext = dbContext;
        }
        
        public async Task Execute(Update update)
        {
            var player =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.TelegramId == update.Message.From.Id)
                             ;
            
            var stats = _statsService.Get(player);

            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, stats);
        }
    }
}