namespace MartianRobots;

/// <summary>Draws Earth's picture of Mars as ASCII text, north at the top and `0 0` at the bottom left.</summary>
internal static class MissionMap
{
    private const string UnknownRegion = "?";
    private const string Scent = "*";

    public static string Render(Mars mars, IReadOnlyDictionary<Square, int> claimingRobotNumbers, IReadOnlySet<Square> scents)
    {
        var yLabelWidth = mars.UpperRight.Y.ToString().Length;
        // A larger number never has fewer digits, so the widest label is the largest number shown.
        var largestNumberShown = claimingRobotNumbers.Values.Append(mars.UpperRight.X).Max();
        var symbolWidth = largestNumberShown.ToString().Length;
        var xFromWestToEast = Enumerable.Range(0, mars.UpperRight.X + 1).ToList();

        string MapLine(string yLabel, IEnumerable<string> symbols) =>
            $"{yLabel.PadLeft(yLabelWidth)} {string.Join(' ', symbols.Select(symbol => symbol.PadLeft(symbolWidth)))}";

        var mapLines = new List<string> { MapLine("", ["N"]), MapLine("", ["^"]) + "   ~ Earth's view of Mars ~" };
        for (var squareY = mars.UpperRight.Y; squareY >= 0; squareY--)
        {
            var symbolsWestToEast = xFromWestToEast.Select(squareX => SymbolFor(new Square(squareX, squareY), claimingRobotNumbers, scents));
            mapLines.Add(MapLine(squareY.ToString(), symbolsWestToEast));
        }

        mapLines.Add(MapLine("", xFromWestToEast.Select(squareX => squareX.ToString())));
        return string.Join('\n', mapLines);
    }

    private static string SymbolFor(Square square, IReadOnlyDictionary<Square, int> claimingRobotNumbers, IReadOnlySet<Square> scents)
    {
        if (claimingRobotNumbers.TryGetValue(square, out var robotNumber))
        {
            return robotNumber.ToString();
        }

        return scents.Contains(square) ? Scent : UnknownRegion;
    }
}
