namespace SeaBattle.Server.Controllers
{
    using System.Threading.Tasks;
    using Engine.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Models;
    using Newtonsoft.Json;

    public class GameController: Controller
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
            
            var gameResult = JsonConvert.DeserializeObject<SerializableGameResult>(game.Result);
            
            var participant1 =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == gameResult.Participant1Id);
            
            var participant2 =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == gameResult.Participant2Id);

            var gameModel = new Game
                            {
                                Id = game.Id,
                                Turn = 0,
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
                                Participant1Field = gameResult.Participant1StartField,
                                Participant2Field = gameResult.Participant2StartField
                            };

            ViewBag.Title = $"Игра #{game.Id}";
            
            return View("Game", gameModel);
        }

        public async Task<ActionResult> GetTurn(int id, int turn)
        {
            var game = await _dbContext.PlayedGames.FirstOrDefaultAsync(g => g.Id == id);
            if (game == null)
            {
                return NotFound();
            }
            
            var gameResult = JsonConvert.DeserializeObject<SerializableGameResult>(game.Result);

            var participant1 =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == gameResult.Participant1Id);
            
            var participant2 =
                await _dbContext.Participants.FirstOrDefaultAsync(p => p.Id == gameResult.Participant2Id);
            
            var gameModel = new Game
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
                                Participant1Field = GetFieldByTurnsHistory(gameResult.Participant1StartField, gameResult.TurnsHistory, turn, gameResult.Participant1Id),
                                Participant2Field = GetFieldByTurnsHistory(gameResult.Participant2StartField, gameResult.TurnsHistory,  turn, gameResult.Participant2Id)
                            };
            ViewBag.Title = $"Игра #{game.Id}";
            
            return View("Game", gameModel);
        }

        private Row[] GetFieldByTurnsHistory(Row[] startField, SerializableTurnResult[] turnsHistory, int currentTurn, int playerId)
        {
            var result = startField;

            for (var i = 0; i < currentTurn; i++)
            {
                var turn = turnsHistory[i];
                if (turn.PlayerId != playerId)
                {
                    foreach (var cell in turn.ChangedCells)
                    {
                        result[cell.Row].Cells[cell.Column].State = cell.State;
                    }
                }
            }

            return result;
        }
    }
}