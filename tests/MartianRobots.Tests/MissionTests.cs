namespace MartianRobots.Tests;

public class MissionTests
{
    private static Mission StartMission(string marsLine) => new(LineReaders.ReadMars(marsLine));

    private static RobotReport Send(Mission mission, string positionLine, string instructionsLine) =>
        mission.Send(LineReaders.ReadPosition(positionLine), LineReaders.ReadInstructions(instructionsLine));

    [Fact]
    public void Brief_sample_reports_each_robot_as_the_brief_does()
    {
        var mission = StartMission("5 3");

        var first = Send(mission, "1 1 E", "RFRFRFRF");
        var second = Send(mission, "3 2 N", "FRRFLLFFRRFLL");
        var third = Send(mission, "0 3 W", "LLFFFLFLFL");

        Assert.Equal("1 1 E", first.ResultLine);
        Assert.Equal("3 3 N LOST", second.ResultLine);
        Assert.Equal("2 3 S", third.ResultLine);
    }

    [Fact]
    public void Robot_sent_no_instructions_reports_its_starting_position()
    {
        var mission = StartMission("5 3");

        var report = Send(mission, "1 1 E", "");

        Assert.Equal("1 1 E", report.ResultLine);
        Assert.False(report.Lost);
    }

    [Theory]
    [InlineData("1 1 N", "1 1 W")]
    [InlineData("1 1 W", "1 1 S")]
    [InlineData("1 1 S", "1 1 E")]
    [InlineData("1 1 E", "1 1 N")]
    public void Left_turns_a_robot_90_degrees_on_its_square(string start, string expected)
    {
        var report = Send(StartMission("5 3"), start, "L");

        Assert.Equal(expected, report.ResultLine);
    }

    [Theory]
    [InlineData("1 1 N", "1 1 E")]
    [InlineData("1 1 E", "1 1 S")]
    [InlineData("1 1 S", "1 1 W")]
    [InlineData("1 1 W", "1 1 N")]
    public void Right_turns_a_robot_90_degrees_on_its_square(string start, string expected)
    {
        var report = Send(StartMission("5 3"), start, "R");

        Assert.Equal(expected, report.ResultLine);
    }

    [Theory]
    [InlineData("2 2 N", "2 3 N")]
    [InlineData("2 2 E", "3 2 E")]
    [InlineData("2 2 S", "2 1 S")]
    [InlineData("2 2 W", "1 2 W")]
    public void Forward_moves_a_robot_one_square_the_way_it_faces(string start, string expected)
    {
        var report = Send(StartMission("5 3"), start, "F");

        Assert.Equal(expected, report.ResultLine);
    }

    [Fact]
    public void Robot_moving_off_Mars_is_lost_at_its_last_square_and_skips_its_remaining_commands()
    {
        var report = Send(StartMission("5 3"), "3 2 N", "FFRR");

        Assert.Equal("3 3 N LOST", report.ResultLine);
        Assert.True(report.Lost);
    }

    [Fact]
    public void Robot_starting_on_a_scented_square_ignores_a_move_off_Mars()
    {
        var mission = StartMission("5 3");

        var first = Send(mission, "2 3 N", "F");
        var second = Send(mission, "2 3 N", "F");

        Assert.Equal("2 3 N LOST", first.ResultLine);
        Assert.Equal("2 3 N", second.ResultLine);
        Assert.False(second.Lost);
    }

    [Fact]
    public void Scent_on_a_corner_stops_a_move_off_Mars_over_either_edge()
    {
        var mission = StartMission("5 3");

        var first = Send(mission, "5 3 N", "F");
        var second = Send(mission, "5 3 E", "F");

        Assert.Equal("5 3 N LOST", first.ResultLine);
        Assert.Equal("5 3 E", second.ResultLine);
    }
}
