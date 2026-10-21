namespace MartianRobots;

public sealed record RobotReport(Position Final, bool Lost, ClaimOutcome Claim)
{
    /// <summary>The robot's final position as the brief prints it, e.g. "3 3 N LOST".</summary>
    public string ResultLine => Lost ? $"{Final} LOST" : Final.ToString();
}
