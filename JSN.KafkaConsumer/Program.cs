using JSN.Core.AutoMapper;
using JSN.Kafka.Helper;
using JSN.KafkaConsumer.Extensions;
using JSN.Shared.Config;

namespace JSN.KafkaConsumer;

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
}