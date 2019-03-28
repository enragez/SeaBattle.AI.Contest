namespace SeaBattle.Server.Services
{
    using System.Threading.Tasks;
    using Dal;
    using Dal.Entities;
    using Engine;
    using Engine.Models;
    using Engine.Models.Serializable;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Rating;
    using Participant = Dal.Entities.Participant;

    public class GameRunner : IGameRunner
    {
        private readonly ApplicationContext _dbContext;
        
        private readonly IEloRatingCalculator _eloRatingCalculator;

        public GameRunner(ApplicationContext dbContext, IEloRatingCalculator eloRatingCalculator)
        {
            _dbContext = dbContext;
            _eloRatingCalculator = eloRatingCalculator;
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
                              Rated = ratedGame,
                              Result = JsonConvert.SerializeObject(new SerializableGameResult(gameResult))
                          };
            _dbContext.PlayedGames.Add(newGame);

            if (ratedGame)
            {
                await UpdatePlayerStatistics(player1, player2, gameResult);
            }

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

        private async Task UpdatePlayerStatistics(Participant participant1, Participant participant2,
                                                  GameResult gameResult)
        {
            var participant1Stats = await _dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant1.Id);
            
            if (participant1Stats == null)
            {
                participant1Stats = new Statistic
                                       {
                                           Wins = 0,
                                           Losses = 0,
                                           Rating = 1000,
                                           GamesPlayed = 0,
                                           Participant = participant1
                                       };

                _dbContext.Statistic.Add(participant1Stats);
            }
            
            var participant2Stats = await _dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant2.Id);
            
            if (participant2Stats == null)
            {
                participant2Stats = new Statistic
                                    {
                                        Wins = 0,
                                        Losses = 0,
                                        Rating = 1000,
                                        GamesPlayed = 0,
                                        Participant = participant2
                                    };

                _dbContext.Statistic.Add(participant2Stats);
            }
            
            participant1Stats.GamesPlayed = participant1Stats.GamesPlayed + 1;
            participant2Stats.GamesPlayed = participant2Stats.GamesPlayed + 1;

            var participant1Score = gameResult.Winner.Id == participant1.Id
                                        ? 1
                                        : 0;
            
            var participant2Score = gameResult.Winner.Id == participant2.Id
                                        ? 1
                                        : 0;

            var (player1NewRating, player2NewRating) = _eloRatingCalculator.Calculate(participant1Stats.Rating,
                                                                                      participant2Stats.Rating,
                                                                                      participant1Score,
                                                                                      participant2Score);

            participant1Stats.Rating = player1NewRating;
            participant2Stats.Rating = player2NewRating;
            
            if (gameResult.Winner.Id == participant1.Id)
            {
                participant1Stats.Wins = participant1Stats.Wins + 1;
                participant2Stats.Losses = participant2Stats.Losses + 1;
            }
            else
            {
                participant2Stats.Wins = participant2Stats.Wins + 1;
                participant1Stats.Losses = participant1Stats.Losses + 1;
            }
        }
    }
}