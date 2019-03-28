namespace SeaBattle.Server.Commands
{
    using System.Threading.Tasks;
    using Dal;
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
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.TelegramId == update.Message.From.Id);

            if (player == null)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Вы не зарегистрированы.

Для участия необходимо использовать команду /register");
                return;
            }
            
            var stats = await _statsService.GetAsync(player);

            await _botService.SendTextMessageAsync(update.Message.Chat.Id, stats);
        }
    }
}