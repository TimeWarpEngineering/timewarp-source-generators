namespace TimeWarp.SourceGenerators.TestConsole;

/// <summary>
/// Simple logging interface for testing interface delegation.
/// </summary>
public interface ILogger
{
    /// <summary>
    /// Logs an informational message.
    /// </summary>
    /// <param name="message">The message to log.</param>
    void Log(string message);

    /// <summary>
    /// Logs an error message with an exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="exception">The exception that occurred.</param>
    void LogError(string message, Exception exception);

    /// <summary>
    /// Gets or sets the minimum log level.
    /// </summary>
    LogLevel MinimumLevel { get; set; }

    /// <summary>
    /// Occurs when a log entry is written.
    /// </summary>
    event EventHandler<LogEventArgs>? LogWritten;
}

/// <summary>
/// Represents the severity level of a log entry.
/// </summary>
public enum LogLevel
{
    Debug,
    Info,
    Warning,
    Error
}

/// <summary>
/// Event arguments for log events.
/// </summary>
public class LogEventArgs : EventArgs
{
    public string Message { get; init; } = string.Empty;
    public LogLevel Level { get; init; }
}
