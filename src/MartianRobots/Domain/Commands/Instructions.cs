namespace MartianRobots.Domain.Commands;

/// <summary>A robot's commands, in the order it carries them out.</summary>
public sealed class Instructions
{
    // Internal so only the line reader can create instructions: every set has already passed the length and known-letter checks.
    internal Instructions(IReadOnlyList<ICommand> commands) => Commands = commands;

    internal IReadOnlyList<ICommand> Commands { get; }
}
