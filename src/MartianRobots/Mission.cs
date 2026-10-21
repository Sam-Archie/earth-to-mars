namespace MartianRobots;

public sealed class Mission(Mars mars)
{
    public RobotReport Send(Position start, Instructions instructions)
    {
        var position = start;
        foreach (var command in instructions.Commands)
        {
            position = command.Propose(position);
        }

        return new RobotReport(position.ToString(), Lost: false);
    }
}
