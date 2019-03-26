namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using Services.Stats;
    using Telegram.Bot.Types;

    public class PlayerStatsCommand : ICommand
    {
        private readonly IBotService _botService;
        
        private readonly IStatisticsService _statsService;

        private readonly ApplicationContext _dbContext;

        public string Name => "playerstats";

        public PlayerStatsCommand(IBotService botService, IStatisticsService statsService, ApplicationContext dbContext)
        {
            _botService = botService;
            _statsService = statsService;
            _dbContext = dbContext;
        }

        public async Task Execute(Update update)
        {
            var playerId = Utils.GetCommandArgument(update);
            if (string.IsNullOrWhiteSpace(playerId))
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Необходимо указать идентификатор игрока.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }

            if (!int.TryParse(playerId, out var id))
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Идентификатор должен быть числом.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }

            var player = await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Игрок с идентификатором {id} не найден.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }
            
            var stats = _statsService.Get(player);

            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, stats);
        }
    }
}