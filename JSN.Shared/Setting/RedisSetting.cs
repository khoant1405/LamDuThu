﻿namespace JSN.Shared.Setting;

public class RedisSetting
{
    public string Servers { get; set; }

    public string SentinelMasterName { get; set; }

    public int? DbNumber { get; set; }

    public string AuthPass { get; set; }

    public bool? IsSentinel { get; set; }

    public int? WaitBeforeForcingMasterFailover { get; set; }

    public int? MaxPoolSize { get; set; }

    public string ClientName { get; set; }

    public bool? IsUseRedisLazy { get; set; }
}