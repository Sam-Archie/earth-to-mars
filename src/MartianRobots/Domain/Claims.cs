using MartianRobots.Domain.Positions;

namespace MartianRobots.Domain;

/// <summary>Earth's record of which robot claimed each square. Each square can be claimed once.</summary>
internal sealed class Claims
{
    private readonly Dictionary<Square, int> _claimingRobotNumbers = [];

    public IReadOnlyDictionary<Square, int> ClaimingRobotNumbers => _claimingRobotNumbers;

    public int Count => _claimingRobotNumbers.Count;

    public ClaimOutcome Claim(Square square, int robotNumber) =>
        _claimingRobotNumbers.TryAdd(square, robotNumber) ? ClaimOutcome.Claimed : ClaimOutcome.AlreadyClaimed;
}
