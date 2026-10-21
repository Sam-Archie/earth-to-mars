namespace MartianRobots;

public sealed class Mission(Mars mars)
{
    public RobotReport Send(Position start, Instructions instructions) => new(start.ToString(), Lost: false);
}
