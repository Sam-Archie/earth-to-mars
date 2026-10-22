namespace MartianRobots.Domain.Positions;

/// <summary>Where a robot stands and which way it faces, printed as the brief prints it, e.g. "1 1 E".</summary>
public sealed record Position(Square Square, Orientation Orientation)
{
    public override string ToString() => $"{Square} {Orientation}";
}
