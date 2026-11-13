# Round 1 — general
**Date:** 2026-11-13
**Scope reviewed:** branch `task/041-retarget-the-source-generator-project-from` (`fb5facf9`) vs `master` (`cf2f4abd`), product commit `cb48bb64`

## Summary

Commit `cb48bb64` sets `<TargetFramework>net11.0</TargetFramework>` on `source/timewarp-source-generators/timewarp-source-generators.csproj` and suppresses `RS1041` on its own `<NoWarn>` line, with a comment that names task 041. `RS2001` and `NU5128` stay. The suppression baseline records `RS1041: 1` for that project. `documentation/releases.md` adds a breaking `1.0.0-beta.15` entry and leaves the beta.14 history sentence. `readme.md` moves the badge to 11.0 and adds a Requirements section. `<Version>` stays `1.0.0-beta.15`. The commit message is `feat!:` and includes a `BREAKING CHANGE:` footer. The implementer restatement matches the original task. Risk is low.

## Record reviewed

Gates were not re-run. `## Results` and `### How to validate` cover the change:

| Gate | Recorded result |
|------|-----------------|
| `./bin/dev build` | 0 Warning(s), 0 Error(s) |
| `./bin/dev test` | exit 0; Passed: 56, Failed: 0, Total: 56 |
| `ganda repo audit` | after `--fix`: 42 passed, 0 failed, 3 skipped |
| pack | `analyzers/dotnet/cs/timewarp-source-generators.dll`, TargetFrameworkAttribute `.NETCoreApp,Version=v11.0`, dependency group `net11.0`, no `lib/` |
| version | `./bin/dev check-version` reports `1.0.0-beta.15` is one prerelease ahead of published beta.14, so no bump |

`ganda repo audit --fix` also added `#region Purpose` comments to the five `.githooks/*.cs` files. Those hunks are comments only. The comment under each region matches the hook's existing behavior.

## Checks against the task

- **Requirement 1 (net11.0, no fallback):** The generator csproj line 4 is `<TargetFramework>net11.0</TargetFramework>`. There is no `<TargetFrameworks>` and no second asset. Repo `Directory.Build.props` already sets `net11.0`; the explicit line matches the test projects. Every `TargetFramework` in `*.csproj` and `*.props` outside kanban is `net11.0`.
- **Requirement 2 (suppress only the wrong-target diagnostic):** `Microsoft.CodeAnalysis.Analyzers` 5.9.0 documents `RS1041` as "Compiler extensions should be implemented in assemblies targeting netstandard2.0". The new `<NoWarn>` is only `RS1041`, and the comment names task 041. The five extension types (`FileLengthAnalyzer`, `FileNameRuleAnalyzer`, `GlobalUsingsRuleAnalyzer`, `PurposeRegionAnalyzer`, `TypeStemIdentifierAnalyzer`) match the recorded "5 errors, one per compiler extension". The baseline value `1` matches one `NoWarn` site, the same shape as the existing `RS2001: 1` and `NU5128: 1` entries. `EnforceExtendedAnalyzerRules` stays `true`. No other diagnostic id was added to `NoWarn` or the baseline.
- **Requirement 3 (polyfills, leave RS2001 and NU5128):** The generator sources have no `IsExternalInit`, polyfill package, or `#if` branch. The original `<NoWarn>$(NoWarn);RS2001;NU5128</NoWarn>` is unchanged. Results say each id still fires when removed, so both stay.
- **Requirement 4 (pack path and tests):** The pack item is still `$(OutputPath)\$(AssemblyName).dll` to `analyzers/dotnet/cs`. The test console references the project with `OutputItemType="Analyzer"`. The analyzer tests use a normal `ProjectReference` and construct the analyzer types. Neither test project hardcodes `netstandard2.0`.
- **Requirement 5 (version and docs):** `source/Directory.Build.props` is still `<Version>1.0.0-beta.15</Version>` and is not in the product diff. `documentation/releases.md` beta.15 states `net11.0`, the .NET 11 SDK compiler, and no Visual Studio load. The beta.14 sentence "The shipped package still targets `netstandard2.0`" is unchanged. `readme.md` content, ignoring the CRLF-to-LF rewrite, is the badge `dotnet-6.0` to `dotnet-11.0` plus the Requirements section. No other product doc states a current `netstandard2.0` target or Visual Studio support. Kanban history that mentions the old target was left as history.
- **Requirement 6 (breaking commit):** `cb48bb64` subject is `feat!:` and the body has `BREAKING CHANGE:`.
- **Restatement vs original task:** `## Goal (implementer restatement)` matches the original requirements: single `net11.0` target, suppress only the wrong-target Roslyn diagnostic, drop netstandard-only polyfills, keep the analyzer DLL under `analyzers/dotnet/cs`, bump the version only when `check-version` requires it, document the host limit, and commit a breaking change. No misread.

## Issues

None.
