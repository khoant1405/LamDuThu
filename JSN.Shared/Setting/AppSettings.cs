using JSN.Shared.Utilities;
using Microsoft.Extensions.Configuration;

namespace JSN.Shared.Setting;

public static class AppSettings
{
    private static IConfiguration _configurationBuilder;

    public static IConfiguration ConfigurationBuilder
    {
        get => _configurationBuilder;
        set
        {
            _configurationBuilder = value;
            LoadConfig();
        }
    }

    public static int PublishAfterMinutes { get; set; } = 1;
    public static int NumberPublish { get; set; } = 1;
    public static int ArticlePageSize { get; set; } = 20;
    public static JwtSetting JwtSetting { get; set; }
    public static List<SqlSetting> SqlSettings { get; set; }
    public static SqlSetting? DefaultSqlSetting { get; set; }
    public static RedisSetting RedisSetting { get; set; }
    public static KafkaSetting KafkaSetting { get; set; }

    public static void LoadConfig()
    {
        JwtSetting = new JwtSetting
        {
            ValidAudience = ConvertHelper.ToString(ConfigurationBuilder["JWT:ValidAudience"]),
            ValidIssuer = ConvertHelper.ToString(ConfigurationBuilder["JWT:ValidIssuer"]),
            Token = ConvertHelper.ToString(ConfigurationBuilder["JWT:Token"]),
            TokenValidityInMinutes = ConvertHelper.ToInt32(ConfigurationBuilder["JWT:TokenValidityInMinutes"]),
            RefreshTokenValidityInDays = ConvertHelper.ToInt32(ConfigurationBuilder["JWT:RefreshTokenValidityInDays"])
        };

        SqlSettings = new List<SqlSetting>();
        var index = 0;
        var sqlName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:Name")
            .Value);
        while (!string.IsNullOrEmpty(sqlName))
        {
            SqlSettings.Add(new SqlSetting
            {
                Name = sqlName,
                ConnectString = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:ConnectString")
                    .Value)
            });
            index++;
            sqlName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:Name")
                .Value);
        }

        DefaultSqlSetting = SqlSettings.FirstOrDefault();

        RedisSetting = new RedisSetting
        {
            Servers = ConvertHelper.ToString(ConfigurationBuilder["Redis:Servers"]),
            SentinelMasterName = ConvertHelper.ToString(ConfigurationBuilder["Redis:SentinelMasterName"]),
            DbNumber = ConvertHelper.ToInt32(ConfigurationBuilder["Redis:DbNumber"]),
            AuthPass = ConvertHelper.ToString(ConfigurationBuilder["Redis:AuthPass"]),
            IsSentinel = ConvertHelper.ToBoolean(ConfigurationBuilder["Redis:IsSentinel"]),
            IsUseRedisLazy = ConvertHelper.ToBoolean(ConfigurationBuilder["Redis:IsUseRedisLazy"]),
            ConnectTimeout = ConvertHelper.ToInt32(ConfigurationBuilder["Redis:MaxPoolSize"]),
            ConnectRetry = ConvertHelper.ToInt32(ConfigurationBuilder["Redis:MaxPoolSize"])
        };

        ArticlePageSize = ConvertHelper.ToInt32(ConfigurationBuilder["ArticlePageSize"], 20);

        PublishAfterMinutes = ConvertHelper.ToInt32(ConfigurationBuilder["PublishAfterMinutes"], 1);

        NumberPublish = ConvertHelper.ToInt32(ConfigurationBuilder["NumberPublish"], 1);

        KafkaSetting = new KafkaSetting
        {
            KafkaIp = ConvertHelper.ToString(ConfigurationBuilder["Kafka:KafkaIp"]),
            GroupId = ConvertHelper.ToString(ConfigurationBuilder["Kafka:GroupId"]),
            ClientId = ConvertHelper.ToString(ConfigurationBuilder["Kafka:ClientId"]),
            CommitPeriod = ConvertHelper.ToInt32(ConfigurationBuilder["Kafka:CommitPeriod"]),
            ConsumerIsClosedWhenConsumeException =
                ConvertHelper.ToBoolean(ConfigurationBuilder["Kafka:ConsumerIsClosedWhenConsumeException"]),
            PartitionSize = ConvertHelper.ToInt32(ConfigurationBuilder["Kafka:PartitionSize"])
        };
        var producerSettings = new List<ProducerSetting>();
        index = 0;
        var producerName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Name")
            .Value);
        while (!string.IsNullOrEmpty(producerName))
        {
            producerSettings.Add(new ProducerSetting
            {
                Name = producerName,
                QueueName = ConvertHelper.ToString(ConfigurationBuilder
                    .GetSection($"Kafka:AllProducers:{index}:QueueName")
                    .Value),
                Size = ConvertHelper.ToInt32(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Size")
                    .Value)
            });
            index++;
            producerName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Name")
                .Value);
        }

        KafkaSetting.AllProducers = producerSettings;
    }
}