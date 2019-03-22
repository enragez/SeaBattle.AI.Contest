namespace SeaBattle.Server.Models
{
    using Entities;
    using Microsoft.EntityFrameworkCore;

    public sealed class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        
        public DbSet<SeaBattle.Server.Entities.Participant> Participants { get; set; }
        
        public DbSet<PlayedGame> PlayedGames { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.Participant>()
                     .HasOne(e => e.Statistic).WithOne(e => e.Participant)
                     .HasForeignKey<Statistic>(e => e.ParticipantId);
        }
    }
}