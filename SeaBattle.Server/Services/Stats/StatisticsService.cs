namespace SeaBattle.Server.Services.Stats
{
    using System.Text;
    using System.Threading.Tasks;
    using Dal;
    using Microsoft.EntityFrameworkCore;
    using Participant = Dal.Entities.Participant;

    public class StatisticsService : IStatisticsService
    {
        private readonly ApplicationContext _dbContext;

        public StatisticsService(ApplicationContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<string> GetAsync(Participant participant)
        {
            var statistic = await _dbContext.Statistics.FirstOrDefaultAsync(s => s.ParticipantId == participant.Id);
            
            if (statistic == null)
            {
                return @"Статистика не найдена.
Возможно игрок не зарегистрирован или пока не учавствовал в играх с учетом статистики.";
            }
            
            var message = new StringBuilder($"Статистика игрока {participant.Name}, Id {participant.Id}");
            message.AppendLine();
            message.AppendLine($"Игр сыграно: {statistic.GamesPlayed}");
            message.AppendLine($"Побед: {statistic.Wins}");
            message.AppendLine($"Поражений: {statistic.Losses}");
            message.AppendLine($"Рейтинг: {statistic.Rating}");

            return message.ToString();
        }
    }
}