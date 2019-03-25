namespace SeaBattle.Server.Services
{
    using System;
    using System.Threading.Tasks;
    using Engine;
    using Engine.Models;
    using Engine.Models.Serializable;
    using Entities;
    using Microsoft.EntityFrameworkCore;
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

        public async Task<(PlayedGame, GameResult)> StartGameAsync(Participant player1, Participant player2, bool ratedGame)
        {
            var participant1 = new PlayerDto
                               {
                                   Id = player1.Id,
                                   StrategyAssembly = player1.Strategy
                               };

            var participant2 = new PlayerDto
                               {
                                   Id = player2.Id,
                                   StrategyAssembly = player2.Strategy
                               };

            var engine = new Engine(participant1, participant2);
            
            var gameResult = engine.StartGame();

            var serializableResult = new SerializableGameResult(gameResult);

            // hack: engine cannot remove from serialization
            serializableResult.Participant1.PlayerDto.StrategyAssembly = null;
            serializableResult.Participant2.PlayerDto.StrategyAssembly = null;
            
            var newGame = new PlayedGame
                          {
                              Result = JsonConvert.SerializeObject(new SerializableGameResult(gameResult))
                          };
            _dbContext.PlayedGames.Add(newGame);

            var saved = false;

            while (!saved)
            {
                try
                {
                    await _dbContext.SaveChangesAsync();
                    
                    saved = true;
                }
                catch (DbUpdateConcurrencyException e)
                {
                }
            }

            return (newGame, gameResult);
        }
    }
}