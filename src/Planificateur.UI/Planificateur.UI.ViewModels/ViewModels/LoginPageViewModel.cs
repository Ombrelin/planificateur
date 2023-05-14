using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Planificateur.ClientSdk.Models;
using Planificateur.UI.ViewModels.Services;

namespace Planificateur.UI.ViewModels.ViewModels;

[INotifyPropertyChanged]
public partial class LoginPageViewModel
{
    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string username = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string password = string.Empty;

    [ObservableProperty] [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
    private string serverUrl = string.Empty;

    
    public bool CanLogin => !(string.IsNullOrEmpty(this.Username) || string.IsNullOrEmpty(this.Password));

    private readonly IPlanificateurApi apiClient;
    private readonly INavigationService navigationService;

    public LoginPageViewModel(IPlanificateurApi apiClient, INavigationService navigationService)
    {
        this.apiClient = apiClient;
        this.navigationService = navigationService;
    }

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
        await apiClient.Login(ServerUrl, new LoginRequest {Username = Username, Password = Password});
        await navigationService.NavigateToAsync("home");
    }
}