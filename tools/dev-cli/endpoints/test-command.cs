#region Purpose
// Run the test suite
#endregion
#region Design
// Builds and runs the test-console project in Release (no Fixie/dotnet test suite yet)
// Fails on non-zero exit from build or run
// Handler stores Command and Ct as fields so private methods are zero-parameter
// Streams output via Amuru RunAsync by default; --quiet uses CaptureAsync
#endregion

namespace DevCli.Commands;

[NuruRoute("test", Description = "Run the test suite")]
internal sealed class TestCommand : ICommand<Unit>
{
  [Option("quiet", "q", Description = "Hide test output unless the command fails")]
  public bool Quiet { get; set; }

  internal sealed class Handler : ICommandHandler<TestCommand, Unit>
  {
    private const string TestProjectRelativePath =
      "tests/timewarp-source-generators-test-console/timewarp-source-generators-test-console.csproj";

    private readonly ITerminal Terminal;
    private TestCommand Command = null!;
    private CancellationToken Ct;
    private string RepoRoot = null!;
    private string TestProjectPath = null!;

    public Handler(ITerminal terminal)
    {
      Terminal = terminal;
    }

    public async ValueTask<Unit> Handle(TestCommand command, CancellationToken ct)
    {
      Command = command;
      Ct = ct;

      if (!FindRepoRoot()) return Value;
      if (!await BuildTestProjectAsync()) return Value;
      if (!await RunTestProjectAsync()) return Value;

      Terminal.WriteLine("\nTests completed successfully!".Green());
      return Value;
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
      TestProjectPath = Path.Combine(RepoRoot, TestProjectRelativePath);
      Terminal.WriteLine("Running test suite...");
      return true;
    }

    private async Task<bool> BuildTestProjectAsync()
    {
      Terminal.WriteLine($"Building {TestProjectRelativePath} (Release)...");
      CommandResult command = DotNet.Build(TestProjectPath)
        .WithConfiguration("Release")
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .Build();

      return await ExecuteAsync(command, "Test project build failed!");
    }

    private async Task<bool> RunTestProjectAsync()
    {
      Terminal.WriteLine($"Running {TestProjectRelativePath} (Release)...");
      CommandResult command = DotNet.Run()
        .WithProject(TestProjectPath)
        .WithConfiguration("Release")
        .WithNoBuild()
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .Build();

      return await ExecuteAsync(command, "Tests failed!");
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
