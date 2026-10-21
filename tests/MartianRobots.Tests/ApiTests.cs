using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace MartianRobots.Tests;

public class ApiTests(WebApplicationFactory<Program> api) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Posting_the_brief_sample_returns_exactly_the_brief_result_lines()
    {
        var sample = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "brief-sample.txt"));

        var response = await PostMission(sample);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Equal("1 1 E\n3 3 N LOST\n2 3 S\n", await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task A_scent_left_in_one_request_does_not_protect_a_robot_in_the_next()
    {
        const string robotLostOffTheNorthEdge = "5 3\n3 2 N\nFF\n";

        await PostMission(robotLostOffTheNorthEdge);
        var response = await PostMission(robotLostOffTheNorthEdge);

        Assert.Equal("3 3 N LOST\n", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("5 X\n1 1 E\nF\n", "5 3")]
    [InlineData("5 3\n7 1 E\nF\n", "`0 0` and `5 3`")]
    [InlineData("5 3\n1 1 E\nFFZ\n", "`Z`")]
    public async Task A_bad_line_is_rejected_with_its_hint(string missionText, string hintFragment)
    {
        var response = await PostMission(missionText);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("text/plain", response.Content.Headers.ContentType?.MediaType);
        Assert.Contains(hintFragment, await response.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task A_robot_with_a_position_but_no_instructions_is_rejected_with_a_hint()
    {
        var response = await PostMission("5 3\n1 1 E\nRFRF\n3 2 N\n");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("3 2 N", await response.Content.ReadAsStringAsync());
    }

    [Theory]
    [InlineData("")]
    [InlineData("\n \n")]
    public async Task A_mission_without_the_size_of_Mars_is_rejected_with_a_hint(string missionText)
    {
        var response = await PostMission(missionText);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("5 3", await response.Content.ReadAsStringAsync());
    }

    private Task<HttpResponseMessage> PostMission(string missionText) =>
        api.CreateClient().PostAsync("/missions", new StringContent(missionText));
}
