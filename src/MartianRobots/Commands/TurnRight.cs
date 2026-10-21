namespace MartianRobots.Commands;

internal sealed class TurnRight : ICommand
{
    public Position Propose(Position current) => current with { Orientation = RightOf(current.Orientation) };

    private static Orientation RightOf(Orientation orientation) => orientation switch
    {
        Orientation.N => Orientation.E,
        Orientation.E => Orientation.S,
        Orientation.S => Orientation.W,
        _ => Orientation.N,
    };
}
