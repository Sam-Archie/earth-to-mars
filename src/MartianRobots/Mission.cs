namespace MartianRobots;

public sealed class Mission(Mars mars)
{
    private readonly HashSet<Square> _scents = [];

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
                return new RobotReport($"{position} LOST", Lost: true);
            }
        }

        return new RobotReport(position.ToString(), Lost: false);
    }
}
