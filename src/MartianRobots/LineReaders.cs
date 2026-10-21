using System.Globalization;
using MartianRobots.Commands;

namespace MartianRobots;

/// <summary>Reads each kind of line Earth sends: the size of Mars, a robot's starting position, and its instructions.</summary>
public static class LineReaders
{
    private const int MaxCoordinate = 50;
    private const int InstructionsLengthLimit = 100;

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
        var values = SplitIntoValues(line);
        if (values is not [var upperRightX, var upperRightY] || ReadSquare(upperRightX, upperRightY) is not { } upperRight)
        {
            return LineRead<Mars>.Bad("Static on the line. Mars's size is the upper-right square, two numbers like `5 3`.");
        }

        var marsIsSmallEnough = upperRight.X <= MaxCoordinate && upperRight.Y <= MaxCoordinate;
        if (!marsIsSmallEnough)
        {
            return LineRead<Mars>.Bad($"Our telescopes only reach so far. Each coordinate is 0 to {MaxCoordinate}.");
        }

        return LineRead<Mars>.Parsed(new Mars(upperRight));
    }

    public static LineRead<Position> ReadPosition(string line, Mars mars)
    {
        var values = SplitIntoValues(line);
        if (values is not [var startX, var startY, var orientationLetter] || ReadSquare(startX, startY) is not { } startSquare)
        {
            return LineRead<Position>.Bad("Garbled position. Send a square and an orientation, like `1 1 E`.");
        }

        if (!mars.Contains(startSquare))
        {
            return LineRead<Position>.Bad($"That square is off the map. Robots start between `0 0` and `{mars.UpperRight}`.");
        }

        if (!OrientationsByLetter.TryGetValue(orientationLetter, out var orientation))
        {
            return LineRead<Position>.Bad("The robot's compass is spinning. Face it N, E, S or W, like `1 1 E`.");
        }

        return LineRead<Position>.Parsed(new Position(startSquare, orientation));
    }

    public static LineRead<Instructions> ReadInstructions(string line)
    {
        var letters = TrimAndUppercase(line);
        var tooManyLetters = letters.Length >= InstructionsLengthLimit;
        if (tooManyLetters)
        {
            return LineRead<Instructions>.Bad(
                $"That's {letters.Length} letters and the antenna overheats at {InstructionsLengthLimit}. Send fewer than {InstructionsLengthLimit}.");
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

    private static string TrimAndUppercase(string line) => line.Trim(' ', '\t', '\r').ToUpperInvariant();

    /// <summary>The values in one line from Earth, e.g. "1 1 e" → ["1", "1", "E"].</summary>
    private static string[] SplitIntoValues(string line) =>
        TrimAndUppercase(line).Split([' ', '\t'], StringSplitOptions.RemoveEmptyEntries);

    /// <summary>The square that two values name, or null when either isn't a number.</summary>
    private static Square? ReadSquare(string xValue, string yValue) =>
        TryReadDigits(xValue, out var x) && TryReadDigits(yValue, out var y) ? new Square(x, y) : null;

    // Digits only, the same on every machine: no sign, separators or local number formats.
    private static bool TryReadDigits(string value, out int number) =>
        int.TryParse(value, NumberStyles.None, CultureInfo.InvariantCulture, out number);
}
