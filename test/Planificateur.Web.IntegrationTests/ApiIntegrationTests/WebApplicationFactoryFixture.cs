using Microsoft.AspNetCore.Mvc.Testing;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

public class WebApplicationFactoryFixture : IAsyncLifetime
{
    public WebApplicationFactory<Startup> WebApplicationFactory { get; }
    
    public Task InitializeAsync()
    {
        throw new NotImplemented
        Exception();
    }

    public Task DisposeAsync()
    {
        throw new NotImplementedException();
    }
}