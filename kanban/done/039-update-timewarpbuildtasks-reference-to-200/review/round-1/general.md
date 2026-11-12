# Round 1 — general
**Date:** 2026-11-12
**Scope reviewed:** branch `task/039-update-timewarpbuildtasks-reference-to-200` (`31fecf06`) vs `master`, product commit `f5834efa`

## Summary

Commit `f5834efa` sets `TimeWarp.Build.Tasks` to `2.0.0` in `Directory.Packages.props` and moves `<Version>` in `source/Directory.Build.props` from `1.0.0-beta.14` to `1.0.0-beta.15`. The `PackageReference` in `Directory.Build.props` stays `PrivateAssets="all"`, and `nuget.config` still lists only nuget.org. The 2.0.0 nupkg from nuget.org matches the breaking changes written in `## Notes`: the task assembly is `net11.0` only, `TWBT0001` fails non-Core or pre-11 MSBuild, the git-metadata properties are unchanged, and the nuspec has no dependencies. The implementer restatement matches the original task. Risk is low.

## Verification (re-run by reviewer)

| Gate | Result |
|------|--------|
| `RestoreSources=/home/steve/.nuget/packages ./bin/dev build` | Exit 0. `0 Warning(s)`, `0 Error(s)`. Packed `TimeWarp.SourceGenerators.1.0.0-beta.15.nupkg`. |
| `RestoreSources=/home/steve/.nuget/packages ./bin/dev test` | Exit 0. Analyzer tests printed `Passed: 56  Failed: 0  Total: 56`. |
| Cached `timewarp.build.tasks/2.0.0/.nupkg.metadata` | `"source": "https://api.nuget.org/v3/index.json"`. All three `project.assets.json` files resolve `TimeWarp.Build.Tasks/2.0.0`. |
| 2.0.0 nuspec, props, and task DLL | Nuspec has no `<dependencies>` and is still a development dependency. `UsingTask` loads `net11.0/TimeWarp.Build.Tasks.dll` when `MSBuildRuntimeType == Core`. Target `TimeWarpSetGitMetadata` emits `TWBT0001` when MSBuild is not Core or the runtime is older than 11.0. DLL strings name `Microsoft.Build.*` and `System.*` only. |
| Packed `TimeWarp.SourceGenerators.1.0.0-beta.15.nupkg` | Dependency group is `Microsoft.CodeAnalysis.CSharp` 5.9.0. `TimeWarp.Build.Tasks` is absent. |
| Generated `AssemblyInfo.cs` after this build | `CommitHash` is `31fecf06f1caaf0681d9a1f3221c331d87536147`. Package `CommitDate` is `2026-11-12T03:54:22`. Informational version is `1.0.0-beta.15`. |
| `./bin/dev check-version` | Sandbox proxy returned 403 for the NuGet lookup, so this walk did not re-execute the live check. `~/.nuget/packages/timewarp.sourcegenerators/1.0.0-beta.14/.nupkg.metadata` records source `https://api.nuget.org/v3/index.json`. The global cache has no `1.0.0-beta.15` folder. The bump to `1.0.0-beta.15` matches requirement 6. |
| `ganda repo audit` | 42 passed, 2 skipped. `self-hosted-runner` failed because `gh repo view --json isPrivate` could not run in this sandbox. |

`Directory.Build.props` line 88 is still `<PackageReference Include="TimeWarp.Build.Tasks" PrivateAssets="all" />`. The diff adds no `NoWarn`, suppression, or feed.

`## Notes` cite `global.json` as the SDK pin. This repo has no `global.json`. The .NET 11 floor is `<TargetFramework>net11.0</TargetFramework>` in `Directory.Build.props` and `dotnet-version: 11.0.x` in `.github/workflows/workflow.yml`. Local `dotnet --version` is `11.0.100`. The recorded effect holds: this CLI build and CI stay on a runtime that can load the `net11.0` task. `TWBT0001` applies to Visual Studio and `MSBuild.exe`, which the package readme also excludes. The repo already builds with the `dotnet` CLI.

## Checks against the task

- **Requirement 1 (breaking changes in Notes):** The 1.0.2 and 2.0.0 props differ by the `net11.0` retarget, the Core-only `UsingTask`, and `TWBT0001`. Property names `TimeWarpEnableGitMetadata`, `CommitHash`, and `CommitDate` are the same. Package commit on the 2.0.0 nuspec is `8dd25a7c26cb3c6c5fba8d9320ce6d2538fce3ae`.
- **Requirement 2 (pin, leave the PackageReference):** `Directory.Packages.props` line 18 is `Version="2.0.0"`. The `PackageReference` is unchanged.
- **Requirement 3 (nuget.org, no new feed):** `nuget.config` is unchanged and clears other sources. The cached 2.0.0 package records nuget.org as its source.
- **Requirement 4 (fix breaks, no suppressions):** `./bin/dev build` is 0 warnings and 0 errors with `TreatWarningsAsErrors` set. The product diff has no suppression.
- **Requirement 5 (no cycle):** The 2.0.0 nuspec has no dependencies. The task DLL does not reference `TimeWarp.SourceGenerators`. The packed source-generator nuspec does not depend on `TimeWarp.Build.Tasks`.
- **Requirement 6 (version bump):** Source version is `1.0.0-beta.15`. Cached nuget.org metadata shows `1.0.0-beta.14` published. The global cache has no `1.0.0-beta.15`.
- **Restatement vs original task:** `## Goal (implementer restatement)` matches the original requirements: record the 2.0.0 breaks, pin 2.0.0, restore from nuget.org, stay clean without suppressions, confirm the cycle stays gone, and bump `<Version>` only when the current version is published. No misread.

## Issues

None.
