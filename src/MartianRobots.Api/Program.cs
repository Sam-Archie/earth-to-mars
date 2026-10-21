using MartianRobots;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

// The API can't ask Earth for a line again, so the first bad line rejects the whole mission with its hint.
app.MapPost("/missions", async (HttpRequest request) =>
{
    var missionText = await new StreamReader(request.Body).ReadToEndAsync();
    var earthLines = missionText.Split('\n').Where(line => !string.IsNullOrWhiteSpace(line)).ToArray();

    var marsRead = LineReaders.ReadMars(earthLines.FirstOrDefault() ?? "");
    if (marsRead.Value is not { } mars)
    {
        return Results.Text(marsRead.Hint, statusCode: StatusCodes.Status400BadRequest);
    }

    var mission = new Mission(mars);
    var resultLines = new List<string>();
    foreach (var robotLines in earthLines[1..].Chunk(2))
    {
        var startPositionRead = LineReaders.ReadPosition(robotLines[0], mars);
        if (startPositionRead.Value is not { } startPosition)
        {
            return Results.Text(startPositionRead.Hint, statusCode: StatusCodes.Status400BadRequest);
        }

        if (robotLines is not [_, var instructionsLine])
        {
            return Results.Text(
                $"Robot at `{startPosition}` landed without instructions. Each robot is two lines: a position like `1 1 E`, then instructions like `RFRF`.",
                statusCode: StatusCodes.Status400BadRequest);
        }

        var instructionsRead = LineReaders.ReadInstructions(instructionsLine);
        if (instructionsRead.Value is not { } instructions)
        {
            return Results.Text(instructionsRead.Hint, statusCode: StatusCodes.Status400BadRequest);
        }

        resultLines.Add(mission.Send(startPosition, instructions).ResultLine + "\n");
    }

    return Results.Text(string.Concat(resultLines));
});

app.Run();
