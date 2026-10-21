namespace MartianRobots.Tests;

public class LineReaderTests
{
    [Fact]
    public void Malformed_size_of_Mars_gets_a_hint_instead_of_a_value()
    {
        var read = LineReaders.ReadMars("5 X");

        Assert.Null(read.Value);
        Assert.Contains("5 3", read.Hint);
    }
}
