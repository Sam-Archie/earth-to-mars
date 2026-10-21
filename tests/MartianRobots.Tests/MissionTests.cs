namespace MartianRobots.Tests;

public class MissionTests
{
    private static MissionFromLines StartMission(string marsLine) => new(LineReaders.ReadMars(marsLine).Value!);

    /// <summary>A mission fed Earth's lines through the line readers, as the adapters feed it.</summary>
    private sealed class MissionFromLines(Mars mars)
    {
        private readonly Mission _mission = new(mars);

        public RobotReport Send(string positionLine, string instructionsLine) =>
            _mission.Send(LineReaders.ReadPosition(positionLine, mars).Value!, LineReaders.ReadInstructions(instructionsLine).Value!);

        public string RenderMap() => _mission.RenderMap();
    }

    [Fact]
    public void Brief_sample_reports_each_robot_as_the_brief_does()
    {
        var mission = StartMission("5 3");

        var first = mission.Send("1 1 E", "RFRFRFRF");
        var second = mission.Send("3 2 N", "FRRFLLFFRRFLL");
        var third = mission.Send("0 3 W", "LLFFFLFLFL");

        Assert.Equal("1 1 E", first.ResultLine);
        Assert.Equal("3 3 N LOST", second.ResultLine);
        Assert.Equal("2 3 S", third.ResultLine);
    }

    [Fact]
    public void Robot_sent_no_instructions_reports_its_starting_position()
    {
        var mission = StartMission("5 3");

        var report = mission.Send("1 1 E", "");

        Assert.Equal("1 1 E", report.ResultLine);
        Assert.False(report.Lost);
    }

    [Fact]
    public void Lowercase_lines_from_Earth_are_read_as_uppercase()
    {
        var report = StartMission("5 3").Send("1 1 e", "rff");

        Assert.Equal("1 0 S LOST", report.ResultLine);
    }

    [Fact]
    public void Extra_spaces_tabs_and_carriage_returns_in_lines_from_Earth_are_ignored()
    {
        var report = StartMission(" 5\t 3 \r").Send("\t1  1 E\r", " RFF \r");

        Assert.Equal("1 0 S LOST", report.ResultLine);
    }

    [Theory]
    [InlineData("1 1 N", "1 1 W")]
    [InlineData("1 1 W", "1 1 S")]
    [InlineData("1 1 S", "1 1 E")]
    [InlineData("1 1 E", "1 1 N")]
    public void Left_turns_a_robot_90_degrees_on_its_square(string start, string expected)
    {
        var report = StartMission("5 3").Send(start, "L");

        Assert.Equal(expected, report.ResultLine);
    }

    [Theory]
    [InlineData("1 1 N", "1 1 E")]
    [InlineData("1 1 E", "1 1 S")]
    [InlineData("1 1 S", "1 1 W")]
    [InlineData("1 1 W", "1 1 N")]
    public void Right_turns_a_robot_90_degrees_on_its_square(string start, string expected)
    {
        var report = StartMission("5 3").Send(start, "R");

        Assert.Equal(expected, report.ResultLine);
    }

    [Theory]
    [InlineData("2 2 N", "2 3 N")]
    [InlineData("2 2 E", "3 2 E")]
    [InlineData("2 2 S", "2 1 S")]
    [InlineData("2 2 W", "1 2 W")]
    public void Forward_moves_a_robot_one_square_the_way_it_faces(string start, string expected)
    {
        var report = StartMission("5 3").Send(start, "F");

        Assert.Equal(expected, report.ResultLine);
    }

    [Fact]
    public void Robot_moving_off_Mars_is_lost_at_its_last_square_and_skips_its_remaining_commands()
    {
        var report = StartMission("5 3").Send("3 2 N", "FFRR");

        Assert.Equal("3 3 N LOST", report.ResultLine);
        Assert.True(report.Lost);
    }

    [Fact]
    public void Robot_starting_on_a_scented_square_ignores_a_move_off_Mars()
    {
        var mission = StartMission("5 3");

        var first = mission.Send("2 3 N", "F");
        var second = mission.Send("2 3 N", "F");

        Assert.Equal("2 3 N LOST", first.ResultLine);
        Assert.Equal("2 3 N", second.ResultLine);
        Assert.False(second.Lost);
    }

    [Fact]
    public void Scent_on_a_corner_stops_a_move_off_Mars_over_either_edge()
    {
        var mission = StartMission("5 3");

        var first = mission.Send("5 3 N", "F");
        var second = mission.Send("5 3 E", "F");

        Assert.Equal("5 3 N LOST", first.ResultLine);
        Assert.Equal("5 3 E", second.ResultLine);
    }

    [Fact]
    public void Robot_finishing_on_Mars_claims_its_square()
    {
        var report = StartMission("5 3").Send("1 1 E", "F");

        Assert.Equal("2 1 E", report.ResultLine);
        Assert.Equal(ClaimOutcome.Claimed, report.Claim);
    }

