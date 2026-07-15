#region Purpose
// Executes the full CI/CD pipeline with mode detection
#endregion
#region Design
// Auto-detects mode from GITHUB_EVENT_NAME or accepts explicit --mode flag
//   pr/merge: clean -> build -> test
//   release:  tag match + NuGet not-published -> clean -> build -> test -> push
// Invokes sibling command handlers in-process so CI needs no self-install
// Clean uses IRepoCleanService (no shell to ./bin/dev)
// Release version comes from Version in source/Directory.Build.props and
// must match the git tag (GITHUB_REF_NAME) on release events
// Pack is GeneratePackageOnBuild; push targets artifacts/packages/*.nupkg
#endregion

namespace DevCli.Commands;

[NuruRoute("workflow", Description = "Execute full CI/CD pipeline")]
internal sealed class WorkflowCommand : ICommand<Unit>
{
  [Option("mode", "m", Description = "CI mode: pr, merge, or release (auto-detected from GITHUB_EVENT_NAME if not specified)")]
  public string? Mode { get; set; }

  [Option("api-key", Description = "NuGet API key for publishing (release mode only)")]
  public string? ApiKey { get; set; }

  internal sealed class Handler : ICommandHandler<WorkflowCommand, Unit>
  {
    private const string PackageId = "TimeWarp.SourceGenerators";
    private const string NuGetSource = "https://api.nuget.org/v3/index.json";

    private readonly ITerminal Terminal;
    private readonly IRepoCleanService RepoCleanService;
    private readonly INuGetPackageService NuGetPackageService;
    private CancellationToken Ct;
    private string RepoRoot = null!;

    public Handler
    (
      ITerminal terminal,
      IRepoCleanService repoCleanService,
      INuGetPackageService nuGetPackageService
    )
    {
      Terminal = terminal;
      RepoCleanService = repoCleanService;
      NuGetPackageService = nuGetPackageService;
    }

    public async ValueTask<Unit> Handle(WorkflowCommand command, CancellationToken ct)
    {
      Ct = ct;

      if (!FindRepoRoot()) return Value;

      CiMode mode = DetermineMode(command.Mode);
      Terminal.WriteLine($"Starting CI workflow (mode: {mode})...");

      if (mode == CiMode.Release)
      {
        await RunReleaseWorkflowAsync(command.ApiKey);
      }
      else
      {
        await RunPrWorkflowAsync();
      }

      if (Environment.ExitCode == 0)
        Terminal.WriteLine("\nWorkflow completed successfully!".Green());

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
      return true;
    }

    private CiMode DetermineMode(string? explicitMode)
    {
      if (!string.IsNullOrEmpty(explicitMode))
      {
        return explicitMode.ToLowerInvariant() switch
        {
          "release" => CiMode.Release,
          "merge" => CiMode.Merge,
          _ => CiMode.Pr
        };
      }

      string? eventName = Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME");

      CiMode mode = eventName switch
      {
        "push" => CiMode.Merge,
        "release" => CiMode.Release,
        // Manual runs are build/test only; use --mode release explicitly to publish
        "workflow_dispatch" => CiMode.Pr,
        _ => CiMode.Pr // pull_request and local dev
      };

      Terminal.WriteLine($"Detected GITHUB_EVENT_NAME: {eventName ?? "(not set)"} -> Mode: {mode}");
      return mode;
    }

    private async Task RunPrWorkflowAsync()
    {
      if (!await RunStepAsync("Clean", CleanAsync)) return;
      if (!await RunStepAsync("Build", () => new BuildCommand.Handler(Terminal).Handle(new BuildCommand(), Ct))) return;
      await RunStepAsync("Test", () => new TestCommand.Handler(Terminal).Handle(new TestCommand(), Ct));
    }

    private async Task RunReleaseWorkflowAsync(string? apiKey)
    {
      string? version = ReadVersion();
      if (version is null)
      {
        Terminal.WriteErrorLine("Error: could not read Version from source/Directory.Build.props.".Red());
        Environment.ExitCode = 1;
        return;
      }

      if (!CheckVersionMatchesTag(version)) return;
      if (!await CheckNotPublishedOnNuGetAsync(version)) return;
      if (!await RunStepAsync("Clean", CleanAsync)) return;
      if (!await RunStepAsync("Build", () => new BuildCommand.Handler(Terminal).Handle(new BuildCommand(), Ct))) return;
      if (!await RunStepAsync("Test", () => new TestCommand.Handler(Terminal).Handle(new TestCommand(), Ct))) return;
      await PushPackageAsync(version, apiKey);
    }

