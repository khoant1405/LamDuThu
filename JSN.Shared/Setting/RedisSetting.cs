namespace JSN.Shared.Setting;

public class RedisSetting
{
    public string? Servers { get; set; }

    public string? SentinelMasterName { get; set; }

    public int? DbNumber { get; set; }

    public string? AuthPass { get; set; }

    public bool? IsSentinel { get; set; }

    public bool IsUseRedisLazy { get; set; }

    public int ConnectTimeout { get; set; }

    public int ConnectRetry { get; set; }
}