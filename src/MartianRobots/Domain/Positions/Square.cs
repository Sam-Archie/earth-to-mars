namespace MartianRobots.Domain.Positions;

public sealed record Square(int X, int Y)
{
    public override string ToString() => $"{X} {Y}";
}
