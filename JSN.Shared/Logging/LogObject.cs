using Newtonsoft.Json;

namespace JSN.Shared.Logging;

public class LogObject()
{
    public LogObject(string serviceType) : this()
    {
        ServiceType = serviceType;
    }

    public LogObject(string serviceType, string action) : this(serviceType)
    {
        Action = action;
    }

    #region Trace info

    [JsonIgnore] public string ServiceType { get; set; }

    public string Action { get; set; }
    public string? Description { get; set; }
    public string DescriptionOptional { get; set; }
    public object? ObjJson { get; set; }

    [JsonIgnore] public string ClientName { get; set; }

    #endregion

    #region Time

    public DateTime StartTime { get; set; } = DateTime.Now;
    public double? TotalDuration { get; set; }

    #endregion
}