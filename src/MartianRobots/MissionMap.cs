namespace MartianRobots;

/// <summary>Draws Earth's picture of Mars as ASCII text, north at the top and `0 0` at the bottom left.</summary>
internal static class MissionMap
{
    private const string UnknownRegion = "?";
    private const string Scent = "*";

    public static string Render(Mars mars, IReadOnlyDictionary<Square, int> claimingRobotNumbers, IReadOnlySet<Square> scents)
    {
        var yLabelWidth = mars.UpperRight.Y.ToString().Length;
        var highestRobotNumberShown = claimingRobotNumbers.Values.DefaultIfEmpty(0).Max();
        var columnWidth = Math.Max(mars.UpperRight.X.ToString().Length, highestRobotNumberShown.ToString().Length);
        var northMarkerIndent = new string(' ', yLabelWidth + columnWidth);
        var xFromWestToEast = Enumerable.Range(0, mars.UpperRight.X + 1).ToList();

        var mapLines = new List<string> { $"{northMarkerIndent}N", $"{northMarkerIndent}^   ~ Earth's view of Mars ~" };
        for (var y = mars.UpperRight.Y; y >= 0; y--)
        {
            var rowSymbols = xFromWestToEast.Select(x => SymbolFor(new Square(x, y), claimingRobotNumbers, scents));
            mapLines.Add($"{y.ToString().PadLeft(yLabelWidth)} {JoinIntoColumns(rowSymbols, columnWidth)}");
        }

        mapLines.Add($"{new string(' ', yLabelWidth)} {JoinIntoColumns(xFromWestToEast.Select(x => x.ToString()), columnWidth)}");
        return string.Join('\n', mapLines);
    }

    private static string JoinIntoColumns(IEnumerable<string> symbols, int columnWidth) =>
        string.Join(' ', symbols.Select(symbol => symbol.PadLeft(columnWidth)));

    private static string SymbolFor(Square square, IReadOnlyDictionary<Square, int> claimingRobotNumbers, IReadOnlySet<Square> scents)
    {
        if (claimingRobotNumbers.TryGetValue(square, out var robotNumber))
        {
            return robotNumber.ToString();
        }

        return scents.Contains(square) ? Scent : UnknownRegion;
    }
}
