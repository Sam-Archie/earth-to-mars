using MartianRobots;

var mars = AskUntilRead("Earth to Mars, come in. How big is Mars? Send its upper-right square, like `5 3`.", LineReaders.ReadMars);
if (mars is null)
{
    return;
}

var mission = new Mission(mars);
for (var robotNumber = 1; ; robotNumber++)
{
    var startPosition = AskUntilRead($"Robot {robotNumber}, where do you land? Send a square and an orientation, like `1 1 E`.",
        line => LineReaders.ReadPosition(line, mars));
    if (startPosition is null)
    {
        return;
    }

    var instructions = AskUntilRead($"Robot {robotNumber}, standing by for instructions, like `RFRF`.", LineReaders.ReadInstructions);
    if (instructions is null)
    {
        Console.Error.WriteLine($"Signal lost before robot {robotNumber} at {startPosition} got its instructions. It stays in the hangar.");
        return;
    }

    Console.WriteLine(mission.Send(startPosition, instructions).ResultLine);
}

// Prompts and hints go to stderr so stdout carries only the brief's result lines. Null means Earth stopped transmitting.
static T? AskUntilRead<T>(string prompt, Func<string, LineRead<T>> readLine) where T : class
{
    while (true)
    {
        Console.Error.WriteLine(prompt);
        if (Console.ReadLine() is not { } line)
        {
            return null;
        }

        var attempt = readLine(line);
        if (attempt.Value is { } value)
        {
            return value;
        }

        Console.Error.WriteLine(attempt.Hint);
    }
}
