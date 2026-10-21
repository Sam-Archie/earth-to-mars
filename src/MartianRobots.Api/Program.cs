using MartianRobots;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapPost("/missions", async (HttpRequest request) =>
{
    var missionText = await new StreamReader(request.Body).ReadToEndAsync();
    var earthLines = missionText.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

    var mars = LineReaders.ReadMars(earthLines[0]).Value!;
    var mission = new Mission(mars);
    var resultLines = earthLines[1..].Chunk(2).Select(robotLines =>
    {
        var startPosition = LineReaders.ReadPosition(robotLines[0], mars).Value!;
        var instructions = LineReaders.ReadInstructions(robotLines[1]).Value!;
        return mission.Send(startPosition, instructions).ResultLine + "\n";
    });

    return Results.Text(string.Concat(resultLines));
});

app.Run();
