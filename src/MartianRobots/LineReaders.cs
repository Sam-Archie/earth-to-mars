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

    private const int MaxCoordinate = 50;

    public static LineRead<Mars> ReadMars(string line)
    {
        var parts = line.Split(' ');
        if (parts.Length != 2 || !int.TryParse(parts[0], out var x) || !int.TryParse(parts[1], out var y))
        {
            return LineRead<Mars>.Bad("Static on the line. Mars's size is the upper-right square, two numbers like `5 3`.");
        }

        return IsCoordinate(x) && IsCoordinate(y)
            ? LineRead<Mars>.Parsed(new Mars(new Square(x, y)))
            : LineRead<Mars>.Bad($"Our telescopes only reach so far. Each coordinate is 0 to {MaxCoordinate}.");
    }

    private static bool IsCoordinate(int value) => value is >= 0 and <= MaxCoordinate;

    public static Position ReadPosition(string line)
    {
        var parts = line.Split(' ');
        return new Position(new Square(int.Parse(parts[0]), int.Parse(parts[1])), Enum.Parse<Orientation>(parts[2]));
    }

    public static Instructions ReadInstructions(string line) =>
        new(line.Select(letter => CommandsByLetter[letter]).ToList());
}
