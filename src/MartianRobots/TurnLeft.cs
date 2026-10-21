namespace MartianRobots;

internal sealed class TurnLeft : ICommand
{
    public Position Propose(Position current) => current with { Orientation = LeftOf(current.Orientation) };

    private static Orientation LeftOf(Orientation orientation) => orientation switch
    {
        Orientation.N => Orientation.W,
        Orientation.W => Orientation.S,
        Orientation.S => Orientation.E,
        _ => Orientation.N,
    };
}
