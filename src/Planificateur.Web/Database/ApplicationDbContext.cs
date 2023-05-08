using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Poll> Polls { get; set; }
    public DbSet<Vote> Votes { get; set; }

    public DbSet<ApplicationUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Poll>(entity =>
        {
            entity.HasKey(poll => poll.Id);
            entity
                .Property(poll => poll.Id)
                .ValueGeneratedNever();
            entity.Property(poll => poll.Name);
            entity.Property(poll => poll.Dates);
            entity.Property(poll => poll.ExpirationDate);
        });

        modelBuilder.Entity<Vote>(entity =>
        {
            entity.HasKey(vote => vote.Id);
            entity
                .Property(vote => vote.Id)
                .ValueGeneratedNever();
            entity.Property(vote => vote.Availabilities);
            entity
                .HasOne<Poll>()
                .WithMany(poll => poll.Votes)
                .HasForeignKey(vote => vote.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.HasKey(user => user.Id);
                entity
                    .Property(vote => vote.Id)
                    .ValueGeneratedNever();
                entity.Property(vote => vote.Username);
                entity.Property(vote => vote.Password);
            }
        );
    }
}