using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Web.Tests.Database;

namespace Planificateur.Web.Tests.ApiIntegrationTests;

[Collection("Database Tests")]
public class AuthenticationTests : ApiIntegrationTests
{
    public AuthenticationTests(WebApplicationFactory<Startup> webApplicationFactory,
        DatabaseFixture databaseFixture) : base(
        webApplicationFactory, databaseFixture)
    {
    }

    [Fact]
    public async Task Register_InsertsNewUserInDb()
    {
        // Given
        var request = new RegisterRequest(
            DataFactory.Username,
            DataFactory.Password
        );

        // When
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/authentication/register", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        var payload = await response.Content.ReadFromJsonAsync<RegisterResponse>();
        Assert.NotNull(payload);
        payload.Username.Should().Be(request.Username);
        payload.Id.Should().NotBeEmpty();

        ApplicationUser userInDb = await DbContext.Users.FirstAsync(u => u.Id == payload.Id);
        userInDb.Username.Should().Be(request.Username);
        userInDb.Password.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Login_WithCorrectCredentials_ReturnsToken()
    {
        // Given
        ApplicationUser user = await InsertTestApplicationUser();
        var request = new LoginRequest(user.Username, DataFactory.Password);

        // When
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/authentication/login", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var result = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(result);
        result.Token.Should().NotBe(string.Empty);
    }


    [Fact]
    public async Task Login_WithInCorrectCredentials_DontReturnsToken()
    {
        // Given
        ApplicationUser user = await InsertTestApplicationUser();

        var request = new LoginRequest("invalid username", "invalid passowrd");

        // When
        HttpResponseMessage response = await Client.PostAsJsonAsync("/api/authentication/login", request);

        // Then
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<ApplicationUser> InsertTestApplicationUser()
    {
        ApplicationUser user = DataFactory.BuildTestUser();
        await DbContext.Users.AddAsync(user);
        await DbContext.SaveChangesAsync();
        return user;
    }
}