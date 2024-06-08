using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Planificateur.ClientSdk.ClientSdk;
using Planificateur.Core.Contracts;
using Planificateur.Tests.Shared;
using Planificateur.Web.Database;
using Planificateur.Web.Tests.Database;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactoryFixture>, IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture webApplicationFactory;
    protected PlanificateurClient Client;
    protected ApplicationDbContext DbContext = null!;
    protected DataFactory DataFactory = new();
    private readonly TestDatabaseContextFactory databaseContextFactory = new();

    private static object userCountLock = new();
    private static int UserCount = 0;

    public ApiIntegrationTests(WebApplicationFactoryFixture webApplicationFactory)
    {
        this.webApplicationFactory = webApplicationFactory;
        Client = new PlanificateurClient(webApplicationFactory.WebApplicationFactory!.CreateClient());
    }

    public async Task InitializeAsync()
    {
        DbContext = await databaseContextFactory
            .BuildNewDbContext(webApplicationFactory.DatabaseFixture.Database.GetConnectionString());
    }
    
    public async Task<RegisterResponse> RegisterNewUser()
    {
        lock (userCountLock)
        {
            UserCount++;
        }
        
        var request = new RegisterRequest(
            $"{DataFactory.Username}-{UserCount}",
            DataFactory.Password
        );
        var (response, statusCode) = await Client.Register(request);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.Created, statusCode);
        
        return response;
    }

    public async Task<LoginResponse> Login()
    {
        var loginRequest = new LoginRequest(
            $"{DataFactory.Username}-{UserCount}",
            DataFactory.Password
        );

        var (response, statusCode) = await Client.Login(loginRequest);
        Assert.NotNull(response);
        Assert.Equal(HttpStatusCode.OK, statusCode);
        return response;
    }



    public Task DisposeAsync() => Task.CompletedTask;
}