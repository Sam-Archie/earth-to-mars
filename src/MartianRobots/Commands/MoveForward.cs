namespace MartianRobots.Commands;

internal sealed class MoveForward : ICommand
{
    public Position Propose(Position current) => current with { Square = Ahead(current) };

    private static Square Ahead(Position position) => position.Orientation switch
    {
        Orientation.N => position.Square with { Y = position.Square.Y + 1 },
        Orientation.E => position.Square with { X = position.Square.X + 1 },
        Orientation.S => position.Square with { Y = position.Square.Y - 1 },
        _ => position.Square with { X = position.Square.X - 1 },
    };
}
