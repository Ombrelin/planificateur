using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Web.Database;

namespace Planificateur.Web.Tests.Database;

public class DatabaseFixture : IAsyncLifetime
{
    public ApplicationDbContext DbContext => BuildNewDbContext();
    
    public static ApplicationDbContext BuildNewDbContext()
    {
        string? dbPort = Environment.GetEnvironmentVariable("DB_PORT");
        string? dbHost = Environment.GetEnvironmentVariable("DB_HOST");
        string? dbUserName = Environment.GetEnvironmentVariable("DB_USERNAME");
        string? dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
        string? dbName = Environment.GetEnvironmentVariable("DB_NAME");

        foreach (string? parameter in new[] { dbPort, dbHost, dbUserName, dbPassword, dbName })
        {
            ArgumentException.ThrowIfNullOrEmpty(parameter);
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = dbHost,
            Port = int.Parse(dbPort),
            Username = dbUserName,
            Password = dbPassword,
            Database = dbName,
            IncludeErrorDetail = true
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