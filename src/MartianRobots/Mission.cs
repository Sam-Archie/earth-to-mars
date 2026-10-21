namespace MartianRobots;

public sealed class Mission(Mars mars)
{
    private readonly HashSet<Square> _scents = [];
    private readonly Dictionary<Square, int> _claimingRobotNumbers = [];
    private int _robotsSent;

    public RobotReport Send(Position start, Instructions instructions)
    {
        var robotNumber = ++_robotsSent;
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
