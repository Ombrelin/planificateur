using Avalonia.Controls;
using Planificateur.UI.ViewModels.ViewModels;

namespace Planificateur.UI.Views;

public partial class LoginPage : UserControl
{
    public LoginPage()
    {
        InitializeComponent();
        DataContext = this.CreateInstance<LoginPageViewModel>();
    }
}