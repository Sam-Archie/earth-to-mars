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

    public static LineRead<Position> ReadPosition(string line, Mars mars)
    {
        var parts = line.Split(' ');
        var square = new Square(int.Parse(parts[0]), int.Parse(parts[1]));
        if (!mars.Contains(square))
        {
            return LineRead<Position>.Bad($"That square is off the map. Robots start between `0 0` and `{mars.UpperRight}`.");
        }

        // Matched by name because Enum.TryParse would also take "1" or "N,E".
        return Enum.GetNames<Orientation>().Contains(parts[2])
            ? LineRead<Position>.Parsed(new Position(square, Enum.Parse<Orientation>(parts[2])))
            : LineRead<Position>.Bad("The robot's compass is spinning. Face it N, E, S or W, like `1 1 E`.");
    }

    public static Instructions ReadInstructions(string line) =>
        new(line.Select(letter => CommandsByLetter[letter]).ToList());
}
