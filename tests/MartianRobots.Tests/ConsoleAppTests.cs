using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MartianRobots.Tests;

public partial class ConsoleAppTests
{
    [Fact]
    public async Task Piping_the_brief_sample_writes_exactly_the_brief_result_lines_to_stdout()
    {
        var sample = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "brief-sample.txt"));

        var run = await RunConsoleApp(sample);

        Assert.Equal("1 1 E\n3 3 N LOST\n2 3 S\n", run.Stdout);
    }

    [Fact]
    public async Task Bad_lines_are_asked_for_again_and_only_the_result_reaches_stdout()
    {
        var run = await RunConsoleApp("5 X\n5 3\n7 1 E\n1 1 E\nFFZ\nLF\n");

        Assert.Equal("1 2 N\n", run.Stdout);
        Assert.Equal(0, run.ExitCode);
    }

    [Fact]
    public async Task Robot_left_without_instructions_when_input_ends_is_dropped_with_a_note()
    {
        var run = await RunConsoleApp("5 3\n2 3 N\n");

        Assert.Equal("", run.Stdout);
        Assert.Contains("2 3 N", run.Stderr);
        Assert.Equal(0, run.ExitCode);
    }

    [Fact]
    public async Task Each_robot_s_loss_or_claim_and_the_mission_s_completion_are_announced_on_stderr()
    {
        var run = await RunConsoleApp("1 1\n0 1 N\nF\n0 0 N\n\n0 1 N\n\n1 1 N\n\n1 0 N\n\n0 0 E\nF\n");

        Assert.Equal("0 1 N LOST\n0 0 N\n0 1 N\n1 1 N\n1 0 N\n1 0 E\n", run.Stdout);
        Assert.Contains("Robot 1 is lost in space", run.Stderr);
        Assert.Contains("Robot 2 claims 0 0", run.Stderr);
        Assert.Contains("Robot 6 reaches 1 0, but it is already claimed", run.Stderr);
        Assert.Contains("Mission complete", run.Stderr);
    }

    [Fact]
    public async Task Mission_map_is_written_to_stderr_after_every_robot_claimed_or_lost()
    {
        var sample = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "brief-sample.txt"));

        var run = await RunConsoleApp(sample);

        var mapsWritten = run.Stderr.Split("~ Earth's view of Mars ~").Length - 1;
        Assert.Equal(3, mapsWritten);
        Assert.Contains("3 ? ? 3 * ? ?", run.Stderr);
    }

    [Fact]
    public async Task Each_mission_appends_a_started_header_to_the_mission_log()
    {
        var workingDirectory = Directory.CreateTempSubdirectory();
        try
        {
            await RunConsoleAppIn(workingDirectory, "5 3\n");
            await RunConsoleAppIn(workingDirectory, "5 3\n");

            var missionLogLines = ReadMissionLogLines(workingDirectory);
            var missionHeaders = missionLogLines.Where(line => MissionHeader().IsMatch(line));
            Assert.Equal(2, missionHeaders.Count());
            Assert.Matches(MissionHeader(), missionLogLines[0]);
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
        }
    }

    [Fact]
    public async Task Every_line_Earth_sends_is_logged_bad_lines_included()
    {
        var run = await RunConsoleApp("5 X\n5 3\n9 9 N\n");

        Assert.Equal(["< 5 X", "< 5 3", "< 9 9 N"], run.MissionLogEntries);
    }

    [Fact]
    public async Task Result_lines_are_logged_and_prompts_messages_and_the_map_are_not()
    {
        var run = await RunConsoleApp("5 3\n1 1 E\nRFRFRFRF\n3 2 N\nFRRFLLFFRRFLL\n");

        Assert.Equal(["< 5 3", "< 1 1 E", "< RFRFRFRF", "> 1 1 E", "< 3 2 N", "< FRRFLLFFRRFLL", "> 3 3 N LOST"],
            run.MissionLogEntries);
    }

    [Fact]
    public async Task Each_entry_reaches_the_mission_log_while_the_mission_is_still_running()
    {
        var workingDirectory = Directory.CreateTempSubdirectory();
        try
        {
            using var process = StartConsoleAppIn(workingDirectory);
            await process.StandardInput.WriteLineAsync("5 3");
            await process.StandardInput.FlushAsync();

            var entryLogged = await WaitUntil(() => ReadMissionLogWhileOpen(workingDirectory).Contains("< 5 3"));

            process.StandardInput.Close();
            await process.WaitForExitAsync();
            Assert.True(entryLogged);
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
        }
    }

    private static async Task<bool> WaitUntil(Func<bool> condition)
    {
        var giveUpAt = DateTime.UtcNow.AddSeconds(10);
        while (DateTime.UtcNow < giveUpAt)
        {
            if (condition())
            {
                return true;
            }

            await Task.Delay(50);
        }

        return false;
    }

    // The running app holds the log open, so read it without asking for sole access.
    private static string ReadMissionLogWhileOpen(DirectoryInfo workingDirectory)
    {
        var missionLogPath = MissionLogPath(workingDirectory);
        if (!File.Exists(missionLogPath))
        {
            return "";
        }

        using var missionLog = new FileStream(missionLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        return new StreamReader(missionLog).ReadToEnd();
    }

    [GeneratedRegex(@"^\d{2}:\d{2}:\d{2} --- Mission started \d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2} ---$")]
    private static partial Regex MissionHeader();

    [GeneratedRegex(@"^\d{2}:\d{2}:\d{2} ")]
    private static partial Regex EntryTime();

    private static string[] ReadMissionLogLines(DirectoryInfo workingDirectory) =>
        File.ReadAllLines(MissionLogPath(workingDirectory));

    private static string MissionLogPath(DirectoryInfo workingDirectory) =>
        Path.Combine(workingDirectory.FullName, "logs", "missions.txt");

    // The latest mission's lines follow its header. Each must start with its time; the entry is what follows it.
    private static string[] ReadMissionLogEntries(DirectoryInfo workingDirectory)
    {
        var missionLogLines = ReadMissionLogLines(workingDirectory);
        var latestHeaderIndex = Array.FindLastIndex(missionLogLines, line => MissionHeader().IsMatch(line));
        var entryLines = missionLogLines[(latestHeaderIndex + 1)..];
        Assert.All(entryLines, line => Assert.Matches(EntryTime(), line));
        return entryLines.Select(line => EntryTime().Replace(line, "")).ToArray();
    }

    private sealed record ConsoleRun(string Stdout, string Stderr, int ExitCode, string[] MissionLogEntries);

    private static async Task<ConsoleRun> RunConsoleApp(string stdin)
    {
        var workingDirectory = Directory.CreateTempSubdirectory();
        try
        {
            return await RunConsoleAppIn(workingDirectory, stdin);
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
        }
    }

    private static async Task<ConsoleRun> RunConsoleAppIn(DirectoryInfo workingDirectory, string stdin)
    {
        using var process = StartConsoleAppIn(workingDirectory);
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        await process.StandardInput.WriteAsync(stdin);
        process.StandardInput.Close();
        await process.WaitForExitAsync();
        return new ConsoleRun((await stdout).ReplaceLineEndings("\n"), await stderr, process.ExitCode,
            ReadMissionLogEntries(workingDirectory));
    }

    private static Process StartConsoleAppIn(DirectoryInfo workingDirectory)
    {
        var startInfo = new ProcessStartInfo("dotnet", [Path.Combine(AppContext.BaseDirectory, "console-app", "MartianRobots.Cli.dll")])
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            WorkingDirectory = workingDirectory.FullName,
        };
        return Process.Start(startInfo)!;
    }
}
