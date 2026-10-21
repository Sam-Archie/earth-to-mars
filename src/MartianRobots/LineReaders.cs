using MartianRobots.Commands;

namespace MartianRobots;

/// <summary>Reads each kind of line Earth sends: the size of Mars, a robot's starting position, and its instructions.</summary>
public static class LineReaders
{
    private static readonly IReadOnlyDictionary<char, ICommand> CommandsByLetter = new Dictionary<char, ICommand>
    {
        ['L'] = new TurnLeft(),
        ['R'] = new TurnRight(),
        ['F'] = new MoveForward(),
    };

    public static Mars ReadMars(string line)
    {
        var parts = line.Split(' ');
        return new Mars(new Square(int.Parse(parts[0]), int.Parse(parts[1])));
    }

    public static Position ReadPosition(string line)
    {
        var parts = line.Split(' ');
        return new Position(new Square(int.Parse(parts[0]), int.Parse(parts[1])), Enum.Parse<Orientation>(parts[2]));
    }

    public static Instructions ReadInstructions(string line) =>
        new(line.Select(letter => CommandsByLetter[letter]).ToList());
}
