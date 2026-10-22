using MartianRobots.Cli;

namespace MartianRobots.Tests;

public class HighlightTests
{
    [Fact]
    public void A_line_for_a_terminal_is_wrapped_in_its_colour()
    {
        var terminal = new StringWriter();

        Highlight.WriteLine(terminal, writerIsTerminal: true, "3 3 N LOST", Colour.Red);

        Assert.Equal("\u001b[31m3 3 N LOST\u001b[0m" + Environment.NewLine, terminal.ToString());
    }

    [Fact]
    public void A_piped_or_redirected_line_stays_plain()
    {
        var pipe = new StringWriter();

        Highlight.WriteLine(pipe, writerIsTerminal: false, "3 3 N LOST", Colour.Red);

        Assert.Equal("3 3 N LOST" + Environment.NewLine, pipe.ToString());
    }

    [Fact]
    public void Only_the_scents_on_a_map_for_a_terminal_turn_red()
    {
        var terminal = new StringWriter();

        Highlight.WriteMap(terminal, writerIsTerminal: true, "3     3 *\n  0 1 2 3");

        Assert.Equal("3     3 \u001b[31m*\u001b[0m\n  0 1 2 3" + Environment.NewLine, terminal.ToString());
    }

    [Fact]
    public void A_piped_or_redirected_map_stays_plain()
    {
        var pipe = new StringWriter();

        Highlight.WriteMap(pipe, writerIsTerminal: false, "3     3 *\n  0 1 2 3");

        Assert.Equal("3     3 *\n  0 1 2 3" + Environment.NewLine, pipe.ToString());
    }
}
