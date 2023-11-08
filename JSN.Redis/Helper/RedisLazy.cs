using JSN.Shared.Config;
using StackExchange.Redis;

namespace JSN.Redis.Helper;

public class RedisLazy
{
    private static readonly Lazy<ConnectionMultiplexer> LazyConnection = new(CreateConnection);

    public IDatabase GetLazyDatabase()
    {
        return LazyConnection.Value.GetDatabase();
    }

    private static ConnectionMultiplexer CreateConnection()
    {
        var config = AppConfig.RedisConfig;
        if (config.IsSentinel != true)
        {
            return ConnectionMultiplexer.Connect(RedisHelper.GetConfigRedis());
        }

        var sentinelOptions = new ConfigurationOptions { TieBreaker = "", CommandMap = CommandMap.Sentinel, AbortOnConnectFail = false };
        var configRedis = RedisHelper.GetConfigRedis();
        foreach (var item in configRedis.EndPoints)
        {
            sentinelOptions.EndPoints.Add(item);
        }

        configRedis.EndPoints.Clear();
        var sentinelConnection = ConnectionMultiplexer.Connect(sentinelOptions);
        return sentinelConnection.GetSentinelMasterConnection(configRedis);
    }
}