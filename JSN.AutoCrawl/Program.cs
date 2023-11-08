using JSN.AutoPublishArticle;
using JSN.AutoPublishArticle.Extensions;
using JSN.Core.AutoMapper;
using JSN.Kafka.Helper;
using JSN.Shared.Config;

var host = Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
{
    // Add your configuration sources here
    // Example: config.AddJsonFile("appsettings.json");
}).ConfigureServices((hostContext, services) =>
{
    services.AddRedis();
    services.AddDatabase();
    services.AddRepositories();
    services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
    services.AddServices();
    services.AddHostedService<Worker>();
}).Build();

// Now, set AppConfigs.ConfigurationBuilder
AppConfig.ConfigurationBuilder = host.Services.GetRequiredService<IConfiguration>();

KafkaHelper.Instance.SetKafkaConfig();
KafkaHelper.Instance.InitProducer();
await KafkaHelper.Instance.SetTopic("PublishArticle", 1);

await host.RunAsync();