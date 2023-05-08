using System.Threading.Tasks;
using Microsoft.Maui.Storage;
using Planificateur.UI.ViewModels.Services;

namespace Planificateur.UI.Services;

public class MobileStorageService : IStorageService
{
    public Task PutAsync(string key, string value)
    {
        Preferences.Default.Set(key, value);
        return Task.CompletedTask;
    }

    public Task<string> GetAsync(string key) => Task.FromResult(Preferences.Default.Get(key, string.Empty));
    public Task<bool> ContainsKeyAsync(string key) => Task.FromResult(Preferences.Default.ContainsKey(key));

    public Task RemoveAsync(string key)
    {
        Preferences.Default.Remove(key);
        return Task.CompletedTask;
    }
}