using MartianRobots;

var mission = new Mission(LineReaders.ReadMars(Console.ReadLine()!));
while (Console.ReadLine() is { } positionLine && Console.ReadLine() is { } instructionsLine)
{
    var report = mission.Send(LineReaders.ReadPosition(positionLine), LineReaders.ReadInstructions(instructionsLine));
    Console.WriteLine(report.ResultLine);
}
