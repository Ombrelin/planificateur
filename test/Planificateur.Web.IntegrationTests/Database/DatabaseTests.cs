using Planificateur.Web.Database;

namespace Planificateur.Web.Tests.Database;

public class DatabaseTests : IAsyncLifetime
{
    protected readonly ApplicationDbContext dbContext;

    public DatabaseTests(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => Task.CompletedTask;
}