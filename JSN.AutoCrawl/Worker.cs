namespace JSN.AutoCrawl;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoSomeThingAt12AmAsync(stoppingToken);
    }

    private async Task DoSomethingAfterMinutesAsync(CancellationToken stoppingToken, int minutes)
    {
        var nextRunTime = DateTimeOffset.Now.AddMinutes(minutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = nextRunTime - DateTimeOffset.Now;
            if (delay > TimeSpan.Zero)
            {
                await Task.Delay(delay, stoppingToken);
            }

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            //await _crawlerService.CrawlAsync();

            nextRunTime = DateTimeOffset.Now.AddMinutes(minutes);
        }
    }

    private async Task DoSomeThingAt12AmAsync(CancellationToken stoppingToken)
    {
        var nextRunTime = CalculateNextRunTime();

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = nextRunTime - DateTimeOffset.Now;
            await Task.Delay(delay, stoppingToken);

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            //await _crawlerService.CrawlAsync();

            nextRunTime = CalculateNextRunTime();
        }
    }

    private DateTime CalculateNextRunTime()
    {
        var now = DateTimeOffset.Now;
        var nextRunTime = now.Date.AddHours(24);
        if (now >= nextRunTime)
        {
            nextRunTime = nextRunTime.AddDays(1);
        }

        return nextRunTime;
    }
}