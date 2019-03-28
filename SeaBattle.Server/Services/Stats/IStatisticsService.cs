namespace SeaBattle.Server.Services.Stats
{
    using System.Threading.Tasks;
    using Dal.Entities;

    public interface IStatisticsService
    {
        Task<string> GetAsync(Participant participant);
    }
}