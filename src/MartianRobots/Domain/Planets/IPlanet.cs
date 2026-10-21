using MartianRobots.Domain.Positions;

namespace MartianRobots.Domain.Planets;

/// <summary>A bounded rectangle of squares robots can be sent to, from `0 0` to its upper-right square.</summary>
public interface IPlanet
{
    Square UpperRight { get; }

    int SquareCount { get; }

    bool Contains(Square square);
}
