using Planificateur.Core;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database;

namespace Planificateur.Web;

public class Program
{
    public static Task Main(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
        builder.Services.AddControllers();
        builder.Services.AddMvc();
        builder.Services.AddSingleton<IPollsRepository, InMemoryPollsRepository>();
        builder.Services.AddSingleton<IVotesRepository, InMemoryVotesRepository>();
        builder.Services.AddSingleton<PollApplication>();
        WebApplication app = builder.Build();

        app.MapControllers();
        app.MapGet("/", () => "Hello World!");

        return app.RunAsync();
    }
}

