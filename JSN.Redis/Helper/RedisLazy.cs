using JSN.Shared.Setting;
using StackExchange.Redis;

namespace JSN.Redis.Helper;

public class RedisLazy
{
    private static Lazy<ConnectionMultiplexer> _lazyConnection = new(CreateConnection);

    public IDatabase GetLazyDatabase()
    {
        return _lazyConnection.Value.GetDatabase();
    }

    private static ConnectionMultiplexer CreateConnection()
    {
        var config = AppConfig.RedisSetting;
        if (config.IsSentinel != true)
        {
            return ConnectionMultiplexer.Connect(RedisHelper.GetConfigRedis());
        }

        var sentinelOptions = new ConfigurationOptions
        {
            TieBreaker = "",
            CommandMap = CommandMap.Sentinel,
            AbortOnConnectFail = false
        };
        var configRedis = RedisHelper.GetConfigRedis();
        foreach (var item in configRedis.EndPoints) sentinelOptions.EndPoints.Add(item);
        configRedis.EndPoints.Clear();
        var sentinelConnection = ConnectionMultiplexer.Connect(sentinelOptions);
        return sentinelConnection.GetSentinelMasterConnection(configRedis);
    }
}