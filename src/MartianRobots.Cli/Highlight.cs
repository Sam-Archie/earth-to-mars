using MartianRobots.Map;

namespace MartianRobots.Cli;

/// <summary>
/// Colours what Earth reads at a terminal. Piped or redirected text stays plain,
/// so stdout is still exactly the brief's result lines wherever it is captured.
/// </summary>
public static class Highlight
{
    private const string Reset = "\u001b[0m";

    public static void Result(string resultLine, Colour colour) =>
        WriteLine(Console.Out, !Console.IsOutputRedirected, resultLine, colour);

    public static void Message(string message, Colour colour) =>
        WriteLine(Console.Error, !Console.IsErrorRedirected, message, colour);

    public static void Map(string missionMap) =>
        WriteMap(Console.Error, !Console.IsErrorRedirected, missionMap);

    public static void WriteLine(TextWriter writer, bool writerIsTerminal, string line, Colour colour) =>
        writer.WriteLine(writerIsTerminal ? Paint(line, colour) : line);

    /// <summary>Only scents are coloured: robot numbers share their digits with the map's labels.</summary>
    public static void WriteMap(TextWriter writer, bool writerIsTerminal, string missionMap) =>
        writer.WriteLine(writerIsTerminal ? missionMap.Replace(MapSymbols.Scent, Paint(MapSymbols.Scent, Colour.Red)) : missionMap);

    private static string Paint(string text, Colour colour) => $"\u001b[{AnsiCode(colour)}m{text}{Reset}";

    private static int AnsiCode(Colour colour) => colour switch
    {
        Colour.Green => 32,
        Colour.Red => 31,
        Colour.Yellow => 33,
        Colour.Cyan => 36,
        Colour.Magenta => 35,
        _ => throw new ArgumentOutOfRangeException(nameof(colour), colour, "No terminal colour code for this colour."),
    };
}
