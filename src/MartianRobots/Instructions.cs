using MartianRobots.Commands;

namespace MartianRobots;

public sealed class Instructions
{
    internal Instructions(IReadOnlyList<ICommand> commands) => Commands = commands;

    internal IReadOnlyList<ICommand> Commands { get; }
}
