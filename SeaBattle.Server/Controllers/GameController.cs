namespace SeaBattle.Server.Controllers
{
    using System.Threading.Tasks;
    using Dal;
    using Dal.Entities;
    using Engine.Models;
    using Engine.Models.Serializable;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Newtonsoft.Json;

    public class GameController : Controller
    {
        private readonly ApplicationContext _dbContext;

        public GameController(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        // GET
        public ActionResult Index()
        {
            return Redirect("Home");
        }

        [HttpGet]
        public async Task<ActionResult> Get(int id)
        {
            var game = await _dbContext.PlayedGames.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            var gameModel = await GetGameModel(game);

            ViewBag.Title = $"Игра #{game.Id}";

            return View("Game", gameModel);
        }

        public async Task<ActionResult> GetTurn(int id, int turn)
        {
            if (turn < 0)
            {
                return NotFound();
            }
            
            var game = await _dbContext.PlayedGames.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }

            var gameModel = await GetGameModel(game, turn);
            
            ViewBag.Title = $"Игра #{game.Id}";

            return View("Game", gameModel);
        }

        private async Task<Game> GetGameModel(PlayedGame game, int turn = 0)
        {
            var gameResult = JsonConvert.DeserializeObject<SerializableGameResult>(game.Result);

            var participant1 =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == gameResult.Participant1.PlayerDto.Id);

            var participant2 =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == gameResult.Participant2.PlayerDto.Id);

            return new Game
                   {
                       Id = game.Id,
                       Turn = turn,
                       StartTime = gameResult.StartTime,
                       EndTime = gameResult.EndTime,
                       Participant1 = new Models.Participant
                                      {
                                          Id = participant1.Id,
                                          Name = participant1.Name
                                      },
                       Participant2 = new Models.Participant
                                      {
                                          Id = participant2.Id,
                                          Name = participant2.Name
                                      },
                       TurnsHistory = gameResult.TurnsHistory,
                       
                       Participant1Field = GetFieldByTurnsHistory(gameResult.Participant1StartField,
                                                                  gameResult.TurnsHistory, 
                                                                  turn,
                                                                  gameResult.Participant1),
                       
                       Participant2Field = GetFieldByTurnsHistory(gameResult.Participant2StartField,
                                                                  gameResult.TurnsHistory, 
                                                                  turn,
                                                                  gameResult.Participant2)
                   };
        }

        private Row[] GetFieldByTurnsHistory(Row[] startField, SerializableTurnResult[] turnsHistory, int currentTurn,
                                             ParticipantModel participantModel)
        {
            var result = startField;

            for (var i = 0; i < currentTurn; i++)
            {
                var turn = turnsHistory[i];

                if (turn.IngameId == participantModel.IngamePlayerId)
                {
                    continue;
                }

                foreach (var cell in turn.ChangedCells)
                {
                    result[cell.Row].Cells[cell.Column].State = cell.State;
                }
            }

            return result;
        }
    }
}