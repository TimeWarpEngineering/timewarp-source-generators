# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators.
Filter: (all)
Outdated:
  Microsoft.CodeAnalysis.Analyzers 5.9.0 -> 5.12.0 (minor)
  Microsoft.CodeAnalysis.CSharp 5.9.0 -> 5.12.0 (minor)
  Microsoft.CodeAnalysis.CSharp.Workspaces 5.9.0 -> 5.12.0 (minor)
  TimeWarp.Nuru 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
  TimeWarp.Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
Edit Directory.Packages.props in this repo only.

Version: set <Version> in source/Directory.Build.props from 1.0.0-beta.15 to 1.0.0-beta.16 in this pull request. The prerelease suffix is -beta.N, never +beta.N. The wave releases it after merge.

## Goal (implementer restatement)

Bump five outdated packages in the root `Directory.Packages.props` only: the three Roslyn
packages (Microsoft.CodeAnalysis.Analyzers, .CSharp, .CSharp.Workspaces) 5.9.0 -> 5.12.0 and
TimeWarp.Nuru / TimeWarp.Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13. Bump `<Version>` in
`source/Directory.Build.props` from 1.0.0-beta.15 to 1.0.0-beta.16.

Acceptance criteria:

- [x] Directory.Packages.props carries the five new versions; no other package changes.
- [x] source/Directory.Build.props `<Version>` is `1.0.0-beta.16`.
- [x] `./bin/dev build`, `./bin/dev test`, and `ganda repo audit` pass.

## Session

- Minted by ganda task wave run 20261114T054552Z-9a0f39
- Bumped Roslyn 5.9.0 -> 5.12.0 and Nuru/Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13 in Directory.Packages.props; Version 1.0.0-beta.16.
- Boy-scout: `ganda repo audit --fix` resolved the vscode-window-icon warning. It only writes the git-ignored `.vscode/settings.json`, so the commit does not change.
- Re-ran dev CLI self-install so it compiles against Nuru 4.0.0-beta.13 (succeeded).

## Results

Package bumps in `Directory.Packages.props`:

- Microsoft.CodeAnalysis.Analyzers 5.9.0 -> 5.12.0
- Microsoft.CodeAnalysis.CSharp 5.9.0 -> 5.12.0
- Microsoft.CodeAnalysis.CSharp.Workspaces 5.9.0 -> 5.12.0
- TimeWarp.Nuru 4.0.0-beta.12 -> 4.0.0-beta.13
- TimeWarp.Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13

`source/Directory.Build.props` `<Version>` 1.0.0-beta.15 -> 1.0.0-beta.16.

### Gates

| Gate | Command | Result |
|------|---------|--------|
| Build | `./bin/dev build` | Pass: 0 warnings, 0 errors; packs TimeWarp.SourceGenerators.1.0.0-beta.16.nupkg |
| Test | `./bin/dev test` | Pass: 56 passed, 0 failed |
| Audit | `ganda repo audit` | First run passed with 1 advisory warning (vscode-window-icon: `.vscode/settings.json` missing). After `ganda repo audit --fix`, which writes the git-ignored local settings file, the rerun passed every check |

Also: `dotnet run --file tools/dev-cli/dev.cs -- self-install` succeeded, so the dev CLI compiles against Nuru 4.0.0-beta.13.

### How to validate

Smoke:

```bash
./bin/dev build
./bin/dev test
grep -E 'CodeAnalysis\.(Analyzers|CSharp)|TimeWarp\.Nuru' Directory.Packages.props
grep '<Version>' source/Directory.Build.props
```

Expect: the build succeeds with 0 warnings and packs `TimeWarp.SourceGenerators.1.0.0-beta.16.nupkg`. The tests report `Passed: 56  Failed: 0`. The grep shows the Roslyn packages at 5.12.0, Nuru and Nuru.DevCli at 4.0.0-beta.13, and `<Version>1.0.0-beta.16</Version>`.
