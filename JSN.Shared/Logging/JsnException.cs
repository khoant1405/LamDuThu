using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using JSN.Shared.Config;
using JSN.Shared.Enum;
using JSN.Shared.Utilities;
using Serilog;
using Serilog.Context;

namespace JSN.Shared.Logging;

[Serializable]
public class JsnException : Exception
{
    #region Overload WriteException

    public static void WriteException(LogObject? logObject, Exception? ex, object? obj = null, string optional = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logObject ??= new LogObject(nameof(JsnException), $"{nameof(JsnException)}.{nameof(WriteException)}");
        if (obj != null)
        {
            var objJson = obj is string ? obj : ConvertHelper.ToJson(obj, true);
            logObject.ObjJson = objJson;
        }

        if (!string.IsNullOrEmpty(optional))
        {
            logObject.DescriptionOptional = optional;
        }

        #region ignore ObjJson

        if (!string.IsNullOrEmpty(logObject.ServiceType) && AppConfig.LogExceptionIgnoreObjJsonByServiceType != null && AppConfig.LogExceptionIgnoreObjJsonByServiceType.Any() &&
            AppConfig.LogExceptionIgnoreObjJsonByServiceType.Contains(logObject.ServiceType))
        {
            logObject.ObjJson = null;
        }

        #endregion

        var trace = $"{filePath}:{lineNumber} {memberName}";

        if (ex != null)
        {
            logObject.Description = GetInnerExceptionMessage(ex);

            using (LogContext.PushProperty("ClientName", logObject.ClientName))
            using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
            using (LogContext.PushProperty("ExceptionType", ex.GetType().Name))
            using (LogContext.PushProperty("IsJsnException", IsJsnException(ex)))
            using (LogContext.PushProperty("Source", ex.Source))
            using (LogContext.PushProperty("StackTrace", $"{ex.StackTrace}\n{trace}"))
            using (LogContext.PushProperty("HelpLink", ex.HelpLink))
            {
                Log.Logger.Error(ConvertHelper.ToJson(logObject, true));
            }
        }
        else
        {
            using (LogContext.PushProperty("ClientName", logObject.ClientName))
            using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
            using (LogContext.PushProperty("StackTrace", trace))
            {
                Log.Logger.Error(ConvertHelper.ToJson(logObject, true));
            }
        }
    }

    #endregion

    #region Constructor

    public JsnException(string msg) : base(msg)
    {
    }

    public JsnException(string msg, Exception e) : base(msg, e)
    {
    }

