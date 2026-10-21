using MartianRobots;

var mars = LineReaders.ReadMars(Console.ReadLine()!).Value!;
var mission = new Mission(mars);
while (Console.ReadLine() is { } positionLine && Console.ReadLine() is { } instructionsLine)
{
    var report = mission.Send(LineReaders.ReadPosition(positionLine, mars).Value!, LineReaders.ReadInstructions(instructionsLine).Value!);
    Console.WriteLine(report.ResultLine);
}
