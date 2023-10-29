using JSN.AutoCrawl;
using JSN.AutoCrawl.Extensions;
using JSN.Shared.Setting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((hostingContext, config) =>
    {
        // Add your configuration sources here
        // Example: config.AddJsonFile("appsettings.json");
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.AddRedis();
        services.AddDatabase();
        services.AddRepositories();
        services.AddServices();
        services.AddHostedService<Worker>();
    })
    .Build();

// Now, set AppSettings.ConfigurationBuilder
AppSettings.ConfigurationBuilder = host.Services.GetRequiredService<IConfiguration>();

await host.RunAsync();