using JSN.Shared.Setting;
using StackExchange.Redis;

namespace JSN.Redis.Helper;

public class RedisLazy
{
    private readonly Lazy<ConnectionMultiplexer> _connection;

    public RedisLazy()
    {
        var config = AppSettings.RedisSetting;
        _connection = new Lazy<ConnectionMultiplexer>(() =>
        {
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
        });
    }

    public ConnectionMultiplexer Connection => _connection.Value;
}