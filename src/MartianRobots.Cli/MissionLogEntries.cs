using Microsoft.Extensions.Logging;

namespace MartianRobots.Cli;

/// <summary>The mission log's entries, in domain words, on any ILogger.</summary>
public static class MissionLogEntries
{
    public static void LogMissionStarted(this ILogger missionLogger) =>
        missionLogger.LogInformation("--- Mission started {Started:yyyy-MM-dd HH:mm:ss} ---", DateTime.Now);

    public static void LogLineReceived(this ILogger missionLogger, string earthLine) =>
        missionLogger.LogInformation("< {EarthLine}", earthLine);

    public static void LogResultSent(this ILogger missionLogger, string resultLine) =>
        missionLogger.LogInformation("> {ResultLine}", resultLine);
}
