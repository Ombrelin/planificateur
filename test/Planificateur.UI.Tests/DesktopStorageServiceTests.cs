using System.IO.IsolatedStorage;
using FluentAssertions;
using Planificateur.UI.Services;

namespace Planificateur.UI.Tests;

public class DesktopStorageServiceTests
{
    public DesktopStorageServiceTests()
    {
        IsolatedStorageFile store = IsolatedStorageFile.GetUserStoreForDomain();
        foreach (var fileName in store.GetFileNames())
        {
            store.DeleteFile(fileName);
        }
    }

    [Fact]
    public async Task PutGet_ReturnsValue()
    {
        // Given
        var service = new DesktopStorageService();

        // When
        await service.PutAsync("test key", "test value");

        // Then
        var result = await service.GetAsync("test key");
        result.Should().Be("test value");
    }

    [Fact]
    public async Task Put_NoFile_DontThrow()
    {
        // Given
        var service = new DesktopStorageService();

        // When
        var action = async () => await service.PutAsync("test key", "test value");

        // Then
        await action.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ContainsKey_DataPresent_ReturnsTrue()
    {
        // Given
        var service = new DesktopStorageService();
        await service.PutAsync("test key", "test value");

        // When
        bool result = await service.ContainsKeyAsync("test key");

        // THen
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ContainsKey_DataAbsent_ReturnsFalse()
    {
        // Given
        var service = new DesktopStorageService();

        // When
        bool result = await service.ContainsKeyAsync("test key");

        // THen
        result.Should().BeFalse();
    }
}