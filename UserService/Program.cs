using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var redisConnectionString = builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Set Redis:ConnectionString (environment variable Redis__ConnectionString).");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.WebHost.UseUrls("http://localhost:5000");
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddHostedService<UserService.StartupService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapGet("/", () => "Hello World!");
app.MapGet("/health", () => "OK");

app.Run();