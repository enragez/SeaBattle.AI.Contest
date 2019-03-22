namespace SeaBattle.Server.Services.Stats
{
    using Entities;

    public interface IStatisticsService
    {
        string Get(Participant participant);
    }
}