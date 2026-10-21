using System.Globalization;
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

    private static readonly IReadOnlyDictionary<string, Orientation> OrientationsByLetter =
        Enum.GetValues<Orientation>().ToDictionary(orientation => orientation.ToString());

    public static LineRead<Mars> ReadMars(string line)
    {
        var parts = PartsOf(line);
        if (parts.Length != 2 || ReadSquare(parts) is not { } upperRight)
        {
            return LineRead<Mars>.Bad("Static on the line. Mars's size is the upper-right square, two numbers like `5 3`.");
        }

        return upperRight.X <= MaxCoordinate && upperRight.Y <= MaxCoordinate
            ? LineRead<Mars>.Parsed(new Mars(upperRight))
            : LineRead<Mars>.Bad($"Our telescopes only reach so far. Each coordinate is 0 to {MaxCoordinate}.");
    }

    public static LineRead<Position> ReadPosition(string line, Mars mars)
    {
        var parts = PartsOf(line);
        if (parts.Length != 3 || ReadSquare(parts) is not { } square)
        {
            return LineRead<Position>.Bad("Garbled position. Send a square and an orientation, like `1 1 E`.");
        }

        if (!mars.Contains(square))
        {
            return LineRead<Position>.Bad($"That square is off the map. Robots start between `0 0` and `{mars.UpperRight}`.");
        }

        return OrientationsByLetter.TryGetValue(parts[2], out var orientation)
            ? LineRead<Position>.Parsed(new Position(square, orientation))
            : LineRead<Position>.Bad("The robot's compass is spinning. Face it N, E, S or W, like `1 1 E`.");
    }

    public static LineRead<Instructions> ReadInstructions(string line)
    {
        var letters = Tidy(line);
        if (letters.Length >= InstructionsLimit)
        {
            return LineRead<Instructions>.Bad(
                $"That's {letters.Length} letters and the antenna overheats at {InstructionsLimit}. Send fewer than {InstructionsLimit}.");
        }

        var commands = new List<ICommand>();
        foreach (var letter in letters)
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

    private static string Tidy(string line) => line.Trim(' ', '\t', '\r').ToUpperInvariant();

    private static string[] PartsOf(string line) => Tidy(line).Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

    /// <summary>The square named by a line's first two parts, or null when they aren't both numbers.</summary>
    private static Square? ReadSquare(string[] parts) =>
        TryReadNumber(parts[0], out var x) && TryReadNumber(parts[1], out var y) ? new Square(x, y) : null;

    // Digits only, the same on every machine: no sign, separators or local number formats.
    private static bool TryReadNumber(string part, out int number) =>
        int.TryParse(part, NumberStyles.None, CultureInfo.InvariantCulture, out number);
}
