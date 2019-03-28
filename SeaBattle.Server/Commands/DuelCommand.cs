namespace SeaBattle.Server.Commands
{
    using System.Threading.Tasks;
    using Dal;
    using Microsoft.EntityFrameworkCore;
    using Services;
    using Telegram.Bot.Types;
    using Utils;

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
            var player1 = await _dbContext.Participants.FirstOrDefaultAsync(p => p.TelegramId == update.Message.From.Id);
            
            if (player1 == null)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Вы не зарегистрированы.

Для участия необходимо использовать команду /register");
                return;
            }
            
            var playerId = Utils.GetCommandArgument(update);
            if (string.IsNullOrWhiteSpace(playerId))
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Необходимо указать идентификатор игрока.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }

            if (!int.TryParse(playerId, out var id))
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Идентификатор должен быть числом.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }

            var player2 = await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == id);

            if (player2 == null)
            {
                await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                              $@"Игрок с идентификатором {id} не найден.

Для получения идентификаторов необходимо использовать команду /players");
                return;
            }

            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          $"Дуэль между игроками {player1.Name} и {player2.Name} запущена");

            if (player1.Id != player2.Id)
            {
                await _botService.SendTextMessageAsync((long) player2.TelegramId,
                                                              $"Игрок {player1.Name} вызвал вас на дуэль");
            }

            var (playedGame, gameResult) = await _runner.StartGameAsync(player1, player2, false);

            var winnerName = gameResult.Winner.Id == player1.Id
                                 ? player1.Name
                                 : player2.Name;

            await _botService.SendTextMessageAsync(update.Message.Chat.Id,
                                                          $@"Игра завершена.
Победитель: {winnerName}

Подробности: {Utils.GetGameUrl(playedGame)}");

            if (player1.Id != player2.Id)
            {
                await _botService.SendTextMessageAsync((long) player2.TelegramId,
                                                              $@"Игра завершена.
Победитель: {winnerName}

Подробности: {Utils.GetGameUrl(playedGame)}");
            }
        }
    }
}