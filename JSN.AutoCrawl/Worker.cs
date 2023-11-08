using JSN.Shared.Config;

namespace JSN.AutoPublishArticle;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await DoSomethingAfterMinutesAsync(stoppingToken, AppConfig.PublishAfterMinutes);
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
}
