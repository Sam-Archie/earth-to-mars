namespace MartianRobots;

/// <summary>Draws Earth's picture of Mars as ASCII text, north at the top and `0 0` at the bottom left.</summary>
internal static class MissionMap
{
    private const string UnknownRegion = "?";
    private const string Scent = "*";

    public static string Render(Mars mars, IReadOnlyDictionary<Square, int> claimingRobotNumbers, IReadOnlySet<Square> scents)
    {
        var yLabelWidth = mars.UpperRight.Y.ToString().Length;
        var axisIndent = new string(' ', yLabelWidth + 1);
        var xLabels = Enumerable.Range(0, mars.UpperRight.X + 1).ToList();

        var mapLines = new List<string> { $"{axisIndent}N", $"{axisIndent}^   ~ Earth's view of Mars ~" };
        for (var y = mars.UpperRight.Y; y >= 0; y--)
        {
            var rowSymbols = xLabels.Select(x => SymbolFor(new Square(x, y), claimingRobotNumbers, scents));
            mapLines.Add($"{y.ToString().PadLeft(yLabelWidth)} {string.Join(' ', rowSymbols)}");
        }

        mapLines.Add($"{axisIndent}{string.Join(' ', xLabels)}");
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
