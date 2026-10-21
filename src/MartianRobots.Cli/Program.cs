using MartianRobots;

var mars = Ask("Earth to Mars, come in. How big is Mars? Send its upper-right square, like `5 3`.", LineReaders.ReadMars);
if (mars is null)
{
    return;
}

var mission = new Mission(mars);
for (var robot = 1; ; robot++)
{
    var start = Ask($"Robot {robot}, where do you land? Send a square and an orientation, like `1 1 E`.",
        line => LineReaders.ReadPosition(line, mars));
    if (start is null)
    {
        return;
    }

    var instructions = Ask($"Robot {robot}, standing by for instructions, like `RFRF`.", LineReaders.ReadInstructions);
    if (instructions is null)
    {
        Console.Error.WriteLine($"Signal lost before robot {robot} at {start} got its instructions. It stays in the hangar.");
        return;
    }

    Console.WriteLine(mission.Send(start, instructions).ResultLine);
}

// Prompts and hints go to stderr so stdout carries only the brief's result lines. Null means Earth stopped transmitting.
static T? Ask<T>(string prompt, Func<string, LineRead<T>> read) where T : class
{
    while (true)
    {
        Console.Error.WriteLine(prompt);
        if (Console.ReadLine() is not { } line)
        {
            return null;
        }

        var lineRead = read(line);
        if (lineRead.Value is { } value)
        {
            return value;
        }

        Console.Error.WriteLine(lineRead.Hint);
    }
}
