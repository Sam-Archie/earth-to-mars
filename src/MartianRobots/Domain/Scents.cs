using MartianRobots.Domain.Positions;

namespace MartianRobots.Domain;

/// <summary>The squares lost robots left a scent on. A scented square stops any later robot moving off the planet from it.</summary>
internal sealed class Scents
{
    private readonly HashSet<Square> _scentedSquares = [];

    public IReadOnlySet<Square> ScentedSquares => _scentedSquares;

    public bool Protects(Square square) => _scentedSquares.Contains(square);

    public void LeaveOn(Square square) => _scentedSquares.Add(square);
}
