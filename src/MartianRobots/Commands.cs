namespace MartianRobots;

internal static class Commands
{
    public static readonly IReadOnlyDictionary<char, ICommand> ByLetter = new Dictionary<char, ICommand>
    {
        ['L'] = new TurnLeft(),
        ['R'] = new TurnRight(),
    };
}
