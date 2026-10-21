using Microsoft.Extensions.Logging;

namespace MartianRobots.Cli;

/// <summary>
/// Appends each mission to logs/missions.txt under the working directory. Every entry is flushed as it is
/// written, so a mission cut off by Ctrl+D still leaves its record. Any other ILogger can take its place,
/// since every entry, the mission header included, comes through MissionLogEntries.
/// </summary>
public sealed class MissionFileLogger : ILogger, IDisposable
{
    private readonly StreamWriter missionLog;

    public MissionFileLogger()
    {
        Directory.CreateDirectory("logs");
        missionLog = new StreamWriter(Path.Combine("logs", "missions.txt"), append: true) { AutoFlush = true };
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter) =>
        missionLog.WriteLine($"{DateTime.Now:HH:mm:ss} {formatter(state, exception)}");

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public void Dispose() => missionLog.Dispose();
}
