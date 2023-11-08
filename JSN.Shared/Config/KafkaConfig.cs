using JSN.Shared.Model;

namespace JSN.Shared.Config;

public class KafkaConfig
{
    public string? KafkaIp { get; set; }
    public string? GroupId { get; set; }
    public string? ClientId { get; set; }
    public int CommitPeriod { get; set; }
    public bool ConsumerIsClosedWhenConsumeException { get; set; }
    public int PartitionSize { get; set; }
    public List<Producer>? AllProducers { get; set; }
    public bool IsKafkaMonitor { get; set; }
    public string? KafkaPrefix { get; set; }
    public short Replica { get; set; }
}
