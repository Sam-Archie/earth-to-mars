using MartianRobots.Domain.Commands;
using MartianRobots.Domain.Positions;

namespace MartianRobots.Cli;

/// <summary>A robot Earth has finished sending: its number, where it lands and its instructions.</summary>
public sealed record RobotToSend(int Number, Position StartPosition, Instructions Instructions);
