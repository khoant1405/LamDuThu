using JSN.Service.Interface;
using JSN.Shared.Config;

namespace JSN.KafkaConsumer;

public class Worker(ILogger<Worker> logger, IArticleService articleService) : BackgroundService
{
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

            logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            var result = await articleService.PublishArticleAsync();

            if (result)
            {
                logger.LogInformation("Publish success at: {time}", DateTimeOffset.Now);
            }
            else
            {
                logger.LogInformation("Publish fail at: {time}", DateTimeOffset.Now);
            }

            nextRunTime = DateTimeOffset.Now.AddMinutes(minutes);
        }
    }
}