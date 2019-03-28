namespace SeaBattle.Server.Dal
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public sealed class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) 
            : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<Participant> Participants { get; set; }
        
        public DbSet<Statistic> Statistics { get; set; }
        
        public DbSet<PlayedGame> PlayedGames { get; set; }
        
        public DbSet<StrategySource> StrategySources { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Statistic>()
                     .HasOne(s => s.Participant)
                     .WithOne(p => p.Statistic);

            modelBuilder.Entity<StrategySource>()
                     .HasOne(s => s.Participant)
                     .WithMany(p => p.StrategySources);

        }
    }
}