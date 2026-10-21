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

    private Task<HttpResponseMessage> PostMission(string missionText) =>
        api.CreateClient().PostAsync("/missions", new StringContent(missionText));
}
