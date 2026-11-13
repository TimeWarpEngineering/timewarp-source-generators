# Retarget the source generator project from netstandard2.0 to net11.0

## Description

`source/timewarp-source-generators/timewarp-source-generators.csproj` is the only project in
the repo still on `netstandard2.0`. Everything else is `net11.0`. Move it to `net11.0`.

Decision (Steven, 2026-11-12): this is deliberate. The package will no longer load in a
compiler host that runs on .NET Framework, which means Visual Studio. That is accepted:
"Visual Studio is the one that is behind not us." Do not multi-target and do not keep a
`netstandard2.0` asset as a fallback.

## Requirements

1. Set `<TargetFramework>net11.0</TargetFramework>` in the generator csproj. If the repo-level
   `Directory.Build.props` already supplies `net11.0`, removing the line is fine too; pick
   whichever matches the other projects.
2. Roslyn's analyzer rules warn when a compiler extension targets something other than
   `netstandard2.0` (RS1041, and possibly others under `EnforceExtendedAnalyzerRules`). Because
   the retarget is the decision, suppress exactly the diagnostic(s) that say "wrong target
   framework" and nothing else, with a comment that names this task. Report every diagnostic id
   you suppress in Results. Any other new diagnostic gets fixed, not suppressed.
3. Remove what only existed for `netstandard2.0`: polyfills (`IsExternalInit`, nullable
   attributes, index/range and similar), polyfill packages, and `#if` branches. Leave the
   `NoWarn` entries (`RS2001`, `NU5128`) alone unless the retarget makes one unnecessary; say
   which in Results.
4. Packaging: the analyzer DLL is packed by hand to `analyzers/dotnet/cs` from `$(OutputPath)`.
   Confirm `dotnet pack` still puts the `net11.0` DLL there and that the test console and the
   analyzer tests still load the generator and analyzers from the built output.
5. This breaks consumers on older hosts, so:
   - bump `<Version>` in `source/Directory.Build.props` to the next beta if
     `./bin/dev check-version` requires it;
   - add a `documentation/releases.md` entry for that version that says the package now targets
     `net11.0`, needs the .NET 11 SDK compiler, and does not load in Visual Studio. The beta.14
     entry says "The shipped package still targets `netstandard2.0`"; leave history as it is.
   - update `readme.md` and any other doc that states a `netstandard2.0` target or Visual
     Studio support.
6. Commit as a breaking change (`feat!:` or a `BREAKING CHANGE:` footer).

## Goal (implementer restatement)

Move `timewarp-source-generators.csproj` off `netstandard2.0` onto `net11.0` (the repo default),
with no multi-targeting and no netstandard fallback asset. Visual Studio (a .NET Framework compiler
host) stops loading the package; that is accepted.

Acceptance:
- The generator project builds for `net11.0`; nothing in the repo still targets `netstandard2.0`.
- Only the "compiler extension should target netstandard2.0" Roslyn diagnostic(s) are suppressed,
  each with a comment naming task 041; any other new diagnostic is fixed.
- Any netstandard-only polyfills, polyfill packages, and `#if` branches are gone (none expected).
- `./bin/dev build` is 0 warnings / 0 errors; `./bin/dev test` passes with the generator and
  analyzers loaded from the `net11.0` build output.
- The packed nupkg carries the `net11.0` DLL under `analyzers/dotnet/cs`.
- Version bumped only if `./bin/dev check-version` requires it; `documentation/releases.md` gains an
  entry for that version stating `net11.0`, .NET 11 SDK compiler required, no Visual Studio; readme
  and other docs updated where they state the target or VS support.
- Committed as a breaking change; `ganda repo audit` clean.

## Checklist

- [x] Generator project targets `net11.0`; no `netstandard2.0` target left in the repo
- [x] Only the target-framework analyzer diagnostic(s) are suppressed, each with a comment
- [x] netstandard-only polyfills and conditionals removed
- [x] `./bin/dev build` reports 0 warnings and 0 errors
- [x] `./bin/dev test` passes (generators and analyzers load from the `net11.0` build)
- [x] Packed nupkg has the `net11.0` DLL under `analyzers/dotnet/cs`
- [x] `documentation/releases.md` and `readme.md` state the new target and the Visual Studio limit
- [x] `ganda repo audit` clean

## Session

- Created: 83d187ba-085b-471b-8964-44a61c5cebc6 (2026-11-12)
- Implement oracle (Claude Opus 5.5, 2026-11-13): retargeted to net11.0, suppressed RS1041 only,
  updated docs, gates green. See Results.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 72a26f76-39d9-4252-8df5-d20053eb94b1, max-turns 500, Done — 2026-11-13T07:57:25Z

