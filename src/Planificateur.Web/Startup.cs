using Microsoft.EntityFrameworkCore;
using Npgsql;
using Planificateur.Core;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database;

namespace Planificateur.Web;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddMvc();
        services.AddScoped<IPollsRepository, PollsRepository>();
        services.AddScoped<IVotesRepository, VotesRepository>();
        services.AddScoped<PollApplication>();
        services.AddNpgsql<ApplicationDbContext>(
            new NpgsqlConnectionStringBuilder
            {
                Host = Configuration["DB_HOST"],
                Port = int.Parse(Configuration["DB_PORT"]),
                Username = Configuration["DB_USERNAME"],
                Password = Configuration["DB_PASSWORD"],
                Database = Configuration["DB_NAME"]
            }.ToString());
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
    {
        context.Database.Migrate();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}