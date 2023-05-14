using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.Styling;
using FluentAvalonia.UI.Controls;
using Microsoft.Extensions.DependencyInjection;
using Planificateur.UI.Services;
using Planificateur.UI.ViewModels.Services;
using Planificateur.UI.ViewModels.ViewModels;
using Planificateur.UI.Views;

namespace Planificateur.UI;

public partial class App : Application
{
    private readonly IServiceCollection services = new ServiceCollection()
        .AddSingleton<LoginPageViewModel>();

    private ServiceProvider? serviceProvider;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var faTheme = AvaloniaLocator.Current.GetService<FluentAvaloniaTheme>();
        faTheme.RequestedTheme = "Dark";
        Frame? navigationFrame = null;
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow();
            navigationFrame = desktop.MainWindow.FindControl<Frame>("NavigationFrame");
            services.AddSingleton<IStorageService, DesktopStorageService>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            navigationFrame = new Frame();
            singleViewPlatform.MainView = navigationFrame;
            services.AddSingleton<IStorageService, MobileStorageService>();
        }

        var navigationService = new NavigationService(navigationFrame);
        ConfigureNavigation(navigationService);

        base.OnFrameworkInitializationCompleted();
    }

    private void ConfigureNavigation(NavigationService navigationService)
    {
        navigationService.RegisterPage("login", typeof(LoginPage));
        services.AddSingleton<INavigationService>(navigationService);
        serviceProvider = services.BuildServiceProvider();
        this.Resources[typeof(IServiceProvider)] = serviceProvider;
        //IStorageService storageService = serviceProvider.GetService<IStorageService>()
        //                                 ?? throw new InvalidOperationException("Can't get storage service from DI");

        Dispatcher.UIThread.Post(async () => await navigationService.NavigateToAsync("login"));
    }
}