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