## Results

### Changes

- `source/timewarp-source-generators/timewarp-source-generators.csproj`: `<TargetFramework>` is now
  `net11.0`. It stays explicit, like the test projects. No `netstandard2.0` target is left
  in the repo. `documentation/releases.md` beta.14 keeps its historical mention.
- **Suppressed diagnostic ids: `RS1041` only.** It was the only new diagnostic: 5 errors, one per
  compiler extension (`file-length-analyzer.cs`, `file-name-rule-analyzer.cs`,
  `global-usings-rule-analyzer.cs`, `purpose-region-analyzer.cs`,
  `type-stem-identifier-analyzer.cs`). It is suppressed with its own `<NoWarn>` line and a
  comment that names task 041. `EnforceExtendedAnalyzerRules` stays on.
- `.timewarp/suppression-baseline.json`: added `RS1041: 1` for the generator csproj by hand.
  The `analyzer-suppressions` check says a baseline rise must be a hand edit, visible in review.
- Polyfills, polyfill packages, and `#if` branches: none existed, so nothing was removed.
- Existing `NoWarn` entries: both stay. Each was removed in turn and the project rebuilt with
  `--no-incremental`. `RS2001` still fires as 3 errors (changed Category on TW0001, TW0007,
  TWA0023 vs AnalyzerReleases.Shipped.md); it has nothing to do with the target framework.
  `NU5128` still fires at pack: there is a `net11.0` dependency group but no lib/ref folder,
  because `IncludeBuildOutput=false`.
- Version: `./bin/dev check-version` reports `1.0.0-beta.15` is one prerelease ahead of the
  published beta.14, so there is no bump. Added a `1.0.0-beta.15` entry to
  `documentation/releases.md`: breaking, `net11.0`, needs the .NET 11 SDK compiler, and does not
  load in Visual Studio. It also mentions Build.Tasks 2.0.0 from task 039.
- `readme.md`: the dotnet badge changes from 6.0 to 11.0, and a new `## Requirements` section
  states the `net11.0` target and the Visual Studio limit. No other doc stated a target or
  Visual Studio support.
- Audit boy-scout: `ganda repo audit --fix` refreshed the outdated memsearch hooks in
  `.githooks/*.cs` (`memsearch-scaffold`). It also wrote the gitignored `.vscode/settings.json`
  (`vscode-window-icon`).

### Packaging check

- `artifacts/packages/TimeWarp.SourceGenerators.1.0.0-beta.15.nupkg` contains
  `analyzers/dotnet/cs/timewarp-source-generators.dll`. That DLL's TargetFrameworkAttribute is
  `.NETCoreApp,Version=v11.0`. The nuspec dependency group is `net11.0`. There is no `lib/` folder.
- Both test projects build against `source/timewarp-source-generators/bin/Release/net11.0/`.
  The test console loads it as `OutputItemType="Analyzer"`, and the analyzer tests reference it.

### Gates

| Gate | Command | Result |
|------|---------|--------|
| build | `./bin/dev build` | 0 Warning(s), 0 Error(s) |
| test | `./bin/dev test` | exit 0; Passed: 56, Failed: 0, Total: 56 |
| audit | `ganda repo audit` | 1st run: passed, 2 advisory warnings (memsearch-scaffold, vscode-window-icon). After `--fix`: 42 passed, 0 failed, 3 skipped. "Repository passes all audit checks." |

### How to validate

Smoke:

```bash
./bin/dev build && ./bin/dev test
unzip -l artifacts/packages/TimeWarp.SourceGenerators.1.0.0-beta.15.nupkg
unzip -p artifacts/packages/TimeWarp.SourceGenerators.1.0.0-beta.15.nupkg \
  analyzers/dotnet/cs/timewarp-source-generators.dll | strings | grep NETCoreApp
grep -rn netstandard2.0 --include='*.csproj' --include='*.props' .
```

Expect: the build reports 0 warnings and 0 errors, and the tests pass 56/56. The nupkg lists
`analyzers/dotnet/cs/timewarp-source-generators.dll` and no `lib/`. The strings check prints
`.NETCoreApp,Version=v11.0`. The final grep prints nothing.

## Notes

Requested by Steven 2026-11-12. Unreleased `1.0.0-beta.15` (task 039, Build.Tasks 2.0.0) is
already on master; `check-version` decides whether this task needs its own bump.
