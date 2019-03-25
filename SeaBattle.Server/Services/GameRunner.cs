namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Engine;
    using Engine.Models;
    using Entities;
    using Models;
    using Newtonsoft.Json;
    using Participant = Entities.Participant;

    public class GameRunner : IGameRunner
    {
        private static object _locker = new object();
        
        private readonly ApplicationContext _dbContext;

        public GameRunner(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public (PlayedGame, GameResult) StartGame(Participant player1, Participant player2, bool ratedGame)
        {
            var participant1 = new SeaBattle.Engine.Models.Participant
                               {
                                   Id = player1.Id,
                                   StrategyAssembly = player1.Strategy
                               };

            var participant2 = new SeaBattle.Engine.Models.Participant
                               {
                                   Id = player2.Id,
                                   StrategyAssembly = player2.Strategy
                               };

            var engine = new Engine(participant1, participant2);
            
            var gameResult = engine.StartGame();

            var newGame = new PlayedGame
                          {
                              Result = JsonConvert.SerializeObject(new SerializableGameResult(gameResult))
                          };

            lock (_locker)
            {
                _dbContext.PlayedGames.Add(newGame);
                _dbContext.SaveChanges();
            }

            return (newGame, gameResult);
        }
    }
}