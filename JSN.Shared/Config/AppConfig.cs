using JSN.Shared.Model;
using JSN.Shared.Utilities;
using Microsoft.Extensions.Configuration;

namespace JSN.Shared.Config;

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
    public static JwtConfig JwtConfig { get; set; }
    public static List<SqlConfig> SqlConfigs { get; set; }
    public static SqlConfig DefaultSqlConfig { get; set; }
    public static RedisConfig RedisConfig { get; set; }
    public static KafkaConfig KafkaConfig { get; set; }
    public static KafkaProducerConfig KafkaProducerConfig { get; set; }
    public static List<string>? LogExceptionIgnoreObjJsonByServiceType { get; set; }

    public static void LoadConfig()
    {
        JwtConfig = LoadJwtConfig();
        SqlConfigs = LoadSqlConfigs();
        DefaultSqlConfig = SqlConfigs.FirstOrDefault();

        RedisConfig = LoadRedisConfig();
        ArticlePageSize = ConvertHelper.ToInt32(ConfigurationBuilder["ArticlePageSize"], 20);
        PublishAfterMinutes = ConvertHelper.ToInt32(ConfigurationBuilder["PublishAfterMinutes"], 1);
        NumberPublish = ConvertHelper.ToInt32(ConfigurationBuilder["NumberPublish"], 1);
        KafkaConfig = LoadKafkaConfig();
        KafkaProducerConfig = LoadKafkaProducerConfig();
        LogExceptionIgnoreObjJsonByServiceType = GetListStringBySplitChar(ConfigurationBuilder["LogExceptionIgnoreObjJsonByServiceType"]);
    }

    private static JwtConfig LoadJwtConfig()
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

    private static List<SqlConfig> LoadSqlConfigs()
    {
        var sqlConfigs = new List<SqlConfig>();
        var index = 0;

        while (true)
        {
            var sqlName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:Name").Value);

            if (string.IsNullOrEmpty(sqlName))
            {
                break;
            }

            sqlConfigs.Add(new SqlConfig
            {
                Name = sqlName,
                ConnectString = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"SQL:{index}:ConnectString").Value)
            });

            index++;
        }

        return sqlConfigs;
    }

    private static RedisConfig LoadRedisConfig()
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

    private static KafkaConfig LoadKafkaConfig()
    {
        var kafkaConfig = new KafkaConfig
        {
            KafkaIp = ConvertHelper.ToString(ConfigurationBuilder["Kafka:KafkaIp"]),
            GroupId = ConvertHelper.ToString(ConfigurationBuilder["Kafka:GroupId"]),
            ClientId = ConvertHelper.ToString(ConfigurationBuilder["Kafka:ClientId"]),
            CommitPeriod = ConvertHelper.ToInt32(ConfigurationBuilder["Kafka:CommitPeriod"]),
            ConsumerIsClosedWhenConsumeException = ConvertHelper.ToBoolean(ConfigurationBuilder["Kafka:ConsumerIsClosedWhenConsumeException"]),
            PartitionSize = ConvertHelper.ToInt32(ConfigurationBuilder["Kafka:PartitionSize"]),
            IsKafkaMonitor = ConvertHelper.ToBoolean(ConfigurationBuilder["Kafka:IsKafkaMonitor"]),
            KafkaPrefix = ConvertHelper.ToString(ConfigurationBuilder["Kafka:KafkaPrefix"]),
            Replica = (short)ConvertHelper.ToInt32(ConfigurationBuilder["KafkaProducer:NumberReplica"])
        };
        kafkaConfig.Replica = kafkaConfig.Replica == 0 ? (short)1 : kafkaConfig.Replica;

        var producerConfigs = new List<Producer>();
        var index = 0;

        while (true)
        {
            var producerName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Name").Value);

            if (string.IsNullOrEmpty(producerName))
            {
                break;
            }

            producerConfigs.Add(new Producer
            {
                Name = producerName,
                QueueName = ConvertHelper.ToString(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:QueueName").Value),
                Size = ConvertHelper.ToInt32(ConfigurationBuilder.GetSection($"Kafka:AllProducers:{index}:Size").Value)
            });

            index++;
        }

        kafkaConfig.AllProducers = producerConfigs;
        return kafkaConfig;
    }

    private static KafkaProducerConfig LoadKafkaProducerConfig()
    {
        return new KafkaProducerConfig
        {
            BatchNumMessages = ConvertHelper.ToInt32(ConfigurationBuilder["KafkaProducer:BatchNumMessages"]),
            LingerMs = ConvertHelper.ToDouble(ConfigurationBuilder["KafkaProducer:LingerMs"]),
            MessageSendMaxRetries = ConvertHelper.ToInt32(ConfigurationBuilder["KafkaProducer:MessageSendMaxRetries"]),
            MessageTimeoutMs = ConvertHelper.ToInt32(ConfigurationBuilder["KafkaProducer:MessageTimeoutMs"]),
            RequestTimeoutMs = ConvertHelper.ToInt32(ConfigurationBuilder["KafkaProducer:RequestTimeoutMs"]),
            IsUseProduceAsync = ConvertHelper.ToBoolean(ConfigurationBuilder["KafkaProducer:IsUseProduceAsync"])
        };
    }

    private static List<string>? GetListStringBySplitChar(string? configValue, List<string>? defaultValue = null)
    {
        if (configValue == null)
        {
            return defaultValue ?? new List<string>();
        }

        return ConvertHelper.ToString(configValue)?.Trim().Split(",", StringSplitOptions.RemoveEmptyEntries).ToList();
    }
}