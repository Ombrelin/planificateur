using Planificateur.Web.Database;
using Planificateur.Web.Database.Repositories;

namespace Planificateur.Web.Tests.Database;

public class DatabaseTests: IAsyncLifetime
{

    protected ApplicationDbContext DbContext = null!;
    private readonly DatabaseFixture databaseFixture;
    private readonly TestDatabaseContextFactory databaseContextFactory = new();

    public DatabaseTests(DatabaseFixture databaseFixture)
    {
        this.databaseFixture = databaseFixture;
    }
    
    public virtual async Task InitializeAsync()
    {
        DbContext = await databaseContextFactory.BuildNewDbContext(databaseFixture.Database.GetConnectionString());
    }

    public Task DisposeAsync() => Task.CompletedTask;
}