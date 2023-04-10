using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using Planificateur.Core;
using Planificateur.Core.Entities;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database;
using Planificateur.Web.Json;

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
        services
            .AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.AddContext<SourceGenerationSerialiser>();
            });
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
        ConfigureSwaggerGen(services);
    }

    private static void ConfigureSwaggerGen(IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "Planificateur API",
                Description = "WebApp to manage date Polls",
                License = new OpenApiLicense
                {
                    Name = "MIT",
                    Url = new Uri("https://github.com/Ombrelin/planificateur/blob/master/LICENSE.md")
                },
                Contact = new OpenApiContact
                {
                    Name = "Arsène Lapostolet",
                    Url = new Uri("https://github.com/Ombrelin")
                }
            });
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Planificateur.Web.xml"));
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Planificateur.Core.xml"));
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ApplicationDbContext context)
    {
        context.Database.Migrate();
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        app.UseRequestLocalization();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = "api";
        });
        
        app.UseStaticFiles();
        app.UseRouting();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}