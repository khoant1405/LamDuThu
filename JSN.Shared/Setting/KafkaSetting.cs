namespace JSN.Shared.Setting;

public class KafkaSetting
{
    public string? KafkaIp { get; set; }
    public string? GroupId { get; set; }
    public string? ClientId { get; set; }
    public int CommitPeriod { get; set; }
    public bool ConsumerIsClosedWhenConsumeException { get; set; }
    public int PartitionSize { get; set; }
    public List<ProducerSetting> AllProducers { get; set; }
}