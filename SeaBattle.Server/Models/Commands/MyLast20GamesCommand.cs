namespace SeaBattle.Server.Models.Commands
{
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Engine.Models.Serializable;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Services;
    using Telegram.Bot.Types;

    public class MyLast20GamesCommand : ICommand
    {
        private readonly IBotService _botService;
        
        private readonly ApplicationContext _dbContext;
        public string Name => "mylast20games";
        
        public MyLast20GamesCommand(IBotService botService, ApplicationContext dbContext)
        {
            _botService = botService;
            _dbContext = dbContext;
        }
        
        public async Task Execute(Update update)
        {
            var player =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.TelegramId == update.Message.From.Id);

            if (player == null)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id,
                                                              @"Вы не зарегистрированы.

Для участия необходимо использовать команду /register");
                return;
            }
            
            var games = await _dbContext.PlayedGames.Where(game => game.Rated)
                                     .ToListAsync();

            var gamesWithResults = games.Select(game => new
                                                   {
                                                       Game = game,
                                                       Result = JsonConvert.DeserializeObject<SerializableGameResult>(game.Result)
                                                   });

            var playerGames = gamesWithResults.Where(dto => dto.Result.Participant1.PlayerDto.Id == player.Id ||
                                                            dto.Result.Participant2.PlayerDto.Id == player.Id)
                                           .ToList();
            
            if (playerGames.Count == 0)
            {
                await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, "Игры не найдены");
                return;
            }

            var message = new StringBuilder("Последние 20 рейтинговых игр:");
            message.AppendLine();

            var last20Games = playerGames.OrderByDescending(game => game.Result.StartTime)
                                      .Take(20);
            
            foreach (var dto in last20Games)
            {
                var winMessage = dto.Result.WinnerId == player.Id
                                     ? "Победа"
                                     : "Поражение";
                
                message.AppendLine($"Игра #{dto.Game.Id}. {winMessage}. {Utils.GetGameUrl(dto.Game)}");
            }
            
            await _botService.Client.SendTextMessageAsync(update.Message.Chat.Id, message.ToString());
        }
    }
}