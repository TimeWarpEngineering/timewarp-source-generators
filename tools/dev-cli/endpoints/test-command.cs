#region Purpose
// Run the test suite
#endregion
#region Design
// Builds and runs test-console then the TW0007 analyzer-tests console in Release
// Fails on non-zero exit from build or run
// Handler stores Command and Ct as fields so private methods are zero-parameter
// Streams output via Amuru RunAsync by default; --quiet uses CaptureAsync
// --coverage restores the local dotnet-coverage tool and wraps each console run
// Each covered run writes one Cobertura file under artifacts/coverage
// The tool targets net8.0. DOTNET_ROLL_FORWARD=Major lets it start on the 11.0.x SDK
// A plain test run does not collect. The workflow test step sets Coverage
#endregion

namespace DevCli.Commands;

[NuruRoute("test", Description = "Run the test suite")]
internal sealed class TestCommand : ICommand<Unit>
{
  [Option("quiet", "q", Description = "Hide test output unless the command fails")]
  public bool Quiet { get; set; }

  [Option("coverage", "c", Description = "Collect Cobertura code coverage into artifacts/coverage")]
  public bool Coverage { get; set; }

  internal sealed class Handler : IRequestHandler<TestCommand, Unit>
  {
    private static readonly string[] TestProjectRelativePaths =
    [
      "tests/timewarp-source-generators-test-console/timewarp-source-generators-test-console.csproj",
      "tests/timewarp-source-generators-analyzer-tests/timewarp-source-generators-analyzer-tests.csproj"
    ];

    private readonly ITerminal Terminal;
    private TestCommand Command = null!;
    private CancellationToken Ct;
    private string RepoRoot = null!;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(TestCommand command, CancellationToken ct)
    {
      Command = command;
      Ct = ct;

      if (!FindRepoRoot()) return Unit.Value;
      if (!await BuildTestProjectAsync()) return Unit.Value;
      if (Command.Coverage && !await RestoreCoverageToolAsync()) return Unit.Value;
      if (!await RunTestProjectAsync()) return Unit.Value;

      Terminal.WriteLine("\nTests completed successfully!".Green());
      return Unit.Value;
    }

    private bool FindRepoRoot()
    {
      string? root = Git.FindRoot();
      if (root is null)
      {
        Terminal.WriteErrorLine("Error: could not find repository root.");
        Environment.ExitCode = 1;
        return false;
      }

      RepoRoot = root;
      Terminal.WriteLine("Running test suite...");
      return true;
    }

    private async Task<bool> BuildTestProjectAsync()
    {
      foreach (string relativePath in TestProjectRelativePaths)
      {
        if (!await BuildOneAsync(relativePath))
          return false;
      }

      return true;
    }

    private async Task<bool> RunTestProjectAsync()
    {
      foreach (string relativePath in TestProjectRelativePaths)
      {
        if (!await RunOneAsync(relativePath))
          return false;
      }

      return true;
    }

    private async Task<bool> BuildOneAsync(string relativePath)
    {
      string testProjectPath = Path.Combine(RepoRoot, relativePath);
      Terminal.WriteLine($"Building {relativePath} (Release)...");
      CommandResult command = DotNet.Build(testProjectPath)
        .WithConfiguration("Release")
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .Build();

      return await ExecuteAsync(command, "Test project build failed!");
    }

    private async Task<bool> RestoreCoverageToolAsync()
    {
      Terminal.WriteLine("Restoring local dotnet-coverage tool...");
      CommandResult restore = Shell.Builder("dotnet")
        .WithArguments("tool", "restore")
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .Build();

      return await ExecuteAsync(restore, "dotnet tool restore failed; cannot collect coverage");
    }

    private async Task<bool> RunOneAsync(string relativePath)
    {
      string testProjectPath = Path.Combine(RepoRoot, relativePath);
      Terminal.WriteLine($"Running {relativePath} (Release)...");
      if (!Command.Coverage)
      {
        CommandResult command = DotNet.Run()
          .WithProject(testProjectPath)
          .WithConfiguration("Release")
          .WithNoBuild()
          .WithWorkingDirectory(RepoRoot)
          .WithNoValidation()
          .Build();

        return await ExecuteAsync(command, "Tests failed!");
      }

      string coverageDirectory = Path.Combine(RepoRoot, "artifacts", "coverage");
      Directory.CreateDirectory(coverageDirectory);
      string outputPath = Path.Combine(
        coverageDirectory,
        Path.GetFileNameWithoutExtension(relativePath) + ".cobertura.xml");

      // dotnet-coverage 18.12.0 targets net8.0. Major roll-forward starts it on the 11.0.x SDK.
      CommandResult covered = Shell.Builder("dotnet")
        .WithArguments
        (
          "dotnet-coverage", "collect",
          "--nologo",
          "--output-format", "cobertura",
          "--output", outputPath,
          "--",
          "dotnet", "run",
          "--project", testProjectPath,
          "--configuration", "Release",
          "--no-build"
        )
        .WithWorkingDirectory(RepoRoot)
        .WithEnvironmentVariable("DOTNET_ROLL_FORWARD", "Major")
        .WithNoValidation()
        .Build();

      if (!await ExecuteAsync(covered, "Tests failed!"))
      {
        return false;
      }

      if (File.Exists(outputPath))
      {
        return true;
      }

      Terminal.WriteErrorLine($"Coverage file was not written: {outputPath}".Red());
      Environment.ExitCode = 1;
      return false;
    }

    private async Task<bool> ExecuteAsync(CommandResult command, string failureMessage)
    {
      if (Command.Quiet)
      {
        CommandOutput result = await command.CaptureAsync(Ct);
        if (!result.Success)
        {
          Terminal.WriteErrorLine(result.Combined);
          Terminal.WriteErrorLine(failureMessage.Red());
          Environment.ExitCode = 1;
          return false;
        }

        return true;
      }

      int exitCode = await command.RunAsync(Ct);
      if (exitCode != 0)
      {
        Terminal.WriteErrorLine(failureMessage.Red());
        Environment.ExitCode = exitCode;
        return false;
      }

      return true;
    }
  }
}
