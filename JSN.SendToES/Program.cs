using Confluent.Kafka;
using JSN.Core.AutoMapper;
using JSN.Kafka.Helper;
using JSN.SendToES.Extensions;
using JSN.Shared.Config;

namespace JSN.SendToES;

public class Program
{
    public static void Main(string[] args)
    {
        // Create the host
        var host = CreateHostBuilder(args).Build();

        // Now, set AppConfigs.ConfigurationBuilder
        AppConfig.ConfigurationBuilder = host.Services.GetRequiredService<IConfiguration>();

        // Set Kafka configuration
        KafkaHelper.Instance.SetKafkaConfig();

        var listThreadConsumer = new List<ThreadConsumerInfo>();
        var consumerBuilder = new ConsumerBuilder<Ignore, string>(KafkaHelper.Instance.GetKafkaConsumerConfig());

        var topics = new List<string>
        {
            "PublishArticleX-develop",
            "PublishArticleY-develop"
        };

        using (var consumer = consumerBuilder.Build())
        {
            consumer.Subscribe(topics);

            try
            {
                while (true)
                {
                    try
                    {
                        var message = consumer.Consume();
                        Console.WriteLine($"Received message: {message.Value} on topic {message.Topic}, partition {message.Partition}, offset {message.Offset}");
                        // Process the received message here
                    }
                    catch (ConsumeException e)
                    {
                        Console.WriteLine($"Error occurred: {e.Error.Reason}");
                    }
                }
            }
            catch (ConsumeException)
            {
                consumer.Close();
            }
        }

        // Start the host
        host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args).ConfigureAppConfiguration((hostingContext, config) =>
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
        });
    }

    public class ThreadConsumerInfo
    {
        public int CategoryId { get; set; }
        public int Partition { get; set; }
        public bool IsRunning { get; set; }
    }
}