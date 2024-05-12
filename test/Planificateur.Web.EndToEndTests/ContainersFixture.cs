using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace Planificateur.Web.EndToEndTests;

public class ContainersFixture : IAsyncLifetime
{
    public IContainer? ApplicationContainer { get; private set; }
    private PostgreSqlContainer? postgreSqlContainer;
    
    public async Task InitializeAsync()
    {
        bool isContinuousIntegration = bool.Parse(Environment.GetEnvironmentVariable("IS_CI") ?? bool.FalseString);
        var applicationImageName = "planficateur-tests-e2e";
        var applicationImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(CommonDirectoryPath.GetSolutionDirectory(), string.Empty)
            .WithDockerfile("src/Planificateur.Web/Dockerfile")
            .WithName(applicationImageName)
            .Build();
        await applicationImage.CreateAsync();

        const string postgresUsername = "user";
        const string postgresPassword = "password";
        const string postgresDatabase = "planificateur";
        const int postgresPort = 5432;
        
        var network = isContinuousIntegration ? new ExistingNetwork("network") : new NetworkBuilder()
            .WithName("network")
            .Build();

        var databaseAlias = "database";
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
        else
        {
            postgreSqlContainerBuilder = postgreSqlContainerBuilder
                .WithNetwork(network);
        }

        postgreSqlContainer = postgreSqlContainerBuilder.Build();
        
        const int appPort = 5000;
        var applicationContainerBuilder = new ContainerBuilder()
            .WithImage(applicationImageName)
            .WithPortBinding(appPort, 8080)
            .WithEnvironment("DB_HOST", databaseAlias)
            .WithEnvironment("DB_NAME", postgresDatabase)
            .WithEnvironment("DB_PASSWORD", postgresPassword)
            .WithEnvironment("DB_PORT", postgresPort.ToString())
            .WithEnvironment("DB_USERNAME", postgresUsername)
            .WithEnvironment("JWT_SECRET", "this-is-a-secret-dont-tell-anyone")
            .WithNetworkAliases("application")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilMessageIsLogged(@".*(Application started).*"));

        if (isContinuousIntegration)
        {
            applicationContainerBuilder = applicationContainerBuilder
                .WithNetwork("network");
        }
        else
        {
            applicationContainerBuilder = applicationContainerBuilder
                .WithNetwork(network);
        }

        ApplicationContainer = applicationContainerBuilder.Build();

        if (!isContinuousIntegration)
        {
            await network.CreateAsync();
        }
        
        await postgreSqlContainer.StartAsync();
        await ApplicationContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (postgreSqlContainer is not null)
        {
            await postgreSqlContainer.DisposeAsync();
        }
        if (ApplicationContainer is not null)
        {
            await ApplicationContainer.DisposeAsync();
        }
    }
    
    private class ExistingNetwork(string name) : INetwork
    {
        public Task CreateAsync(CancellationToken ct = new CancellationToken())
        {
            return Task.CompletedTask;  
        }

        public Task DeleteAsync(CancellationToken ct = new CancellationToken())
        {
            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }

        public string Name { get; } = name;
    }
}