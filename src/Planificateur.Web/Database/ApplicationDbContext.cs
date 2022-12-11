using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Entities;

namespace Planificateur.Web.Database;

public class ApplicationDbContext : DbContext {

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
            entity
                .HasOne<Poll>()
                .WithMany(poll => poll.Votes)
                .HasForeignKey(vote => vote.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

}