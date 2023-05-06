using Microsoft.AspNetCore.Mvc.Testing;
using Planificateur.Tests.Shared;
using Planificateur.Web.Database;
using Planificateur.Web.Tests.Database;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<DatabaseFixture>
{
    protected HttpClient Client;
    protected ApplicationDbContext DbContext;
    protected DataFactory DataFactory = new();

    public ApiIntegrationTests(WebApplicationFactory<Startup> webApplicationFactory, DatabaseFixture databaseFixture)
    {
        Client = webApplicationFactory.CreateClient();
        DbContext = databaseFixture.DbContext;
    }
}