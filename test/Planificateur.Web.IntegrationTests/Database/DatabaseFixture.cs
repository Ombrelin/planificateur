using Testcontainers.PostgreSql;

namespace Planificateur.Web.Tests.Database;

public class DatabaseFixture : IAsyncLifetime
{
    private readonly static string DatabaseName = "planificateur-integration-tests";
    private readonly static string DatabaseUsername = "test";
    private readonly static string DatabasePassword = "test";
    
    public PostgreSqlContainer Database { get; } = new PostgreSqlBuilder()
        .WithDatabase(DatabaseName)
        .WithUsername(DatabaseUsername)
        .WithPassword(DatabasePassword)
        .Build();
    
    public async Task InitializeAsync()
    {
        await Database.StartAsync();
        Environment.SetEnvironmentVariable("DB_PORT", Database.GetMappedPublicPort(PostgreSqlBuilder.PostgreSqlPort).ToString());
        Environment.SetEnvironmentVariable("DB_HOST", Database.Hostname);
        Environment.SetEnvironmentVariable("DB_USERNAME", DatabaseUsername);
        Environment.SetEnvironmentVariable("DB_PASSWORD", DatabasePassword);
        Environment.SetEnvironmentVariable("DB_NAME", DatabaseName);
    }

    public async Task DisposeAsync()
    {
        Environment.SetEnvironmentVariable("DB_PORT", string.Empty);
        Environment.SetEnvironmentVariable("DB_HOST", string.Empty);
        Environment.SetEnvironmentVariable("DB_USERNAME", string.Empty);
        Environment.SetEnvironmentVariable("DB_PASSWORD", string.Empty);
        Environment.SetEnvironmentVariable("DB_NAME", string.Empty);
        await Database.DisposeAsync();
    }
}