    private async ValueTask<Unit> CleanAsync()
    {
      Terminal.WriteLine("Cleaning repository artifacts...");
      CleanResult result = await RepoCleanService.CleanAsync(Ct);
      Terminal.WriteLine
      (
        $"Clean complete: {result.ObjDirectoriesDeleted} obj, {result.BinDirectoriesDeleted} bin, {result.RootBinFilesCleaned} root-bin items."
      );
      return Value;
    }

    private async Task<bool> RunStepAsync(string title, Func<ValueTask<Unit>> step)
    {
      Terminal.WriteLine($"\n=== {title} ===");
      await step();

      if (Environment.ExitCode != 0)
      {
        Terminal.WriteErrorLine($"{title} failed!".Red());
        return false;
      }
      return true;
    }

    private string? ReadVersion()
    {
      string propsPath = Path.Combine(RepoRoot, "source", "Directory.Build.props");
      if (!File.Exists(propsPath)) return null;

      Match match = Regex.Match(File.ReadAllText(propsPath), "<Version>(.+?)</Version>");
      return match.Success ? match.Groups[1].Value : null;
    }

    private bool CheckVersionMatchesTag(string version)
    {
      // Only enforce on tag-triggered releases; workflow_dispatch has no tag
      if (Environment.GetEnvironmentVariable("GITHUB_EVENT_NAME") != "release") return true;

      string? tag = Environment.GetEnvironmentVariable("GITHUB_REF_NAME");
      string? tagVersion = tag?.TrimStart('v');

      if (tagVersion != version)
      {
        Terminal.WriteErrorLine($"Error: release tag '{tag}' does not match Version '{version}'.".Red());
        Environment.ExitCode = 1;
        return false;
      }

      Terminal.WriteLine($"Release tag '{tag}' matches Version '{version}'.");
      return true;
    }

    private async Task<bool> CheckNotPublishedOnNuGetAsync(string version)
    {
      Terminal.WriteLine($"\n=== Check NuGet (not published) ===");
      Terminal.WriteLine($"Checking if {PackageId} {version} is already on NuGet.org...");

      NuGetSearchResult? search = await NuGetPackageService.SearchAsync(PackageId, Ct);
      bool alreadyPublished = search?.Versions.Any
      (
        v => string.Equals(v.Version, version, StringComparison.OrdinalIgnoreCase)
      ) == true;

      if (alreadyPublished)
      {
        Terminal.WriteErrorLine
        (
          $"Error: {PackageId} {version} is already published to NuGet.org. Increment Version in source/Directory.Build.props.".Red()
        );
        Environment.ExitCode = 1;
        return false;
      }

      Terminal.WriteLine($"{PackageId} {version} is not yet published on NuGet.org.");
      return true;
    }

    private async Task PushPackageAsync(string version, string? apiKey)
    {
      Terminal.WriteLine("\n=== Push to NuGet ===");

      string nupkgPath = Path.Combine(RepoRoot, "artifacts", "packages", $"{PackageId}.{version}.nupkg");
      if (!File.Exists(nupkgPath))
      {
        Terminal.WriteErrorLine($"Error: package not found: {nupkgPath}".Red());
        Environment.ExitCode = 1;
        return;
      }

      Terminal.WriteLine($"Pushing {PackageId}.{version}.nupkg...");

      List<string> args = ["nuget", "push", nupkgPath, "--source", NuGetSource, "--skip-duplicate"];
      if (!string.IsNullOrEmpty(apiKey))
      {
        args.AddRange(["--api-key", apiKey]);
      }

      int exitCode = await Shell.Builder("dotnet")
        .WithArguments([.. args])
        .WithWorkingDirectory(RepoRoot)
        .WithNoValidation()
        .RunAsync(Ct);

      if (exitCode != 0)
      {
        Terminal.WriteErrorLine("Push failed!".Red());
        Environment.ExitCode = exitCode;
        return;
      }

      Terminal.WriteLine($"\nPublished {PackageId} {version} to NuGet.org!".Green());
    }
  }
}

internal enum CiMode
{
  Pr,
  Merge,
  Release
}
