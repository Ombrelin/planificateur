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

public abstract class ApiIntegrationTests : IClassFixture<WebApplicationFactoryFixture>, IAsyncLifetime
{
    private readonly WebApplicationFactoryFixture webApplicationFactory;
    protected readonly PlanificateurClient Client;
    protected ApplicationDbContext DbContext = null!;
    protected readonly DataFactory DataFactory = new();
    private readonly TestDatabaseContextFactory databaseContextFactory = new();


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
        var request = new RegisterRequest(
            DataFactory.GetNewUsername(),
            DataFactory.Password
        );
        var (response, statusCode) = await Client.Register(request);
        Assert.Equal(HttpStatusCode.Created, statusCode);
        Assert.NotNull(response);
        
        return response;
    }

    public async Task<LoginResponse> Login(string username)
    {
        var loginRequest = new LoginRequest(
            username,
            DataFactory.Password
        );

        var (response, statusCode) = await Client.Login(loginRequest);
        Assert.Equal(HttpStatusCode.OK, statusCode);
        Assert.NotNull(response);
        return response;
    }



    public Task DisposeAsync() => Task.CompletedTask;
}