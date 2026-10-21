namespace MartianRobots.Tests;

public class LineReaderTests
{
    [Theory]
    [InlineData("5 X")]
    [InlineData("")]
    public void Malformed_size_of_Mars_gets_a_hint_instead_of_a_value(string line)
    {
        var read = LineReaders.ReadMars(line);

        Assert.Null(read.Value);
        Assert.Contains("5 3", read.Hint);
    }

    [Fact]
    public void Size_of_Mars_with_a_coordinate_over_50_gets_a_hint()
    {
        var read = LineReaders.ReadMars("99 3");

        Assert.Null(read.Value);
        Assert.Contains("50", read.Hint);
    }

    [Fact]
    public void Starting_position_off_Mars_gets_a_hint_naming_the_squares_on_Mars()
    {
        var read = LineReaders.ReadPosition("7 1 E", Mars5By3);

        Assert.Null(read.Value);
        Assert.Contains("5 3", read.Hint);
    }

    private static readonly Mars Mars5By3 = LineReaders.ReadMars("5 3").Value!;
}
