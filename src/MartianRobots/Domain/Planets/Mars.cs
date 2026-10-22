using MartianRobots.Domain.Positions;

namespace MartianRobots.Domain.Planets;

/// <summary>A rectangle of squares from 0 0 at the lower left to <see cref="UpperRight"/>, both corners included.</summary>
public sealed record Mars(Square UpperRight) : IPlanet
{
    // + 1 because both ends count: `5 3` means columns 0 to 5 and rows 0 to 3, so 6 × 4 squares.
    public int SquareCount => (UpperRight.X + 1) * (UpperRight.Y + 1);

    public bool Contains(Square square) =>
        square.X >= 0 && square.Y >= 0 && square.X <= UpperRight.X && square.Y <= UpperRight.Y;
}
