using JSN.Shared.Utilities;

namespace JSN.Shared.Logging;

public class LogRecoveryObject : LogObject
{
    public string DbName { get; set; }
    public int? TotalTask { get; set; }
    public string ListConnections { get; set; }
    public int? TotalExecute { get; set; }
    public int? TotalError { get; set; }
    public double? TimeMeasureGetData { get; set; }
    public double? TimeMeasureRecovery { get; set; }

    #region Implement

    public void SetTotalTime()
    {
        TotalDuration = DateTimeHelper.DifferenceBySeconds(StartTime, DateTime.Now);
    }

    #endregion

    #region Constructor

    public LogRecoveryObject()
    {
    }

    public LogRecoveryObject(string serviceType) : base(serviceType)
    {
    }

    public LogRecoveryObject(string serviceType, string action) : base(serviceType, action)
    {
    }

    #endregion
}
