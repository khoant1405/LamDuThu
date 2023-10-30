using JSN.Service.Interface;

namespace JSN.AutoCrawl;

public class Worker : BackgroundService
{
    private readonly ICrawlerService _crawlerService;
    private readonly ILogger<Worker> _logger;

    public Worker(ICrawlerService crawlerService, ILogger<Worker> logger)
    {
        _logger = logger;
        _crawlerService = crawlerService;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoSomeThingAt12amAsync(stoppingToken);
    }

    private async Task DoSomethingAfterMinutesAsync(CancellationToken stoppingToken, int minutes)
    {
        var nextRunTime = DateTimeOffset.Now.AddMinutes(minutes);

        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = nextRunTime - DateTimeOffset.Now;
            if (delay > TimeSpan.Zero) await Task.Delay(delay, stoppingToken);

            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            //await _crawlerService.CrawlAsync();

            nextRunTime = DateTimeOffset.Now.AddMinutes(minutes);
        }
    }

    private async Task DoSomeThingAt12amAsync(CancellationToken stoppingToken)
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
        if (now >= nextRunTime) nextRunTime = nextRunTime.AddDays(1);

        return nextRunTime;
    }
}