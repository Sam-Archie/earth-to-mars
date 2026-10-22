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
            Highlight.Message($"Robot {robotNumber} is lost in space, last seen at {square}. It leaves a scent so no one else follows.", Colour.Red);
            return;
        }

        if (report.Claim == ClaimOutcome.Claimed)
        {
            Highlight.Message($"Robot {robotNumber} claims {square} for Earth. Earth applauds politely.", Colour.Green);
        }
        else
        {
            Highlight.Message($"Robot {robotNumber} reaches {square}, but it is already claimed. Awkward. It pretends it was just passing.", Colour.Yellow);
        }

        if (report.CompletedMission)
        {
            Highlight.Message("Mission complete! Every square on Mars is claimed. Earth breaks out the space biscuits.", Colour.Magenta);
        }
    }
}
