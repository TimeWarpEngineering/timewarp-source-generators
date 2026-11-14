# Round 1 — general
**Date:** 2026-11-14
**Scope reviewed:** branch `task/042-refresh-outdated-nu-get-packages-in-time-warp` vs merge-base `b42f1e045981d7dff4a5b09d5c9b57a9e30cbc4e`. Product commits `c7b9bd4` and `348ba41`.

## Summary

The change updates five central pins in `Directory.Packages.props` and sets `<Version>` in `source/Directory.Build.props` to `1.0.0-beta.16`. A sixth pin, `TimeWarp.Build.Tasks`, moves from `2.0.0` to `2.0.1-beta.1`. Results records that move as a `package-pin-currency` attest failure after `2.0.1-beta.1` published on 2026-11-14, and records a second build, test, and audit after it. The implementer restatement matches the original task. No misread. Risk is low. The extra pin is dev-only (`PrivateAssets="all"`) and does not ship in the analyzer package.

## Record reviewed

Gates were not re-run. `## Results` and `### How to validate` cover the product diff, including the Build.Tasks bump and the rerun after it:

| Gate | Recorded result |
|------|-----------------|
| `./bin/dev build` | Pass: 0 warnings, 0 errors; packs `TimeWarp.SourceGenerators.1.0.0-beta.16.nupkg`. Rerun after the Build.Tasks bump also passed |
| `./bin/dev test` | Pass: 56 passed, 0 failed. Rerun after the Build.Tasks bump also passed |
| `ganda repo audit` | Advisory `vscode-window-icon` cleared by `--fix` into git-ignored `.vscode/settings.json`. Attest then failed `package-pin-currency` for Build.Tasks. After that bump, the final audit passed every check |
| dev CLI | `dotnet run --file tools/dev-cli/dev.cs -- self-install` succeeded against Nuru `4.0.0-beta.13` |

## Checks against the task

- **Five named pins:** `Directory.Packages.props` lines 8–10 are Roslyn `5.12.0` (`Microsoft.CodeAnalysis.Analyzers`, `.CSharp`, `.CSharp.Workspaces`). Lines 22–23 are `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` at `4.0.0-beta.13`. `Microsoft.CodeAnalysis.BannedApiAnalyzers` stays `5.6.0`. Amuru, Amuru.Tools, and Terminal stay at the versions the wave did not list.
- **Call sites use the central pins:** `ManagePackageVersionsCentrally` is true. `Directory.Build.props` references `Microsoft.CodeAnalysis.Analyzers` and `Microsoft.CodeAnalysis.CSharp` without inline versions. The analyzer test project references `Microsoft.CodeAnalysis.CSharp.Workspaces` the same way. `tools/dev-cli/Directory.Build.props` references `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` together, with no inline versions. No other `PackageVersion` or `Version=` override exists in `*.props` or `*.csproj`.
- **Version:** `source/Directory.Build.props` line 7 is `<Version>1.0.0-beta.16</Version>`. The suffix is `-beta.16`.
- **Build.Tasks:** line 18 is `2.0.1-beta.1`. `Directory.Build.props` line 88 is still `<PackageReference Include="TimeWarp.Build.Tasks" PrivateAssets="all" />`. This pin is outside the wave list. Results names the attest failure and the rerun. The acceptance checkbox that says "no other package changes" is stale next to that record. The bump is the currency fix the record describes, so it is not a defect.
- **Repo scope:** the product diff is only `Directory.Packages.props` and `source/Directory.Build.props`.
- **Restatement vs original task:** `## Goal (implementer restatement)` names the same five packages, the same versions, the same `<Version>` bump, and the same two files. No misread.

## Issues

None.
