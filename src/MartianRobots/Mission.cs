namespace MartianRobots;

public sealed class Mission(Mars mars)
{
    private readonly HashSet<Square> _scents = [];
    private readonly HashSet<Square> _claims = [];

    public RobotReport Send(Position start, Instructions instructions)
    {
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

        if (!_claims.Add(position.Square))
        {
            return new RobotReport(position, Lost: false, ClaimOutcome.AlreadyClaimed, CompletedMission: false);
        }

        var marsIsFullyClaimed = _claims.Count == mars.SquareCount;
        return new RobotReport(position, Lost: false, ClaimOutcome.Claimed, marsIsFullyClaimed);
    }
}
