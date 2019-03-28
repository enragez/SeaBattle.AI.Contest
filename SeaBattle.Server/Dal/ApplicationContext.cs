namespace SeaBattle.Server.Dal
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public sealed class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        {
        }
        
        public DbSet<Participant> Participants { get; set; }
        
        public DbSet<Statistic> Statistic { get; set; }
        
        public DbSet<PlayedGame> PlayedGames { get; set; }
        
        public DbSet<StrategySource> StrategySources { get; set; }
    }
}