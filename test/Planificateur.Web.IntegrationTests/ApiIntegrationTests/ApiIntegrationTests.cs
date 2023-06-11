using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Planificateur.Core.Contracts;
using Planificateur.Tests.Shared;
using Planificateur.Web.Database;
using Planificateur.Web.Tests.Database;
using Xunit;

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

    public async Task<RegisterResponse> RegisterNewUser()
    {
        var request = new RegisterRequest(
            DataFactory.Username,
            DataFactory.Password
        );
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/authentication/register", request);

        return (await response.Content.ReadFromJsonAsync<RegisterResponse>())!;
    }

    public async Task<LoginResponse> Login()
    {
        HttpResponseMessage response = await Client.PostAsJsonAsync(
            "/api/authentication/login",
            new LoginRequest(
                DataFactory.Username,
                DataFactory.Password
            )
        );
        var loginResponse = (await response.Content.ReadFromJsonAsync<LoginResponse>())!;

        Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", loginResponse.Token);

        return loginResponse;
    }
}