WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddMvc();
WebApplication app = builder.Build();

app.MapControllers();
app.MapGet("/", () => "Hello World!");

app.Run();