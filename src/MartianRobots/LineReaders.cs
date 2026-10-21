using MartianRobots.Commands;

namespace MartianRobots;

/// <summary>Reads each kind of line Earth sends: the size of Mars, a robot's starting position, and its instructions.</summary>
public static class LineReaders
{
    private const int MaxCoordinate = 50;
    private const int InstructionsLimit = 100;

    private static readonly IReadOnlyDictionary<char, ICommand> CommandsByLetter = new Dictionary<char, ICommand>
    {
        ['L'] = new TurnLeft(),
        ['R'] = new TurnRight(),
        ['F'] = new MoveForward(),
    };

    public static LineRead<Mars> ReadMars(string line)
    {
        var parts = Tidy(line).Split(' ');
        if (parts.Length != 2 || !TryReadSquare(parts[0], parts[1], out var upperRight))
        {
            return LineRead<Mars>.Bad("Static on the line. Mars's size is the upper-right square, two numbers like `5 3`.");
        }

        return IsCoordinate(upperRight.X) && IsCoordinate(upperRight.Y)
            ? LineRead<Mars>.Parsed(new Mars(upperRight))
            : LineRead<Mars>.Bad($"Our telescopes only reach so far. Each coordinate is 0 to {MaxCoordinate}.");
    }

    public static LineRead<Position> ReadPosition(string line, Mars mars)
    {
        var parts = Tidy(line).Split(' ');
        if (parts.Length != 3 || !TryReadSquare(parts[0], parts[1], out var square))
        {
            return LineRead<Position>.Bad("Garbled position. Send a square and an orientation, like `1 1 E`.");
        }

        if (!mars.Contains(square))
        {
            return LineRead<Position>.Bad($"That square is off the map. Robots start between `0 0` and `{mars.UpperRight}`.");
        }

        // Matched by name because Enum.TryParse would also take "1" or "N,E".
        return Enum.GetNames<Orientation>().Contains(parts[2])
            ? LineRead<Position>.Parsed(new Position(square, Enum.Parse<Orientation>(parts[2])))
            : LineRead<Position>.Bad("The robot's compass is spinning. Face it N, E, S or W, like `1 1 E`.");
    }

    public static LineRead<Instructions> ReadInstructions(string line)
    {
        line = Tidy(line);
        if (line.Length >= InstructionsLimit)
        {
            return LineRead<Instructions>.Bad(
                $"That's {line.Length} letters and the antenna overheats at {InstructionsLimit}. Send fewer than {InstructionsLimit}.");
        }

        var commands = new List<ICommand>();
        foreach (var letter in line)
        {
            if (!CommandsByLetter.TryGetValue(letter, out var command))
            {
                return LineRead<Instructions>.Bad(
                    $"Mars doesn't speak `{letter}`. Robots know {string.Join(", ", CommandsByLetter.Keys)}, like `RFRF`.");
            }

            commands.Add(command);
        }

        return LineRead<Instructions>.Parsed(new Instructions(commands));
    }

    private static string Tidy(string line) => line.ToUpperInvariant();

    private static bool TryReadSquare(string x, string y, out Square square)
    {
        var read = int.TryParse(x, out var column) & int.TryParse(y, out var row);
        square = new Square(column, row);
        return read;
    }

    private static bool IsCoordinate(int value) => value is >= 0 and <= MaxCoordinate;
}
