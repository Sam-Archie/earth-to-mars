namespace MartianRobots.Tests;

public class MissionTests
{
    private static Mission StartMission(string marsLine) => new(LineReaders.ReadMars(marsLine));

    private static RobotReport Send(Mission mission, string positionLine, string instructionsLine) =>
        mission.Send(LineReaders.ReadPosition(positionLine), LineReaders.ReadInstructions(instructionsLine));

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
}
