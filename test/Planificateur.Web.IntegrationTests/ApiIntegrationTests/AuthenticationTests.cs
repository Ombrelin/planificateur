using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Web.Database.Entities;
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
        var (response, statusCode) = await Client.Register(request);

        // Then
        statusCode.Should().Be(HttpStatusCode.Created);
        Assert.NotNull(response);
        response.Username.Should().Be(request.Username);
        response.Id.Should().NotBeEmpty();

        ApplicationUserEntity userInDb = await DbContext.Users.FirstAsync(u => u.Id == response.Id);
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
        var (response, statusCode) = await Client.Login(request);

        // Then
        statusCode.Should().Be(HttpStatusCode.OK);

        Assert.NotNull(response);
        response.Token.Should().NotBe(string.Empty);
    }


    [Fact]
    public async Task Login_WithInCorrectCredentials_DontReturnsToken()
    {
        // Given
        ApplicationUser user = await InsertTestApplicationUser();

        var request = new LoginRequest("invalid username", "invalid passowrd");

        // When
        var (response, statusCode) = await Client.Login(request);

        // Then
        statusCode.Should().Be(HttpStatusCode.NotFound);
    }

    private async Task<ApplicationUser> InsertTestApplicationUser()
    {
        ApplicationUser user = DataFactory.BuildTestUser();
        await DbContext.Users.AddAsync(new ApplicationUserEntity(user));
        await DbContext.SaveChangesAsync();
        return user;
    }
}