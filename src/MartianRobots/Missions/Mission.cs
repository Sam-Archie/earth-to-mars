using MartianRobots.Domain;
using MartianRobots.Domain.Commands;
using MartianRobots.Domain.Planets;
using MartianRobots.Domain.Positions;
using MartianRobots.Map;

namespace MartianRobots.Missions;

public sealed class Mission(IPlanet planet)
{
    private readonly Scents _scents = new();
    private readonly Claims _claims = new();

    /// <summary>The number Earth calls the next robot sent: robots count from 1 in the order sent, lost ones included.</summary>
    /// <remarks>
    /// The brief's input has no robot count, so nothing says how many robots a mission has; it simply counts them as they come.
    /// The mission keeps the count, rather than each caller, so the number Earth is asked for ("Robot 2, where do you land?")
    /// is the same number the robot claims its square with on the map.
    /// </remarks>
    public int NextRobotNumber { get; private set; } = 1;

    public RobotReport Send(Position start, Instructions instructions)
    {
        var robotNumber = NextRobotNumber++;
        var position = start;
        foreach (var command in instructions.Commands)
        {
            var proposed = command.Propose(position);
            // Off the edge, a robot is lost and leaves a scent where it stood, unless an earlier robot's
            // scent is already there: then the move is ignored and the robot carries on with its next command.
            if (planet.Contains(proposed.Square))
            {
                position = proposed;
            }
            else if (!_scents.Protects(position.Square))
            {
                _scents.LeaveOn(position.Square);
                return new RobotReport(position, Lost: true, ClaimOutcome.None, CompletedMission: false);
            }
        }

        // Only a robot that is still on Mars after its last command claims the square it ends on.
        var claimOutcome = _claims.Claim(position.Square, robotNumber);
        var planetIsFullyClaimed = claimOutcome == ClaimOutcome.Claimed && _claims.Count == planet.SquareCount;
        return new RobotReport(position, Lost: false, claimOutcome, planetIsFullyClaimed);
    }

    public string RenderMap() => MissionMap.Render(planet, _claims.ClaimingRobotNumbers, _scents.ScentedSquares);
}
