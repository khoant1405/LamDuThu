namespace JSN.Shared.Logging;

public class LogThreadConsumerObject : LogObject
{
    public int? Partition { get; set; }

    #region Constructor

    public LogThreadConsumerObject()
    {
    }

    public LogThreadConsumerObject(string serviceType) : base(serviceType)
    {
    }

    public LogThreadConsumerObject(string serviceType, string action) : base(serviceType, action)
    {
    }

    #endregion
}
