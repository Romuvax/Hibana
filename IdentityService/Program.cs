using Npgsql;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);
var redisConnectionString = builder.Configuration["Redis:ConnectionString"]
    ?? throw new InvalidOperationException(
        "Set Redis:ConnectionString (environment variable Redis__ConnectionString).");
var connString = builder.Configuration["Database:ConnectionString"];
// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.WebHost.UseUrls("http://localhost:5001");
builder.Services.AddOpenApi();
builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(redisConnectionString));
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}



app.MapGet("/", () => "Hello World!");

app.MapGet("/health", () => "OK");

app.MapPost("/link/", async () =>
{
    await using (var conn = new NpgsqlConnection())
    {
        await conn.OpenAsync();
        await using (var trans = conn.BeginTransaction())
        {
            await trans.CommitAsync();
        }
        await conn.CloseAsync();
    }
});

app.Run();