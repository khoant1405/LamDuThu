namespace JSN.Shared.Config;

public class KafkaProducerConfig
{
    public int BatchNumMessages { get; set; }
    public double LingerMs { get; set; }
    public int MessageSendMaxRetries { get; set; }
    public int MessageTimeoutMs { get; set; }
    public int RequestTimeoutMs { get; set; }
    public bool IsUseProduceAsync { get; set; }
}
