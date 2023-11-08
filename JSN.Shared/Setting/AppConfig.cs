using JSN.Shared.Model;
using JSN.Shared.Utilities;
using Microsoft.Extensions.Configuration;

namespace JSN.Shared.Setting;

public static class AppConfig
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

    // Define default values for settings
    public static int PublishAfterMinutes { get; set; } = 1;
    public static int NumberPublish { get; set; } = 1;
    public static int ArticlePageSize { get; set; } = 20;
    public static JwtConfig JwtSetting { get; set; }
    public static List<SqlConfig> SqlSettings { get; set; }
    public static SqlConfig? DefaultSqlSetting { get; set; }
    public static RedisConfig RedisSetting { get; set; }
    public static KafkaConfig KafkaSetting { get; set; }

    public static void LoadConfig()
    {
        JwtSetting = LoadJwtSetting();
        SqlSettings = LoadSqlSettings();
        DefaultSqlSetting = SqlSettings.FirstOrDefault();
        RedisSetting = LoadRedisSetting();
        ArticlePageSize = ConvertHelper.ToInt32(ConfigurationBuilder["ArticlePageSize"], 20);
        PublishAfterMinutes = ConvertHelper.ToInt32(ConfigurationBuilder["PublishAfterMinutes"], 1);
        NumberPublish = ConvertHelper.ToInt32(ConfigurationBuilder["NumberPublish"], 1);
        KafkaSetting = LoadKafkaSetting();
    }

    private static JwtConfig LoadJwtSetting()
    {
        return new JwtConfig
        {
            ValidAudience = ConvertHelper.ToString(ConfigurationBuilder["JWT:ValidAudience"]),
            ValidIssuer = ConvertHelper.ToString(ConfigurationBuilder["JWT:ValidIssuer"]),
            Token = ConvertHelper.ToString(ConfigurationBuilder["JWT:Token"]),
            TokenValidityInMinutes = ConvertHelper.ToInt32(ConfigurationBuilder["JWT:TokenValidityInMinutes"]),
            RefreshTokenValidityInDays = ConvertHelper.ToInt32(ConfigurationBuilder["JWT:RefreshTokenValidityInDays"])
        };
    }

    private static List<SqlConfig> LoadSqlSettings()
    {
        var sqlSettings = new List<SqlConfig>();
        var index = 0;

        while (true)
        {
            var sqlName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:Name").Value);

            if (string.IsNullOrEmpty(sqlName))
            {
                break;
            }

            sqlSettings.Add(new SqlConfig
            {
                Name = sqlName,
                ConnectString =
                    ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:ConnectString").Value)
            });

            index++;
        }

        return sqlSettings;
    }

    private static RedisConfig LoadRedisSetting()
    {
        return new RedisConfig
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
    }

    private static KafkaConfig LoadKafkaSetting()
    {
        var kafkaSetting = new KafkaConfig
        {
            KafkaIp = ConvertHelper.ToString(ConfigurationBuilder["Kafka:KafkaIp"]),
            GroupId = ConvertHelper.ToString(ConfigurationBuilder["Kafka:GroupId"]),
            ClientId = ConvertHelper.ToString(ConfigurationBuilder["Kafka:ClientId"]),
            CommitPeriod = ConvertHelper.ToInt32(ConfigurationBuilder["Kafka:CommitPeriod"]),
            ConsumerIsClosedWhenConsumeException =
                ConvertHelper.ToBoolean(ConfigurationBuilder["Kafka:ConsumerIsClosedWhenConsumeException"]),
            PartitionSize = ConvertHelper.ToInt32(ConfigurationBuilder["Kafka:PartitionSize"])
        };

        var producerSettings = new List<Producer>();
        var index = 0;

        while (true)
        {
            var producerName =
                ConvertHelper.ToString(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Name").Value);

            if (string.IsNullOrEmpty(producerName))
            {
                break;
            }

            producerSettings.Add(new Producer
            {
                Name = producerName,
                QueueName = ConvertHelper.ToString(ConfigurationBuilder
                    .GetSection($"Kafka:AllProducers:{index}:QueueName").Value),
                Size = ConvertHelper.ToInt32(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Size").Value)
            });

            index++;
        }

        kafkaSetting.AllProducers = producerSettings;
        return kafkaSetting;
    }
}