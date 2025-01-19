using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Web.Database;
using Testcontainers.PostgreSql;

namespace Planificateur.Web.Tests.Database;

public class TestDatabaseContextFactory
{
    public async Task<ApplicationDbContext> BuildNewDbContext(string connectionString)
    {
        var builder = new NpgsqlConnectionStringBuilder
        {
            ConnectionString = connectionString,
            IncludeErrorDetail = true
        };
        DbContextOptions<ApplicationDbContext> options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(builder.ToString())
            .Options;

        var context = new ApplicationDbContext(options);
        await context.Database.MigrateAsync();
        return context;
    }
}