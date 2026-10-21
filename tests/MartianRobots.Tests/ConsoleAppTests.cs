using System.Diagnostics;

namespace MartianRobots.Tests;

public class ConsoleAppTests
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

    private sealed record ConsoleRun(string Stdout, string Stderr, int ExitCode);

    private static async Task<ConsoleRun> RunConsoleApp(string stdin)
    {
        var workingDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var startInfo = new ProcessStartInfo("dotnet", [Path.Combine(AppContext.BaseDirectory, "MartianRobots.Cli.dll")])
            {
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                WorkingDirectory = workingDirectory.FullName,
            };
            using var process = Process.Start(startInfo)!;
            var stdout = process.StandardOutput.ReadToEndAsync();
            var stderr = process.StandardError.ReadToEndAsync();
            await process.StandardInput.WriteAsync(stdin);
            process.StandardInput.Close();
            await process.WaitForExitAsync();
            return new ConsoleRun((await stdout).ReplaceLineEndings("\n"), await stderr, process.ExitCode);
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
        }
    }
}
