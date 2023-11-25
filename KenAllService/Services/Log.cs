namespace KenAllService.Services;

#pragma warning disable SYSLIB1006
public static partial class Log
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Import completed. success=[{success}], failed=[{failed}]")]
    public static partial void InfoImportCompleted(this ILogger logger, int success, long failed);
}
#pragma warning restore SYSLIB1006
