using Microsoft.Extensions.Logging;

namespace Shared;

public enum LogType
{
    Information,
    Warning,
    Error,
    Critical,
    Debug,
    Trace
}

public class ServiceLogger
{
    private readonly ILogger _logger;
    private readonly long _logId;
    private readonly DateTime _logTime;

    public ServiceLogger(ILogger logger)
    {
        _logger = logger;
        _logId = Random.Shared.NextInt64();
        _logTime = DateTime.UtcNow;
    }

    public long LogId => _logId;
    public DateTime LogTime => _logTime;

    public void Log(LogType type, string message, params object?[] args)
    {
        var logLevel = type switch
        {
            LogType.Information => LogLevel.Information,
            LogType.Warning => LogLevel.Warning,
            LogType.Error => LogLevel.Error,
            LogType.Critical => LogLevel.Critical,
            LogType.Debug => LogLevel.Debug,
            LogType.Trace => LogLevel.Trace,
            _ => LogLevel.Information
        };

        var timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss.fff");
        var formattedMessage = $"[{timestamp}] [{_logId}] {message}";
        _logger.Log(logLevel, formattedMessage, args);
    }

    public void Information(string message, params object?[] args) =>
        Log(LogType.Information, message, args);

    public void Warning(string message, params object?[] args) =>
        Log(LogType.Warning, message, args);

    public void Error(string message, params object?[] args) =>
        Log(LogType.Error, message, args);

    public void Error(Exception ex, string message, params object?[] args) =>
        Log(LogType.Error, message + " - Exception: {Exception}", [..args, ex]);

    public void Critical(string message, params object?[] args) =>
        Log(LogType.Critical, message, args);

    public void Debug(string message, params object?[] args) =>
        Log(LogType.Debug, message, args);

    public void Trace(string message, params object?[] args) =>
        Log(LogType.Trace, message, args);
}
