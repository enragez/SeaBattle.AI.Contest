namespace SeaBattle.Server.Services
{
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
                              Rated = ratedGame,
                              Result = JsonConvert.SerializeObject(new SerializableGameResult(gameResult))
                          };
            _dbContext.PlayedGames.Add(newGame);

            if (ratedGame)
            {
                await UpdatePlayerStatistic(player1, gameResult);
                await UpdatePlayerStatistic(player2, gameResult);
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

        private async Task UpdatePlayerStatistic(Participant participant, GameResult gameResult)
        {
            var participantStatistic = await _dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant.Id);
            
            if (participantStatistic == null)
            {
                participantStatistic = new Statistic
                                        {
                                            Wins = 0,
                                            Losses = 0,
                                            Rating = 1000,
                                            GamesPlayed = 0,
                                            Participant = participant
                                        };

                _dbContext.Statistic.Add(participantStatistic);
            }

            participantStatistic.GamesPlayed = participantStatistic.GamesPlayed + 1;
            
            if (gameResult.Winner.Id == participant.Id)
            {
                participantStatistic.Wins = participantStatistic.Wins + 1;
                participantStatistic.Rating = participantStatistic.Rating + 25;
            }
            else
            {
                participantStatistic.Losses = participantStatistic.Losses + 1;
                participantStatistic.Rating = participantStatistic.Rating - 25;
            }
        }
    }
}