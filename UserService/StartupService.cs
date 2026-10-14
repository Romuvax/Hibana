using StackExchange.Redis;

namespace UserService;

public class StartupService : IHostedService
{
    private readonly ILogger<StartupService> _logger;
    private readonly IConnectionMultiplexer _redis;

    public StartupService(ILogger<StartupService> logger, IConnectionMultiplexer redis)
    {
        _logger = logger; 
        _redis = redis;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        IDatabase db = _redis.GetDatabase();

        var testvalue = await db.StringGetAsync("hello");
        if (testvalue.IsNullOrEmpty)
        {
            await db.StringSetAsync("hello", "world");
        }
        
        var value = await db.StringGetAsync("hello");
        _logger.LogInformation("test value retrieved {}", value);
        _logger.LogInformation("Startup service initialized.");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Startup service stopping.");
    }
}