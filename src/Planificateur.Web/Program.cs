
using Planificateur.Web;

Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) => { config.AddEnvironmentVariables(); })
    .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>())
    .Build()
    .Run();