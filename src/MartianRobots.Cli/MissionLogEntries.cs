using Microsoft.Extensions.Logging;

namespace MartianRobots.Cli;

/// <summary>The mission log's entries, in domain words, on any ILogger.</summary>
public static class MissionLogEntries
{
    public static void LogLineReceived(this ILogger missionLogger, string line) =>
        missionLogger.LogInformation("< {Line}", line);

    public static void LogResultSent(this ILogger missionLogger, string resultLine) =>
        missionLogger.LogInformation("> {ResultLine}", resultLine);
}
