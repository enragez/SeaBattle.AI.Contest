namespace SeaBattle.Server.Scheduling.Jobs
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentScheduler;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Models;
    using Services;
    using Participant = Entities.Participant;

    public class GamesJob : IJob
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly IGameRunner _gameRunner;

        private readonly IBotService _botService;

        public GamesJob(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            
            _gameRunner = _serviceProvider.GetRequiredService<IGameRunner>();
            
            _botService = _serviceProvider.GetRequiredService<IBotService>();
        }
        
        public async void Execute()
        {
            var dbContext = _serviceProvider.GetRequiredService<ApplicationContext>();
            
            var participantsCount = await dbContext.Participants.CountAsync();

            var participants = await dbContext.Participants.ToListAsync();
                
            if (participantsCount % 2 != 0)
            {
                // remove random participant
                participants = participants.RandomPermutation()
                                        .Take(participantsCount - 1)
                                        .ToList();
            }

            // shuffle players
            participants = participants.RandomPermutation().ToList();

            for (var i = 0; i < participants.Count; i += 2)
            {
                var participant1 = participants[i];
                var participant2 = participants[i + 1];

                await StartGame(dbContext, participant1, participant2);
            }
        }

        private async Task StartGame(ApplicationContext dbContext, Participant participant1, Participant participant2)
        {
            var participant1Statistic =
                await dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant1.Id);

            var participant2Statistic =
                await dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant2.Id);
            
            await _botService.Client.SendTextMessageAsync(_botService.ChannelId,
                                                    $@"Запущена автоматическая игра:

{participant1.Name} (Id: {participant1.Id}, Рейтинг: {participant1Statistic?.Rating ?? 1000}) 
vs
{participant2.Name} (Id: {participant2.Id}, Рейтинг: {participant2Statistic?.Rating ?? 1000})");
            
            var (playedGame, gameResult) = await _gameRunner.StartGameAsync(participant1, participant2, true);

            var winner = gameResult.Winner.Id == participant1.Id
                             ? participant1
                             : participant2;

            var newParticipant1Statistic =
                await dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant1.Id);

            var newParticipant2Statistic =
                await dbContext.Statistic.FirstOrDefaultAsync(s => s.ParticipantId == participant2.Id);
            
            await _botService.Client.SendTextMessageAsync(_botService.ChannelId,
                                                          $@"Завершена автоматическая игра.

{participant1.Name} (Id: {participant1.Id}, Обновленный рейтинг: {newParticipant1Statistic.Rating})
vs
{participant2.Name} (Id: {participant2.Id}, Обновленный рейтинг: {newParticipant2Statistic.Rating})

Победитель: {winner.Name} (Id: {gameResult.Winner.Id})

Подробности: {Utils.GetGameUrl(playedGame)}");
            
        }
    }
}