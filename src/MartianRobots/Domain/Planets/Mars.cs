using MartianRobots.Domain.Positions;

namespace MartianRobots.Domain.Planets;

public sealed record Mars(Square UpperRight)
{
    internal int SquareCount => (UpperRight.X + 1) * (UpperRight.Y + 1);

    public bool Contains(Square square) =>
        square.X >= 0 && square.Y >= 0 && square.X <= UpperRight.X && square.Y <= UpperRight.Y;
}
