namespace SeaBattle.Server.Models
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public sealed class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        {
        }
        
        public DbSet<SeaBattle.Server.Entities.Participant> Participants { get; set; }
        
        public DbSet<Statistic> Statistic { get; set; }
        
        public DbSet<PlayedGame> PlayedGames { get; set; }
        
        public DbSet<StrategySource> StrategySources { get; set; }
    }
}