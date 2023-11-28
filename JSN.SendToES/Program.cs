using JSN.Core.AutoMapper;
using JSN.Kafka.Helper;
using JSN.SendToES.Extensions;
using JSN.Shared.Config;

namespace JSN.SendToES;

public class Program
{
    [Obsolete]
    public static void Main(string[] args)
    {
        var host = CreateHostBuilder(args).Build();
        AppConfig.ConfigurationBuilder = host.Services.GetRequiredService<IConfiguration>();
        KafkaHelper.Instance.SetKafkaConfig();

        var topics = new List<string>
        {
            "PublishArticleX-develop",
            "PublishArticleY-develop"
        };

        using var consumer = new KafkaMessageConsumer();
        consumer.Subscribe(topics);

        consumer.StartConsuming(message =>
        {
            Console.WriteLine($"Received message: {message.Value} on topic {message.Topic}, partition {message.Partition}, offset {message.Offset}");
            // Process the received message here
        });

        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((_, config) =>
        {
            // Add your configuration sources here
            // Example: config.AddJsonFile("appsettings.json");
        }).ConfigureServices((_, services) =>
        {
            services.AddRedis();
            services.AddDatabase();
            services.AddRepositories();
            services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
            services.AddServices();
        });
    }
}