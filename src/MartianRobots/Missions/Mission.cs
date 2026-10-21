using MartianRobots.Domain;
using MartianRobots.Domain.Commands;
using MartianRobots.Domain.Planets;
using MartianRobots.Domain.Positions;
using MartianRobots.Map;

namespace MartianRobots.Missions;

public sealed class Mission(Mars mars)
{
    private readonly HashSet<Square> _scents = [];
    private readonly Dictionary<Square, int> _claimingRobotNumbers = [];

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
            else if (!_scents.Contains(position.Square))
            {
                _scents.Add(position.Square);
                return new RobotReport(position, Lost: true, ClaimOutcome.None, CompletedMission: false);
            }
        }

        if (!_claimingRobotNumbers.TryAdd(position.Square, robotNumber))
        {
            return new RobotReport(position, Lost: false, ClaimOutcome.AlreadyClaimed, CompletedMission: false);
        }

        var marsIsFullyClaimed = _claimingRobotNumbers.Count == mars.SquareCount;
        return new RobotReport(position, Lost: false, ClaimOutcome.Claimed, marsIsFullyClaimed);
    }

    public string RenderMap() => MissionMap.Render(mars, _claimingRobotNumbers, _scents);
}