    protected JsnException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }

    #endregion

    #region Overload WriteInfo

    public static void WriteInfo(string msg, LogType type)
    {
        Log.Logger.Information($"{type}: {msg}");
    }

    public static void WriteInfo(LogObject? logObject, Exception? ex, object? obj = null, string optional = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logObject ??= new LogObject(nameof(JsnException), $"{nameof(JsnException)}.{nameof(WriteInfo)}");
        if (obj != null)
        {
            var objJson = obj is string ? obj : ConvertHelper.ToJson(obj, true);
            logObject.ObjJson = objJson;
        }

        if (!string.IsNullOrEmpty(optional))
        {
            logObject.DescriptionOptional = optional;
        }

        #region ignore ObjJson

        if (!string.IsNullOrEmpty(logObject.ServiceType) && AppConfig.LogExceptionIgnoreObjJsonByServiceType != null && AppConfig.LogExceptionIgnoreObjJsonByServiceType.Any() &&
            AppConfig.LogExceptionIgnoreObjJsonByServiceType.Contains(logObject.ServiceType))
        {
            logObject.ObjJson = null;
        }

        #endregion ignore ObjJson

        var trace = $"{filePath}:{lineNumber} {memberName}";

        if (ex != null)
        {
            logObject.Description = GetInnerExceptionMessage(ex);

            using (LogContext.PushProperty("ClientName", logObject.ClientName))
            using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
            using (LogContext.PushProperty("ExceptionType", ex.GetType().Name))
            using (LogContext.PushProperty("Source", ex.Source))
            using (LogContext.PushProperty("StackTrace", $"{ex.StackTrace}\n{trace}"))
            using (LogContext.PushProperty("HelpLink", ex.HelpLink))
            {
                Log.Logger.Information(ConvertHelper.ToJson(logObject, true));
            }
        }
        else
        {
            using (LogContext.PushProperty("ClientName", logObject.ClientName))
            using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
            using (LogContext.PushProperty("StackTrace", trace))
            {
                Log.Logger.Information(ConvertHelper.ToJson(logObject, true));
            }
        }
    }

    public static void WriteInfo(LogObject? logObject, string optional = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        logObject ??= new LogObject(nameof(JsnException), $"{nameof(JsnException)}.{nameof(WriteInfo)}");
        if (!string.IsNullOrEmpty(optional))
        {
            logObject.DescriptionOptional = optional;
        }

        #region ignore ObjJson

        if (!string.IsNullOrEmpty(logObject.ServiceType) && AppConfig.LogExceptionIgnoreObjJsonByServiceType != null && AppConfig.LogExceptionIgnoreObjJsonByServiceType.Any() &&
            AppConfig.LogExceptionIgnoreObjJsonByServiceType.Contains(logObject.ServiceType))
        {
            logObject.ObjJson = null;
        }

        #endregion ignore ObjJson

        var trace = $"{filePath}:{lineNumber} {memberName}";

        using (LogContext.PushProperty("ClientName", logObject.ClientName))
        using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
        using (LogContext.PushProperty("StackTrace", trace))
        {
            Log.Logger.Information(ConvertHelper.ToJson(logObject, true));
        }
    }

    public static void WriteLogTime(LogImpactObject? logObject, object? obj = null, string optional = "")
    {
        logObject ??= new LogImpactObject(nameof(JsnException), $"{nameof(JsnException)}.{nameof(WriteInfo)}");
        if (obj != null)
        {
            var objJson = obj is string ? obj : ConvertHelper.ToJson(obj, true);
            logObject.ObjJson = objJson;
        }

        if (!string.IsNullOrEmpty(optional))
        {
            logObject.DescriptionOptional = optional;
        }

        #region ignore ObjJson

        if (!string.IsNullOrEmpty(logObject.ServiceType) && AppConfig.LogExceptionIgnoreObjJsonByServiceType != null && AppConfig.LogExceptionIgnoreObjJsonByServiceType.Any() &&
            AppConfig.LogExceptionIgnoreObjJsonByServiceType.Contains(logObject.ServiceType))
        {
            logObject.ObjJson = null;
        }

        #endregion ignore ObjJson

        using (LogContext.PushProperty("ClientName", logObject.ClientName))
        using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
        using (LogContext.PushProperty("Action", logObject.Action))
        using (LogContext.PushProperty("ImpactType", logObject.ImpactType))
        using (LogContext.PushProperty("ImpactStartedAt", logObject.ImpactStartedAt))
        using (LogContext.PushProperty("ImpactFinishedAt", logObject.ImpactFinishedAt))
        using (LogContext.PushProperty("ImpactRabbitTime", logObject.ImpactRabbitTime))
        using (LogContext.PushProperty("ImpactTransferTime", logObject.ImpactTransferTime))
        using (LogContext.PushProperty("ImpactKafkaPreSyncTime", logObject.ImpactKafkaPreSyncTime))
        using (LogContext.PushProperty("ImpactPreProcessTime", logObject.ImpactPreProcessTime))
        using (LogContext.PushProperty("TimeMeasureGetData", logObject.TimeMeasureGetData))
        using (LogContext.PushProperty("TimeMeasureGetListSend", logObject.TimeMeasureGetListSend))
        using (LogContext.PushProperty("ImpactTotalTimeMeasure", logObject.ImpactTotalTimeMeasure))
        using (LogContext.PushProperty("ImpactKafkaSyncTime", logObject.ImpactKafkaSyncTime))
        using (LogContext.PushProperty("ImpactTotalDuration", logObject.ImpactTotalDuration))
        {
            Log.Logger.Information("");
        }
    }

    #endregion

    #region Overload WriteWarning

    public static void WriteWarning(LogObject? logObject, Exception? ex, object? obj = null, string optional = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "",
        [CallerLineNumber] int lineNumber = 0)
    {
        logObject ??= new LogObject(nameof(JsnException), $"{nameof(JsnException)}.{nameof(WriteWarning)}");
        if (obj != null)
        {
            var objJson = obj is string ? obj : ConvertHelper.ToJson(obj, true);
            logObject.ObjJson = objJson;
        }

        if (!string.IsNullOrEmpty(optional))
        {
            logObject.DescriptionOptional = optional;
        }

        #region ignore ObjJson

        if (!string.IsNullOrEmpty(logObject.ServiceType) && AppConfig.LogExceptionIgnoreObjJsonByServiceType != null && AppConfig.LogExceptionIgnoreObjJsonByServiceType.Any() &&
            AppConfig.LogExceptionIgnoreObjJsonByServiceType.Contains(logObject.ServiceType))
        {
            logObject.ObjJson = null;
        }

        #endregion ignore ObjJson

        var trace = $"{filePath}:{lineNumber} {memberName}";

        if (ex != null)
        {
            logObject.Description = GetInnerExceptionMessage(ex);

            using (LogContext.PushProperty("ClientName", logObject.ClientName))
            using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
            using (LogContext.PushProperty("ExceptionType", ex.GetType().Name))
            using (LogContext.PushProperty("Source", ex.Source))
            using (LogContext.PushProperty("StackTrace", $"{ex.StackTrace}\n{trace}"))
            using (LogContext.PushProperty("HelpLink", ex.HelpLink))
            {
                Log.Logger.Warning(ConvertHelper.ToJson(logObject, true));
            }
        }
        else
        {
            using (LogContext.PushProperty("ClientName", logObject.ClientName))
            using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
            using (LogContext.PushProperty("StackTrace", trace))
            {
                Log.Logger.Warning(ConvertHelper.ToJson(logObject, true));
            }
        }
    }

    public static void WriteWarning(LogObject? logObject, string optional = "", [CallerFilePath] string filePath = "", [CallerMemberName] string memberName = "", [CallerLineNumber] int lineNumber = 0)
    {
        logObject ??= new LogObject(nameof(JsnException), $"{nameof(JsnException)}.{nameof(WriteWarning)}");
        if (!string.IsNullOrEmpty(optional))
        {
            logObject.DescriptionOptional = optional;
        }

        #region ignore ObjJson

        if (!string.IsNullOrEmpty(logObject.ServiceType) && AppConfig.LogExceptionIgnoreObjJsonByServiceType != null && AppConfig.LogExceptionIgnoreObjJsonByServiceType.Any() &&
            AppConfig.LogExceptionIgnoreObjJsonByServiceType.Contains(logObject.ServiceType))
        {
            logObject.ObjJson = null;
        }

        var trace = $"{filePath}:{lineNumber} {memberName}";

        #endregion

        using (LogContext.PushProperty("ClientName", logObject.ClientName))
        using (LogContext.PushProperty("ServiceType", logObject.ServiceType))
        using (LogContext.PushProperty("StackTrace", trace))
        {
            Log.Logger.Warning(ConvertHelper.ToJson(logObject, true));
        }
    }

    #endregion

    #region Helpers

    public static string? GetInnerExceptionMessage(Exception? ex)
    {
        return !string.IsNullOrEmpty(ex?.InnerException?.Message) ? GetInnerExceptionMessage(ex.InnerException) : ex?.Message;
    }

    private static bool IsJsnException(Exception? ex)
    {
        return ex is JsnException;
    }

    #endregion
}
