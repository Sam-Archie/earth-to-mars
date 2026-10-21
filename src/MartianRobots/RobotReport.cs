namespace MartianRobots;

/// <param name="ResultLine">The robot's final position as the brief prints it, e.g. "3 3 N LOST".</param>
public sealed record RobotReport(string ResultLine, bool Lost);
