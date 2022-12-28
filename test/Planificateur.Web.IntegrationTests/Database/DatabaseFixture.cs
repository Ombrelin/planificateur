using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Web.Database;

namespace Planificateur.Web.Tests.Database;

public class DatabaseFixture : IAsyncLifetime {
    public  ApplicationDbContext DbContext { get; }

    public DatabaseFixture()
    {

        DbContext = BuildNewDbContext();
    }
    
    public static ApplicationDbContext BuildNewDbContext()
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("DB_HOST"),
            Port = int.Parse(Environment.GetEnvironmentVariable("DB_PORT")),
            Username = Environment.GetEnvironmentVariable("DB_USERNAME"),
            Password = Environment.GetEnvironmentVariable("DB_PASSWORD"),
            Database = Environment.GetEnvironmentVariable("DB_NAME")
        };
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(builder.ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task InitializeAsync() => await DbContext.Database.MigrateAsync();

    public async Task DisposeAsync()
    {
        await DbContext.Database.EnsureDeletedAsync();
        await DbContext.DisposeAsync();
    }
}