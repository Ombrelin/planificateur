using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Planificateur.UI.ViewModels.Services;

namespace Planificateur.UI.Services;

public class DesktopStorageService : IStorageService
{
    private static IsolatedStorageFile Store => IsolatedStorageFile.GetUserStoreForDomain();

    public async Task PutAsync(string key, string value)
    {
        await using IsolatedStorageFileStream stream = Store.OpenFile(key, FileMode.Create, FileAccess.Write);
        var writer = new StreamWriter(stream);
        await writer.WriteAsync(value);
        await writer.FlushAsync();
    }

    public async Task<string> GetAsync(string key)
    {
        await using IsolatedStorageFileStream stream = Store.OpenFile(key, FileMode.Open);
        var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public Task<bool> ContainsKeyAsync(string key) => Task.FromResult(Store.FileExists(key));

    public Task RemoveAsync(string key)
    {
        Store.DeleteFile(key);
        return Task.CompletedTask;
    }
}