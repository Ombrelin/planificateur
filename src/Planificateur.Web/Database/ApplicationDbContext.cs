using Microsoft.EntityFrameworkCore;
using Planificateur.Web.Database.Entities;

namespace Planificateur.Web.Database;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<PollEntity> Polls { get; set; }
    public DbSet<VoteEntity> Votes { get; set; }

    public DbSet<ApplicationUserEntity> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<PollEntity>(entity =>
        {
            entity.ToTable("Polls");
            entity.HasKey(poll => poll.Id);
            entity
                .Property(poll => poll.Id)
                .ValueGeneratedNever();
            entity.Property(poll => poll.Name);
            entity.Property(poll => poll.Dates);
            entity.Property(poll => poll.ExpirationDate);
            entity.Property(poll => poll.AuthorId);
        });

        modelBuilder.Entity<VoteEntity>(entity =>
        {
            entity.ToTable("Votes");
            entity.HasKey(vote => vote.Id);
            entity
                .Property(vote => vote.Id)
                .ValueGeneratedNever();
            entity.Property(vote => vote.Availabilities);
            entity
                .HasOne<PollEntity>()
                .WithMany(poll => poll.Votes)
                .HasForeignKey(vote => vote.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ApplicationUserEntity>(entity =>
            {
                entity.ToTable("Users");
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