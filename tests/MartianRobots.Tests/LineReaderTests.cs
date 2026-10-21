using MartianRobots.Domain.Planets;
using MartianRobots.Input;

namespace MartianRobots.Tests;

public class LineReaderTests
{
    [Theory]
    [InlineData("5 X")]
    [InlineData("+5 3")]
    [InlineData("")]
    public void Malformed_size_of_Mars_gets_a_hint_instead_of_a_value(string line)
    {
        var read = LineReaders.ReadMars(line);

        Assert.Null(read.Value);
        Assert.Contains("5 3", read.Hint);
    }

    [Fact]
    public void Size_of_Mars_with_an_x_or_y_over_50_gets_a_hint()
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

    [Fact]
    public void Starting_position_with_an_unknown_orientation_gets_a_hint_naming_the_four()
    {
        var read = LineReaders.ReadPosition("1 1 Q", Mars5By3);

        Assert.Null(read.Value);
        Assert.Contains("N, E, S or W", read.Hint);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1 1")]
    [InlineData("1 X E")]
    public void Malformed_starting_position_gets_a_hint_showing_its_shape(string line)
    {
        var read = LineReaders.ReadPosition(line, Mars5By3);

        Assert.Null(read.Value);
        Assert.Contains("1 1 E", read.Hint);
    }

    [Fact]
    public void Instructions_with_an_unknown_command_letter_get_a_hint_naming_that_letter()
    {
        var read = LineReaders.ReadInstructions("FFZ");

        Assert.Null(read.Value);
        Assert.Contains("`Z`", read.Hint);
    }

    [Fact]
    public void Instructions_of_100_letters_get_a_hint_to_send_fewer()
    {
        var read = LineReaders.ReadInstructions(new string('F', 100));

        Assert.Null(read.Value);
        Assert.Contains("100", read.Hint);
    }

    private static readonly Mars Mars5By3 = LineReaders.ReadMars("5 3").Value!;
}
