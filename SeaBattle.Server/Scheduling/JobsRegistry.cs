namespace SeaBattle.Server.Scheduling
{
    using System;
    using FluentScheduler;
    using Jobs;
    using Models;

    public class JobsRegistry : Registry
    {
        public JobsRegistry(IServiceProvider serviceProvider, GamesConfiguration config)
        {
            if (!config.Enabled)
            {
                return;
            }

            var job = Schedule(() => new GamesJob(serviceProvider));

            if (config.StartNow)
            {
                job.ToRunNow().AndEvery(config.Interval).Minutes();
            }
            else
            {
                job.ToRunEvery(config.Interval).Minutes();
            }
        }
    }
}