namespace JSN.Shared.Logging;

public class LogImpactObject : LogObject
{
    public string ImpactType { get; set; }

    public DateTime? ImpactStartedAt { get; set; }
    public DateTime? ImpactFinishedAt { get; set; }

    public double? ImpactRabbitTime { get; set; }
    public double? ImpactTransferTime { get; set; }
    public double? ImpactKafkaPreSyncTime { get; set; }
    public double? ImpactPreProcessTime { get; set; }
    public double? TimeMeasureGetData { get; set; }
    public double? TimeMeasureGetListSend { get; set; }
    public double? ImpactTotalTimeMeasure { get; set; }
    public double? ImpactKafkaSyncTime { get; set; }

    public double? ImpactTotalDuration { get; set; }

    #region Constructor

    public LogImpactObject()
    {
    }

    public LogImpactObject(string serviceType) : base(serviceType)
    {
    }

    public LogImpactObject(string serviceType, string action) : base(serviceType, action)
    {
    }

    #endregion
}
