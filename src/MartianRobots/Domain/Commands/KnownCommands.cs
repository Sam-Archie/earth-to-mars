namespace MartianRobots.Domain.Commands;

/// <summary>Every command a robot understands, by its letter. Adding a command type is one class and one entry here.</summary>
internal static class KnownCommands
{
    public static IReadOnlyDictionary<char, ICommand> ByLetter { get; } = new Dictionary<char, ICommand>
    {
        ['L'] = new TurnLeft(),
        ['R'] = new TurnRight(),
        ['F'] = new MoveForward(),
    };
}
