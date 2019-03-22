namespace SeaBattle.Server.Services.Stats
{
    using System.Text;
    using Entities;

    public class StatisticsService : IStatisticsService
    {
        public string Get(Participant participant)
        {
            if (participant.Statistic == null)
            {
                return @"Статистика не найдена.
Возможно игрок не зарегистрирован или пока не учавствовал в играх с учетом статистики.";
            }
            
            var message = new StringBuilder($"Статистика игрока {participant.Name}, Id {participant.Id}");
            message.AppendLine();
            message.AppendLine($"Игр сыграно: {participant.Statistic.GamesPlayed}");
            message.AppendLine($"Побед: {participant.Statistic.Wins}");
            message.AppendLine($"Поражений: {participant.Statistic.Losses}");
            message.AppendLine($"Рейтинг: {participant.Statistic.Rating}");

            return message.ToString();
        }
    }
}