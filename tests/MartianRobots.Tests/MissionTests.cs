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
}
