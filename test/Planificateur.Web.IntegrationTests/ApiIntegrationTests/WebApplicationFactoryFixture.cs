using Microsoft.AspNetCore.Mvc.Testing;
using Planificateur.Web.Tests.Database;
using Testcontainers.PostgreSql;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

public class WebApplicationFactoryFixture : IDisposable
{
    public DatabaseFixture DatabaseFixture { get; }
    public WebApplicationFactory<Startup>? WebApplicationFactory { get; private set; }

    public WebApplicationFactoryFixture(DatabaseFixture databaseFixture)
    {
        this.DatabaseFixture = databaseFixture;
        Environment.SetEnvironmentVariable("JWT_SECRET", "this is a secret, please don't tell anyone about it");
        WebApplicationFactory = new WebApplicationFactory<Startup>();
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("JWT_SECRET", string.Empty);
    }
}