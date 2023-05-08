namespace Planificateur.UI.ViewModels.Services;

public interface INavigationService
{
    Task NavigateToAsync(string page);
}