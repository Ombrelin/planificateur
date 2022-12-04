using Planificateur.Core;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database;

namespace Planificateur.Web;

public class Program
{
    public static Task Main(string[] args)
    {
        var database = new InMemoryDatabase();
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddMvc();
        builder.Services.AddSingleton<IPollsRepository>(database);
        builder.Services.AddSingleton<IVotesRepository>(database);
        builder.Services.AddSingleton<PollApplication>();
        WebApplication app = builder.Build();

        app.UseStaticFiles();
        app.MapControllers();
        app.MapGet("/", () => "Hello World!");

        return app.RunAsync();
    }
}

