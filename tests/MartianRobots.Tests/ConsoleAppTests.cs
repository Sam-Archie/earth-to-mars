using System.Diagnostics;

namespace MartianRobots.Tests;

public class ConsoleAppTests
{
    [Fact]
    public async Task Piping_the_brief_sample_writes_exactly_the_brief_result_lines_to_stdout()
    {
        var sample = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "brief-sample.txt"));
        var workingDirectory = Directory.CreateTempSubdirectory();
        try
        {
            var stdout = await RunConsoleApp(sample, workingDirectory.FullName);

            Assert.Equal("1 1 E\n3 3 N LOST\n2 3 S\n", stdout.ReplaceLineEndings("\n"));
        }
        finally
        {
            workingDirectory.Delete(recursive: true);
        }
    }

    private static async Task<string> RunConsoleApp(string stdin, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo("dotnet", [Path.Combine(AppContext.BaseDirectory, "MartianRobots.Cli.dll")])
        {
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            WorkingDirectory = workingDirectory,
        };
        using var process = Process.Start(startInfo)!;
        await process.StandardInput.WriteAsync(stdin);
        process.StandardInput.Close();
        var stdout = await process.StandardOutput.ReadToEndAsync();
        await process.WaitForExitAsync();
        return stdout;
    }
}
