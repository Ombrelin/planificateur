using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAvalonia.UI.Controls;
using Planificateur.UI.ViewModels.Services;

namespace Planificateur.UI.Services;

public class NavigationService : INavigationService
{
    private readonly IDictionary<string, Type> registeredPage = new Dictionary<string, Type>();
    private readonly Frame navigationFrame;

    public NavigationService(Frame navigationFrame)
    {
        this.navigationFrame = navigationFrame;
    }

    public void RegisterPage(string pageName, Type pageType)
    {
        this.registeredPage[pageName] = pageType;
    }

    public Task NavigateToAsync(string page)
    {
        this.navigationFrame.Navigate(this.registeredPage[page]);
        return Task.CompletedTask;
    }
}