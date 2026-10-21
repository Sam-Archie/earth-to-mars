using MartianRobots.Cli;
using MartianRobots.Domain.Commands;
using MartianRobots.Domain.Planets;
using MartianRobots.Domain.Positions;
using MartianRobots.Input;
using MartianRobots.Missions;
using Microsoft.Extensions.Logging;

using var missionLogger = new MissionFileLogger();
missionLogger.LogMissionStarted();
var mars = AskUntilRead("Earth to Mars, come in. How big is Mars? Send its upper-right square, like `5 3`.", LineReaders.ReadMars, missionLogger);
if (mars is null)
{
    return;
}

var mission = new Mission(mars);
while (AskForRobot(mission.NextRobotNumber, mars, missionLogger) is { } robot)
{
    var report = mission.Send(robot.StartPosition, robot.Instructions);
    Console.WriteLine(report.ResultLine);
    missionLogger.LogResultSent(report.ResultLine);
    RobotAnnouncements.Announce(robot.Number, report);
    Console.Error.WriteLine(mission.RenderMap());
}

// Null means Earth stopped transmitting. A robot that has a position but no instructions stays in the hangar.
static RobotToSend? AskForRobot(int robotNumber, Mars mars, ILogger missionLogger)
{
    var startPosition = AskUntilRead($"Robot {robotNumber}, where do you land? Send a square and an orientation, like `1 1 E`.",
        line => LineReaders.ReadPosition(line, mars), missionLogger);
    if (startPosition is null)
    {
        return null;
    }

    var instructions = AskUntilRead($"Robot {robotNumber}, standing by for instructions, like `RFRF`.", LineReaders.ReadInstructions, missionLogger);
    if (instructions is null)
    {
        Console.Error.WriteLine($"Signal lost before robot {robotNumber} at {startPosition} got its instructions. It stays in the hangar.");
        return null;
    }

    return new RobotToSend(robotNumber, startPosition, instructions);
}

// Prompts and hints go to stderr so stdout carries only the brief's result lines. Null means Earth stopped transmitting.
static T? AskUntilRead<T>(string prompt, Func<string, LineRead<T>> readLine, ILogger missionLogger) where T : class
{
    while (true)
    {
        Console.Error.WriteLine(prompt);
        if (Console.ReadLine() is not { } line)
        {
            return null;
        }

        missionLogger.LogLineReceived(line);
        var attempt = readLine(line);
        if (attempt.Value is { } value)
        {
            return value;
        }

        Console.Error.WriteLine(attempt.Hint);
    }
}

/// <summary>A robot Earth has finished sending: its number, where it lands and its instructions.</summary>
internal sealed record RobotToSend(int Number, Position StartPosition, Instructions Instructions);
