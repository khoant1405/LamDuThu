namespace JSN.Shared.Setting
{
    public class RedisSetting
    {
        public string Servers { get; set; }

        public string SentinelMasterName { get; set; }

        public int? DbNumber { get; set; }

        public string AuthPass { get; set; }

        public bool? IsSentinel { get; set; }

        public string ClientName { get; set; }

        public int? MaxPoolSize { get; set; }

        public int? MaxPoolTimeout { get; set; }

        public int? ConnectTimeout { get; set; }

        public int? RetryTimeout { get; set; }

        public int? WaitBeforeForcingMasterFailover { get; set; }

        public int? SentinelWorkerConnectTimeoutMs { get; set; }

        public int? SentinelWorkerReceiveTimeoutMs { get; set; }

        public int? SentinelWorkerSendTimeoutMs { get; set; }

        public string ServersRead { get; set; }
    }
}
