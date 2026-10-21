using MartianRobots.Domain.Planets;
using MartianRobots.Input;
using Microsoft.Extensions.Logging;

namespace MartianRobots.Cli;

/// <summary>
/// The link Earth's transmissions arrive on: asks Earth for each line, reads it from stdin, and asks again until the line reads.
/// Prompts and hints go to stderr so stdout carries only the brief's result lines. Null means Earth stopped transmitting.
/// </summary>
public sealed class EarthLink(ILogger missionLogger)
{
    public Mars? AskForMars() =>
        AskUntilRead("Earth to Mars, come in. How big is Mars? Send its upper-right square, like `5 3`.", LineReaders.ReadMars);

    /// <summary>A robot that has a position but no instructions when Earth stops transmitting stays in the hangar.</summary>
    public RobotToSend? AskForRobot(int robotNumber, Mars mars)
    {
        var startPosition = AskUntilRead($"Robot {robotNumber}, where do you land? Send a square and an orientation, like `1 1 E`.",
            line => LineReaders.ReadPosition(line, mars));
        if (startPosition is null)
        {
            return null;
        }

        var instructions = AskUntilRead($"Robot {robotNumber}, standing by for instructions, like `RFRF`.", LineReaders.ReadInstructions);
        if (instructions is null)
        {
            Console.Error.WriteLine($"Signal lost before robot {robotNumber} at {startPosition} got its instructions. It stays in the hangar.");
            return null;
        }

        return new RobotToSend(robotNumber, startPosition, instructions);
    }

    private T? AskUntilRead<T>(string prompt, Func<string, LineRead<T>> readLine) where T : class
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
}
