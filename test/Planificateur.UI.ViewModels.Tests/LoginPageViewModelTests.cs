using Moq;
using Planificateur.ClientSdk.Models;
using Planificateur.UI.ViewModels.Services;
using Planificateur.UI.ViewModels.ViewModels;

namespace Planificateur.UI.ViewModels.Tests;

public class LoginPageViewModelTests
{
    private readonly LoginPageViewModel target;

    private readonly Mock<IPlanificateurApi> apiClientMock = new();
    private readonly Mock<INavigationService> navigationServiceMock = new();

    public LoginPageViewModelTests()
    {
        target = new LoginPageViewModel(apiClientMock.Object, navigationServiceMock.Object);
    }

    [Fact]
    public async Task Login_CallsApiAndNavigates()
    {
        // Given
        target.ServerUrl = "https://my.server.com";
        target.Username = "Test User";
        target.Password = "Test Password";

        // When
        await target.LoginCommand.ExecuteAsync(null);

        // Then
        apiClientMock.Verify(
            mock => mock.Login(
                It.Is<string>(url => url == target.ServerUrl),
                It.Is<LoginRequest>(request =>
                    request.Username == target.Username && request.Password == target.Password)),
            Times.Once
        );
        navigationServiceMock.Verify(mock => mock.NavigateToAsync("home"), Times.Once);
    }
}