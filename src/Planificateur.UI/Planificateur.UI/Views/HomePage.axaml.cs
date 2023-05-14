using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Planificateur.UI.ViewModels.ViewModels;

namespace Planificateur.UI.Views;

public partial class HomePage : UserControl
{
    public HomePage()
    {
        InitializeComponent();
        DataContext = this.CreateInstance<HomePageViewModel>();
    }
    
}