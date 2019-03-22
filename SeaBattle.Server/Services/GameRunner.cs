namespace SeaBattle.Server.Services
{
    using System.IO;
    using System.Threading.Tasks;
    using Engine;
    using Engine.Models;
    using Entities;
    using Models;
    using Newtonsoft.Json;
    using Participant = Entities.Participant;

    public class GameRunner : IGameRunner
    {
        private readonly ApplicationContext _dbContext;

        public GameRunner(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GameResult> StartGameAsync(Participant player1, Participant player2, bool ratedGame)
        {
            var player1StrategyFilePath = Path.Combine(Path.GetTempPath(), $"{player1.Id}.dll");
            if (File.Exists(player1StrategyFilePath))
            {
                File.Delete(player1StrategyFilePath);
            }

            File.WriteAllBytes(player1StrategyFilePath, player1.Strategy);

            var player2StrategyFilePath = Path.Combine(Path.GetTempPath(), $"{player2.Id}.dll");
            if (File.Exists(player1StrategyFilePath))
            {
                File.Delete(player1StrategyFilePath);
            }

            File.WriteAllBytes(player2StrategyFilePath, player2.Strategy);

            var newGame = new PlayedGame();
            _dbContext.PlayedGames.Add(newGame);

            await _dbContext.SaveChangesAsync();

            var gameResult = await RunGame(newGame.Id, player1, player2, player1StrategyFilePath, player2StrategyFilePath);

            newGame.Result = JsonConvert.SerializeObject(gameResult);

            await _dbContext.SaveChangesAsync();

            return gameResult;
        }

        private Task<GameResult> RunGame(int gameId, Participant player1, Participant player2,
                                         string player1StrategyFilePath, string player2StrategyFilePath)
        {
            return new Task<GameResult>(() =>
                                        {
                                            var participant1 = new SeaBattle.Engine.Models.Participant
                                                               {
                                                                   Id = player1.Id,
                                                                   StrategyAssemblyPath = player1StrategyFilePath
                                                               };

                                            var participant2 = new SeaBattle.Engine.Models.Participant
                                                               {
                                                                   Id = player2.Id,
                                                                   StrategyAssemblyPath = player2StrategyFilePath
                                                               };

                                            var engine = new Engine(gameId, participant1, participant2);

                                            return engine.StartGame();
                                        });
        }
    }
}