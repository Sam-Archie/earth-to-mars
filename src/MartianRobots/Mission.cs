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
                return new RobotReport(position, Lost: true, ClaimOutcome.None);
            }
        }

        var claim = _claims.Add(position.Square) ? ClaimOutcome.Claimed : ClaimOutcome.AlreadyClaimed;
        return new RobotReport(position, Lost: false, claim);
    }
}
