namespace MartianRobots.Domain.Commands;

public sealed class Instructions
{
    internal Instructions(IReadOnlyList<ICommand> commands) => Commands = commands;

    internal IReadOnlyList<ICommand> Commands { get; }
}
