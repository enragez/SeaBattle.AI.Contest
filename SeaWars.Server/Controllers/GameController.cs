using System.Web.Mvc;

namespace SeaWars.Server.Controllers
{
    using Engine.Models;
    using Models;
    using Newtonsoft.Json;

    public class GameController : Controller
    {
        // GET
        public ActionResult Index()
        {
            return Redirect("Home");
        }

        private const string MockGameFilePath = @"C:\Users\a.ivoylov\AppData\Local\Temp\tmpBCAB.json";
        
        [HttpGet]
        public ActionResult Get(int id)
        {
            var gameResultStr = System.IO.File.ReadAllText(MockGameFilePath);

            var gameResult = JsonConvert.DeserializeObject<SerializableGameResult>(gameResultStr);

            var gameModel = new Game
                            {
                                Id = gameResult.Id,
                                Turn = 0,
                                StartTime = gameResult.StartTime,
                                EndTime = gameResult.EndTime,
                                Participant1 = new Models.Participant
                                               {
                                                   Id = gameResult.Participant1Id,
                                                   Name = "Вася"
                                               },
                                Participant2 = new Models.Participant
                                               {
                                                   Id = gameResult.Participant2Id,
                                                   Name = "Петя"
                                               },
                                TurnsHistory = gameResult.TurnsHistory,
                                Participant1Field = gameResult.Participant1StartField,
                                Participant2Field = gameResult.Participant2StartField
                            };

            ViewBag.Title = $"Игра #{gameResult.Id}";
            
            return View("Game", gameModel);
        }

        public ActionResult GetTurn(int id, int turn)
        {
            var gameResultStr = System.IO.File.ReadAllText(MockGameFilePath);

            var gameResult = JsonConvert.DeserializeObject<SerializableGameResult>(gameResultStr);
            
            var gameModel = new Game
                            {
                                Id = gameResult.Id,
                                Turn = turn,
                                StartTime = gameResult.StartTime,
                                EndTime = gameResult.EndTime,
                                Participant1 = new Models.Participant
                                               {
                                                   Id = gameResult.Participant1Id,
                                                   Name = "Вася"
                                               },
                                Participant2 = new Models.Participant
                                               {
                                                   Id = gameResult.Participant2Id,
                                                   Name = "Петя"
                                               },
                                TurnsHistory = gameResult.TurnsHistory,
                                Participant1Field = GetFieldByTurnsHistory(gameResult.Participant1StartField, gameResult.TurnsHistory, turn, gameResult.Participant1Id),
                                Participant2Field = GetFieldByTurnsHistory(gameResult.Participant2StartField, gameResult.TurnsHistory,  turn, gameResult.Participant2Id)
                            };
            ViewBag.Title = $"Игра #{gameResult.Id}";
            
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