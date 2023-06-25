using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Planificateur.Core.Contracts;
using Planificateur.Tests.Shared;
using Planificateur.Web.Database;
using Planificateur.Web.Tests.Database;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Startup>>, IClassFixture<DatabaseFixture>
{
    protected HttpClient Client;
    protected ApplicationDbContext DbContext;
    protected DataFactory DataFactory = new();

    private static int UserCount = 0;
    
    public ApiIntegrationTests(WebApplicationFactory<Startup> webApplicationFactory, DatabaseFixture databaseFixture)
    {
        Client = webApplicationFactory.CreateClient();
        DbContext = databaseFixture.DbContext;
    }

    public async Task<RegisterResponse> RegisterNewUser()
    {
        var request = new RegisterRequest(
            $"{DataFactory.Username}-{++UserCount}",
            DataFactory.Password
        );
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/authentication/register", request);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!;
    }

    public async Task<LoginResponse> Login()
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/api/authentication/login",
            new LoginRequest(
                $"{DataFactory.Username}-{UserCount}",
                DataFactory.Password
            )
        );
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var loginResponse = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;


        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

        return loginResponse;
    }
}