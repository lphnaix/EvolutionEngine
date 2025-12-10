namespace Engine.Core.Logging;

public enum LogLevel
{
    Debug,
    Info,
    Warn,
    Error
}

public interface ILogger
{
    void Log(LogLevel level, string message);
}

/// <summary>
/// Simple console/file logger placeholder. File support can be added later.
/// </summary>
public sealed class ConsoleLogger : ILogger
{
    private readonly object _lock = new();

    public void Log(LogLevel level, string message)
    {
        lock (_lock)
        {
            var prefix = $"[{DateTime.UtcNow:O}] [{level}] ";
            Console.WriteLine(prefix + message);
        }
    }
}
