using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

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

    [RelayCommand(AllowConcurrentExecutions = false, CanExecute = nameof(CanLogin))]
    private async Task Login()
    {
    }
}