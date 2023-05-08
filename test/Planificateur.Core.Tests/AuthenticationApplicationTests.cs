using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FluentAssertions;
using Microsoft.IdentityModel.Tokens;
using Planificateur.Core.Contracts;
using Planificateur.Core.Entities;
using Planificateur.Core.Tests.Repositories;
using Planificateur.Tests.Shared;

namespace Planificateur.Core.Tests;

public class AuthenticationApplicationTests
{
    private readonly DataFactory dataFactory = new();

    private const string JwtSecret = "my-secret-string-to-sign-jwt-token";
    private readonly FakeApplicationUserRepository fakeRepository = new();
    private readonly AuthenticationApplication target;

    public AuthenticationApplicationTests()
    {
        target = new AuthenticationApplication(fakeRepository, JwtSecret);
    }

    [Fact]
    public async Task Register_Nominal_InsertsUserWithHashedPassword()
    {
        // When
        RegisterResponse result = await target.Register(
            new RegisterRequest(dataFactory.Username, dataFactory.Password)
        );

        // Then
        result.Username.Should().Be(dataFactory.Username);

        fakeRepository.Data.Should().HaveCount(1);
        fakeRepository.Data.First().Value.Username.Should().Be(dataFactory.Username);
        BCrypt.Net.BCrypt.Verify(dataFactory.Password, fakeRepository.Data.First().Value.Password);
    }

    [Fact]
    public async Task Register_UserAlreadyExists_Throws()
    {
        // Given
        ApplicationUser user = dataFactory.BuildTestUser();
        await fakeRepository.Insert(user);

        // When
        var act = async () => await target.Register(
            new RegisterRequest(user.Username, dataFactory.Password)
        );

        // Then
        await act.Should().ThrowAsync<ArgumentException>();
    }


    [Fact]
    public async Task Login_GoodCredentials_ReturnsJwtToken()
    {
        // Given
        var tokenValidationSpecs = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtSecret)),
            ValidateIssuer = false,
            ValidateAudience = false
        };

        ApplicationUser applicationUser = dataFactory.BuildTestUser();

        await fakeRepository.Insert(applicationUser);

        // When
        LoginResponse response = await target.Login(
            new LoginRequest(applicationUser.Username, dataFactory.Password)
        );

        // Then
        string tokenString = response.Token;
        var handler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal = handler.ValidateToken(tokenString, tokenValidationSpecs, out _);

        var claims = principal.Claims.ToArray();
        Claim firstClaim = claims[0];
        Claim secondClaim = claims[1];

        firstClaim.Type.Should().Be(ClaimTypes.NameIdentifier);
        firstClaim.Value.Should().Be(applicationUser.Id.ToString());
        secondClaim.Type.Should().Be(ClaimTypes.Name);
        secondClaim.Value.Should().Be(applicationUser.Username);
    }

    [Fact]
    public async Task Login_BadCredentials_ThrowsException()
    {
        // Given
        ApplicationUser applicationUser = this.dataFactory.BuildTestUser();
        await fakeRepository.Insert(applicationUser);

        // When
        Func<Task> act = async () =>
            await target.Login(new LoginRequest(applicationUser.Username, "not the right password"));

        // Then
        await act.Should().ThrowAsync<ArgumentException>();
    }
}