using MartianRobots.Domain;
using MartianRobots.Missions;

namespace MartianRobots.Cli;

/// <summary>What Earth hears about each robot on stderr: lost in space, claimed, already claimed, Mission complete.</summary>
public static class RobotAnnouncements
{
    public static void Announce(int robotNumber, RobotReport report)
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
}
