using MartianRobots;
using MartianRobots.Cli;
using Microsoft.Extensions.Logging;

using var missionLogger = new MissionFileLogger();
missionLogger.LogMissionStarted();
var mars = AskUntilRead("Earth to Mars, come in. How big is Mars? Send its upper-right square, like `5 3`.", LineReaders.ReadMars, missionLogger);
if (mars is null)
{
    return;
}

var mission = new Mission(mars);
while (true)
{
    var robotNumber = mission.NextRobotNumber;
    var startPosition = AskUntilRead($"Robot {robotNumber}, where do you land? Send a square and an orientation, like `1 1 E`.",
        line => LineReaders.ReadPosition(line, mars), missionLogger);
    if (startPosition is null)
    {
        return;
    }

    var instructions = AskUntilRead($"Robot {robotNumber}, standing by for instructions, like `RFRF`.", LineReaders.ReadInstructions, missionLogger);
    if (instructions is null)
    {
        Console.Error.WriteLine($"Signal lost before robot {robotNumber} at {startPosition} got its instructions. It stays in the hangar.");
        return;
    }

    var report = mission.Send(startPosition, instructions);
    Console.WriteLine(report.ResultLine);
    missionLogger.LogResultSent(report.ResultLine);
    Announce(robotNumber, report);
    Console.Error.WriteLine(mission.RenderMap());
}

static void Announce(int robotNumber, RobotReport report)
{
    var square = report.Final.Square;
    if (report.Lost)
    {
        Console.Error.WriteLine($"Robot {robotNumber} is lost in space, last seen at {square}. It leaves a scent so no one else follows.");
        return;
    }

    Console.Error.WriteLine(report.Claim == ClaimOutcome.Claimed
        ? $"Robot {robotNumber} claims {square} for Earth. Earth applauds politely."
        : $"Robot {robotNumber} reaches {square}, but it is already claimed. Awkward. It pretends it was just passing.");

    if (report.CompletedMission)
    {
        Console.Error.WriteLine("Mission complete! Every square on Mars is claimed. Earth breaks out the space biscuits.");
    }
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
