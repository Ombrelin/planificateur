using FluentAssertions;
using Planificateur.Core.Entities;

namespace Planificateur.Core.Tests.Entities;

public class ApplicationUserTests
{
    [Fact]
    public void Constructor_Creation_SetsProperties()
    {
        // Given
        const string username = "John Shepard";
        const string password = "Test1";

        // When
        var user = new ApplicationUser(username, password);

        // Then
        user.Id.Should().NotBeEmpty();
        user.Username.Should().Be(username);
    }

    [Fact]
    public void Constructor_Reconstitution_SetsProperties()
    {
        // Given
        const string username = "John Shepard";
        const string password = "Test1";
        var id = Guid.NewGuid();

        // When
        var user = new ApplicationUser(id, username, password);

        // Then
        user.Id.Should().Be(id);
        user.Username.Should().Be(username);
        user.Password.Should().Be(password);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public void User_NotNullOrEmpty_Throws(string? value)
    {
        // Given
        string? username = value;
        const string password = "Test1";

        // When
        var act = () => new ApplicationUser(username, password);

        // Then
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("test")]
    [InlineData("Test")]
    [InlineData("test1")]
    [InlineData("Tes1")]
    [InlineData("1")]
    public void Password_InvalidValues_Throws(string? value)
    {
        // Given
        const string username = "John Shepard";
        string? password = value;

        // When
        var act = () => new ApplicationUser(username, password);

        // Then
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void VerifyPassword_RightPassword_ReturnTrue()
    {
        // Given
        const string username = "John Shepard";
        const string password = "Test1";
        var user = new ApplicationUser(username, password);

        // When
        bool result = user.VerifyPassword(password);

        // Then
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WrongPassword_ReturnFalse()
    {
        // Given
        const string username = "John Shepard";
        const string password = "Test1";
        var user = new ApplicationUser(username, password);

        // When
        bool result = user.VerifyPassword("not the right password");

        // Then
        result.Should().BeFalse();
    }

    [Fact]
    public void Constructor_Creation_ComputesPasswordBCrypt()
    {
        // Given
        const string username = "John Shepard";
        const string password = "Test1";

        // When
        var user = new ApplicationUser(username, password);

        // Then
        BCrypt.Net.BCrypt.Verify(password, user.Password).Should().BeTrue();
    }
}