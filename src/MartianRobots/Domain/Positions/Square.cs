namespace MartianRobots.Domain.Positions;

/// <summary>One grid square on a planet, printed as the brief prints it, e.g. "1 1".</summary>
public sealed record Square(int X, int Y)
{
    public override string ToString() => $"{X} {Y}";
}
