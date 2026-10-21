namespace MartianRobots;

public sealed class Mission(Mars mars)
{
    public RobotReport Send(Position start, Instructions instructions)
    {
        var position = start;
        foreach (var command in instructions.Commands)
        {
            var proposed = command.Propose(position);
            if (!mars.Contains(proposed.Square))
            {
                return new RobotReport($"{position} LOST", Lost: true);
            }

            position = proposed;
        }

        return new RobotReport(position.ToString(), Lost: false);
    }
}