    [Fact]
    public void Lost_robot_claims_nothing()
    {
        var report = StartMission("5 3").Send("3 2 N", "FF");

        Assert.Equal("3 3 N LOST", report.ResultLine);
        Assert.Equal(ClaimOutcome.None, report.Claim);
    }

    [Fact]
    public void Robot_finishing_on_a_claimed_square_gets_its_result_line_but_finds_it_already_claimed()
    {
        var mission = StartMission("5 3");

        mission.Send("1 1 E", "F");
        var second = mission.Send("3 1 W", "F");

        Assert.Equal("2 1 W", second.ResultLine);
        Assert.Equal(ClaimOutcome.AlreadyClaimed, second.Claim);
    }

    [Fact]
    public void Robot_passes_through_a_claimed_square_and_claims_where_it_finishes()
    {
        var mission = StartMission("5 3");

        mission.Send("1 1 E", "F");
        var second = mission.Send("1 1 E", "FF");

        Assert.Equal("3 1 E", second.ResultLine);
        Assert.Equal(ClaimOutcome.Claimed, second.Claim);
    }

    [Fact]
    public void Robot_sent_no_instructions_claims_its_starting_square()
    {
        var report = StartMission("5 3").Send("1 1 E", "");

        Assert.Equal("1 1 E", report.ResultLine);
        Assert.Equal(ClaimOutcome.Claimed, report.Claim);
    }

    [Fact]
    public void Robot_claiming_the_last_unclaimed_square_completes_the_mission_and_later_robots_still_run()
    {
        var mission = StartMission("1 1");

        var first = mission.Send("0 0 N", "");
        var second = mission.Send("0 1 N", "");
        var third = mission.Send("1 1 N", "");
        var fourth = mission.Send("1 0 N", "");
        var fifth = mission.Send("0 0 E", "F");

        Assert.All([first, second, third], report =>
        {
            Assert.Equal(ClaimOutcome.Claimed, report.Claim);
            Assert.False(report.CompletedMission);
        });
        Assert.Equal(ClaimOutcome.Claimed, fourth.Claim);
        Assert.True(fourth.CompletedMission);
        Assert.Equal("1 0 E", fifth.ResultLine);
        Assert.Equal(ClaimOutcome.AlreadyClaimed, fifth.Claim);
        Assert.False(fifth.CompletedMission);
    }

    [Fact]
    public void Map_after_the_brief_sample_shows_each_claim_by_robot_number_the_scent_and_unknown_regions()
    {
        var mission = StartMission("5 3");
        mission.Send("1 1 E", "RFRFRFRF");
        mission.Send("3 2 N", "FRRFLLFFRRFLL");
        mission.Send("0 3 W", "LLFFFLFLFL");

        Assert.Equal("""
              N
              ^   ~ Earth's view of Mars ~
            3 ? ? 3 * ? ?
            2 ? ? ? ? ? ?
            1 ? 1 ? ? ? ?
            0 ? ? ? ? ? ?
              0 1 2 3 4 5
            """, mission.RenderMap());
    }

    [Fact]
    public void Map_shows_a_claim_over_a_scent_on_the_same_square()
    {
        var mission = StartMission("2 1");
        mission.Send("1 1 N", "F");
        mission.Send("1 0 N", "F");

        Assert.Equal("""
              N
              ^   ~ Earth's view of Mars ~
            1 ? 2 ?
            0 ? ? ?
              0 1 2
            """, mission.RenderMap());
    }

    [Fact]
    public void Map_columns_widen_to_fit_robot_10_and_stay_aligned()
    {
        var mission = StartMission("3 0");
        mission.Send("0 0 N", "F");
        for (var robotNumber = 2; robotNumber <= 9; robotNumber++)
        {
            mission.Send("0 0 N", "F");
        }

        mission.Send("2 0 N", "");

        Assert.Equal("""
               N
               ^   ~ Earth's view of Mars ~
            0  2  ? 10  ?
               0  1  2  3
            """, mission.RenderMap());
    }

    [Fact]
    public void Map_columns_widen_to_fit_x_labels_past_9_and_stay_aligned()
    {
        var mission = StartMission("10 0");
        mission.Send("0 0 N", "");

        Assert.Equal("""
               N
               ^   ~ Earth's view of Mars ~
            0  1  ?  ?  ?  ?  ?  ?  ?  ?  ?  ?
               0  1  2  3  4  5  6  7  8  9 10
            """, mission.RenderMap());
    }

    [Fact]
    public void Scents_and_claims_last_for_one_mission_only()
    {
        var firstMission = StartMission("5 3");
        firstMission.Send("2 3 N", "F");
        firstMission.Send("1 1 E", "");

        var nextMission = StartMission("5 3");
        var lostAgain = nextMission.Send("2 3 N", "F");
        var claimedAgain = nextMission.Send("1 1 E", "");

        Assert.Equal("2 3 N LOST", lostAgain.ResultLine);
        Assert.Equal(ClaimOutcome.Claimed, claimedAgain.Claim);
    }
}
