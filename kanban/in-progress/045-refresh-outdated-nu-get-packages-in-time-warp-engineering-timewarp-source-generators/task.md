# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators.
Filter: (all)
Outdated:
  TimeWarp.Nuru 4.0.0-beta.15 -> 4.0.0-beta.16 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Nuru.DevCli 4.0.0-beta.15 -> 4.0.0-beta.16 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Amuru 3.0.0-beta.5 -> 3.0.0-beta.10 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Amuru.Tools 3.0.0-beta.5 -> 3.0.0-beta.10 (dev-only: tools/dev-cli/Directory.Build.props)
Every outdated pin is dev-only. Bump them. Do not change <Version> in source/Directory.Build.props: the wave merges this task and does not release this repo.
Edit Directory.Packages.props in this repo only.

## Goal (implementer restatement)

Bump the four dev-only TimeWarp package pins in the root `Directory.Packages.props` to their latest prereleases.
Only the dev CLI (`tools/dev-cli`) and the git hooks consume them; no shipped package changes.

Acceptance criteria:

- `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` are pinned at `4.0.0-beta.16`.
- `TimeWarp.Amuru` and `TimeWarp.Amuru.Tools` are pinned at `3.0.0-beta.10`.
- `<Version>` in `source/Directory.Build.props` is unchanged (no release).
- Only this repo's `Directory.Packages.props` changes for the bump.
- `./bin/dev build` passes and `ganda repo audit` passes.

## Session

- Minted by ganda task wave run 20261118T170809Z-b6e473
- 2026-11-19 implementer: bumped the four Dev CLI pins in `Directory.Packages.props`.
  Amuru 3.0.0-beta.10 ships analyzers AMURU0001/AMURU0002 that reject `Shell.Builder("dotnet")`.
  Three dev CLI call sites failed to compile and now use the typed builders:
  `DotNet.Tool().Restore()` and `DotNet.Tool().Run("dotnet-coverage")` in `tools/dev-cli/endpoints/test-command.cs`,
  and `DotNet.NuGet().Push(...)` in `tools/dev-cli/endpoints/workflow-command.cs`.
  Checked by hand that `dotnet tool run dotnet-coverage collect … -- <cmd>` passes `--` through to the tool and writes the cobertura file.
- Boy-scout: `ganda repo audit` raised the advisory `vscode-window-icon` (missing `.vscode/settings.json`).
  `ganda repo audit --fix` wrote it. The file is gitignored, so nothing was committed for it.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5), session not reported, max-turns 500, Done — 2026-11-18T17:13:49Z

## Results

Changes:

- `Directory.Packages.props`: TimeWarp.Nuru and TimeWarp.Nuru.DevCli 4.0.0-beta.15 → 4.0.0-beta.16, TimeWarp.Amuru and TimeWarp.Amuru.Tools 3.0.0-beta.5 → 3.0.0-beta.10.
- `tools/dev-cli/endpoints/test-command.cs`: `dotnet tool restore` and the `dotnet-coverage collect` run now use `DotNet.Tool()` (AMURU0001/AMURU0002).
- `tools/dev-cli/endpoints/workflow-command.cs`: `dotnet nuget push` now uses `DotNet.NuGet().Push(...)` with `WithSource`, `WithSkipDuplicate`, and an optional `WithApiKey` (AMURU0001).
- `<Version>` in `source/Directory.Build.props` is unchanged.

Gates:

- `./bin/dev build`: PASS (0 warnings, 0 errors), run before and after the dev CLI fix.
- `dotnet run --file tools/dev-cli/dev.cs -- self-install`: first run FAIL (3 AMURU errors), after the fix PASS. Run because the bump changes the dev CLI's own packages.
- Test files: none added or edited. No test covers the dev CLI endpoints (grep for `TestCommand`, `WorkflowCommand`, `PushPackageAsync`, and `RestoreCoverageToolAsync` under `tests/` finds nothing). CI runs the full suite.
- `ganda repo audit`: PASS (1 advisory, `vscode-window-icon`). After `ganda repo audit --fix`: all checks PASS.

### How to validate

Smoke:

```bash
dotnet run --file tools/dev-cli/dev.cs -- self-install
./bin/dev build
./bin/dev --help
```

Expect: self-install prints "Successfully installed dev CLI" and has no AMURU0001/AMURU0002 errors. Build prints "Build completed successfully!" with 0 warnings and 0 errors. `--help` lists the dev commands.
