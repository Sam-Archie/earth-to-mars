namespace MartianRobots.Domain.Positions;

public sealed record Position(Square Square, Orientation Orientation)
{
    public override string ToString() => $"{Square} {Orientation}";
}
