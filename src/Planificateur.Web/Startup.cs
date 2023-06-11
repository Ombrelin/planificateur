using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using Planificateur.Core;
using Planificateur.Core.Repositories;
using Planificateur.Web.Database;
using Planificateur.Web.Database.Repositories;
using Planificateur.Web.Json;
using Planificateur.Web.Middlewares;

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
            .AddControllers(options => { options.Filters.Add(typeof(ExceptionMiddleware)); })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.AddContext<SourceGenerationSerialiser>();
            });
        services.AddMvc();
        services.AddScoped<IPollsRepository, PollsRepository>();
        services.AddScoped<IVotesRepository, VotesRepository>();
        services.AddScoped<IApplicationUsersRepository, ApplicationUsersRepository>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<PollApplication>(services =>
        {
            HttpContext httpContext = (services
                    .GetService<IHttpContextAccessor>()  ?? throw new InvalidOperationException("Not http context accessor when configuring access control manager"))
                .HttpContext ?? throw new InvalidOperationException("Not http context when configuring access control manager");

            string? currentUserIdString = httpContext.User.Claims.FirstOrDefault(claim => claim.Type is ClaimTypes.NameIdentifier)?.Value;
            if (currentUserIdString is null)
            {
                return new PollApplication(
                    services.GetService<IPollsRepository>()!,
                    services.GetService<IVotesRepository>()!
                );
            }

            return new PollApplication(
                services.GetService<IPollsRepository>()!,
                services.GetService<IVotesRepository>()!,
                Guid.Parse(currentUserIdString)
            );
        });
        services.AddScoped<AuthenticationApplication>(services => new AuthenticationApplication(
            services.GetService<IApplicationUsersRepository>(),
            Configuration["JWT_SECRET"]
        ));
        services.AddNpgsql<ApplicationDbContext>(
            new NpgsqlConnectionStringBuilder
            {
                Host = Configuration["DB_HOST"],
                Port = int.Parse(Configuration["DB_PORT"] ?? string.Empty),
                Username = Configuration["DB_USERNAME"],
                Password = Configuration["DB_PASSWORD"],
                Database = Configuration["DB_NAME"]
            }.ToString());
        ConfigureAuthentication(services);
        ConfigureSwaggerGen(services);
        ConfigureLogging(services);
    }

    private void ConfigureAuthentication(IServiceCollection services)
    {
        string jwtSecret = Configuration["JWT_SECRET"] ??
                           throw new ArgumentException("JWT_SECRET Env variable is not set");
        byte[] key = Encoding.ASCII.GetBytes(jwtSecret);

        services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(auth =>
            {
                auth.SaveToken = true;
                auth.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
    }

    private static void ConfigureLogging(IServiceCollection services)
    {
        services.AddLogging(options =>
        {
            options.ClearProviders();
            options.AddConsole();
            options.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.None);
        });
        services.AddW3CLogging(logging => { logging.LoggingFields = W3CLoggingFields.All; });
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
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                Description = "JWT Bearer Authorization",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        },
                        Scheme = "oauth2",
                        Name = "Bearer",
                        In = ParameterLocation.Header
                    },
                    new List<string>()
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
        app.UseW3CLogging();
        app.UseRouting();
        app.UseCors(cors =>
        {
            cors.AllowAnyOrigin();
            cors.AllowAnyMethod();
            cors.AllowAnyHeader();
        });
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseEndpoints(endpoints => endpoints.MapControllers());
    }
}