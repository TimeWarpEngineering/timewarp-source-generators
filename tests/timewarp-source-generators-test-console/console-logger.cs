namespace TimeWarp.SourceGenerators.TestConsole;

/// <summary>
/// A simple console-based logger implementation.
/// </summary>
public class ConsoleLogger : ILogger
{
    public LogLevel MinimumLevel { get; set; } = LogLevel.Info;

    public event EventHandler<LogEventArgs>? LogWritten;

    public void Log(string message)
    {
        if (MinimumLevel <= LogLevel.Info)
        {
            Console.WriteLine($"[INFO] {message}");
            OnLogWritten(message, LogLevel.Info);
        }
    }

    public void LogError(string message, Exception exception)
    {
        if (MinimumLevel <= LogLevel.Error)
        {
            Console.WriteLine($"[ERROR] {message}: {exception.Message}");
            OnLogWritten($"{message}: {exception.Message}", LogLevel.Error);
        }
    }

    private void OnLogWritten(string message, LogLevel level)
    {
        LogWritten?.Invoke(this, new LogEventArgs { Message = message, Level = level });
    }
}
