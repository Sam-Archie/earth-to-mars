namespace MartianRobots;

public sealed record Mars(Square UpperRight)
{
    public bool Contains(Square square) =>
        square.X >= 0 && square.Y >= 0 && square.X <= UpperRight.X && square.Y <= UpperRight.Y;
}
