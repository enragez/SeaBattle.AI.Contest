namespace SeaBattle.Server.Models.Commands
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using Telegram.Bot.Types;

    public class DuelCommand : ICommand
    {
        private readonly IBotService _botService;
        
        private readonly ApplicationContext _dbContext;
        
        private readonly IGameRunner _runner;

        public string Name => "duel";

        public DuelCommand(IBotService botService, ApplicationContext dbContext, IGameRunner runner)
        {
            _botService = botService;
            _dbContext = dbContext;
            _runner = runner;
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

            var player2 = await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == id);

            if (player2 == null)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Игрок с идентификатором {id} не найден.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }
            
            var player1 = await _dbContext.Participants.FirstOrDefaultAsync(p => p.TelegramId == update.Message.From.Id);

            var gameResult = await _runner.StartGameAsync(player1, player2, false);

            var winnerName = gameResult.Winner.Id == player1.Id
                                 ? player1.Name
                                 : player2.Name;

            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                          $@"Игра завершена.
Победитель: {winnerName}

Подробности: ТУТ_ДОЛЖЕН_БЫТЬ_URL");
        }
    }
}