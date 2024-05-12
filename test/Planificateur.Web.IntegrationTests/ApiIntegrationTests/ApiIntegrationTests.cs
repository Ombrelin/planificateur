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

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<DatabaseFixture>
{
    protected PlanificateurClient Client;
    protected ApplicationDbContext DbContext;
    protected DataFactory DataFactory = new();

    private static int UserCount = 0;

    public ApiIntegrationTests(WebApplicationFactory<Startup> webApplicationFactory, DatabaseFixture databaseFixture)
    {
        Client = new PlanificateurClient(webApplicationFactory.CreateClient());
        DbContext = databaseFixture.DbContext;
    }

    public async Task<RegisterResponse> RegisterNewUser()
    {
        var request = new RegisterRequest(
            $"{DataFactory.Username}-{++UserCount}",
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
}