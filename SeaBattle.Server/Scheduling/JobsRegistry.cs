namespace SeaBattle.Server.Scheduling
{
    using System;
    using FluentScheduler;
    using Jobs;

    public class JobsRegistry : Registry
    {
        public JobsRegistry(IServiceProvider serviceProvider)
        {
            Schedule(() => new GamesJob(serviceProvider)).ToRunNow().AndEvery(1).Minutes();
        }
    }
}