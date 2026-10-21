using MartianRobots.Missions;

namespace MartianRobots.Cli;

/// <summary>One mission run over the console: asks Earth for Mars and each robot, then reports every robot as it finishes.</summary>
public static class MissionControl
{
    public static void Start()
    {
        using var missionLogger = new MissionFileLogger();
        missionLogger.LogMissionStarted();
        var earthConsole = new EarthConsole(missionLogger);
        var mars = earthConsole.AskForMars();
        if (mars is null)
        {
            return;
        }

        var mission = new Mission(mars);
        while (earthConsole.AskForRobot(mission.NextRobotNumber, mars) is { } robot)
        {
            var report = mission.Send(robot.StartPosition, robot.Instructions);
            Console.WriteLine(report.ResultLine);
            missionLogger.LogResultSent(report.ResultLine);
            RobotAnnouncements.Announce(robot.Number, report);
            Console.Error.WriteLine(mission.RenderMap());
        }
    }
}
