using MartianRobots.Cli;
using MartianRobots.Missions;

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
