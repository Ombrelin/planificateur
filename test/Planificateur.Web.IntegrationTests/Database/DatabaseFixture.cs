using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Web.Database;
using Testcontainers.PostgreSql;

namespace Planificateur.Web.Tests.Database;

public class DatabaseFixture : IAsyncLifetime
{
    private static PostgreSqlContainer? _postgresContainer;
    public ApplicationDbContext DbContext => BuildNewDbContext();
    
    public static ApplicationDbContext BuildNewDbContext()
    {
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(_postgresContainer.GetConnectionString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public async Task InitializeAsync()
    {
        bool isContinuousIntegration = bool.Parse(Environment.GetEnvironmentVariable("IS_CI") ?? bool.FalseString);
        var databaseAlias = "database";
        const string postgresUsername = "user";
        const string postgresPassword = "password";
        const string postgresDatabase = "planificateur";
        const int postgresPort = 5432;
        var postgreSqlContainerBuilder = new PostgreSqlBuilder()
            .WithPortBinding(postgresPort, postgresPort)
            .WithUsername(postgresUsername)
            .WithPassword(postgresPassword)
            .WithDatabase(postgresDatabase)
            .WithNetworkAliases(databaseAlias);

        if (isContinuousIntegration)
        {
            postgreSqlContainerBuilder = postgreSqlContainerBuilder
                .WithNetwork("network");
        }

        _postgresContainer = postgreSqlContainerBuilder.Build();
        await _postgresContainer.StartAsync();
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (_postgresContainer is not null)
        {
            await _postgresContainer.DisposeAsync();
        }
        
    }
}