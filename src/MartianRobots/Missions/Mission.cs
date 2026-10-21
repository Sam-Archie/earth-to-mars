using MartianRobots.Domain;
using MartianRobots.Domain.Commands;
using MartianRobots.Domain.Planets;
using MartianRobots.Domain.Positions;
using MartianRobots.Map;

namespace MartianRobots.Missions;

public sealed class Mission(Mars mars)
{
    private readonly Scents _scents = new();
    private readonly Claims _claims = new();

    /// <summary>The number Earth calls the next robot sent: robots count from 1 in the order sent, lost ones included.</summary>
    public int NextRobotNumber { get; private set; } = 1;

    public RobotReport Send(Position start, Instructions instructions)
    {
        var robotNumber = NextRobotNumber++;
        var position = start;
        foreach (var command in instructions.Commands)
        {
            var proposed = command.Propose(position);
            if (mars.Contains(proposed.Square))
            {
                position = proposed;
            }
            else if (!_scents.Protects(position.Square))
            {
                _scents.LeaveOn(position.Square);
                return new RobotReport(position, Lost: true, ClaimOutcome.None, CompletedMission: false);
            }
        }

        var claimOutcome = _claims.Claim(position.Square, robotNumber);
        var marsIsFullyClaimed = claimOutcome == ClaimOutcome.Claimed && _claims.Count == mars.SquareCount;
        return new RobotReport(position, Lost: false, claimOutcome, marsIsFullyClaimed);
    }

    public string RenderMap() => MissionMap.Render(mars, _claims.ClaimingRobotNumbers, _scents.ScentedSquares);
}
