# Update TimeWarp.Build.Tasks reference to 2.0.0

## Description

`ganda nuget outdated` (2026-11-12) reports `TimeWarp.Build.Tasks` 1.0.2 -> 2.0.0 on nuget.org.
It is the only outdated package. This repo pins 1.0.2 in `Directory.Packages.props`; the
reference itself is in `Directory.Build.props` (`PrivateAssets="all"`).

This is a major version bump, so expect breaking changes: removed, renamed, or re-defaulted
MSBuild tasks, targets, or properties.

## Goal (implementer restatement)

Move this repo's build-time dependency `TimeWarp.Build.Tasks` from 1.0.2 to 2.0.0 (the only
outdated package). First find what the major bump breaks and write it down; then change only the
central pin, restore from nuget.org, and get a clean build and test run with no suppressions.

Acceptance:

- Notes list the 2.0.0 breaking changes and whether each one affects this repo.
- 2.0.0 has no package or assembly dependency on TimeWarp.SourceGenerators (no cycle).
- `Directory.Packages.props` pins 2.0.0 and restore from nuget.org succeeds.
- `./bin/dev build`: 0 warnings, 0 errors. `./bin/dev test` passes. `ganda repo audit` passes.
- `<Version>` bumped only if `./bin/dev check-version` reports the current one as published.

## Requirements

1. Before changing the pin, find out what 2.0.0 changed against 1.0.2 (release notes, or diff
   the two packages with `tw-dotnet-inspect`) and record the breaking changes that touch this
   repo in `## Notes`.
2. Set `<PackageVersion Include="TimeWarp.Build.Tasks" Version="2.0.0" />` in
   `Directory.Packages.props`. Leave the `PackageReference` in `Directory.Build.props` as it is
   unless 2.0.0 requires a change.
3. Restore and build with the configured sources (nuget.org). If 2.0.0 does not restore, stop
   and report; do not add a NuGet feed.
4. Fix any build break or new diagnostic that 2.0.0 introduces. Do not suppress a diagnostic to
   get a green build.
5. TimeWarp.SourceGenerators and TimeWarp.Build.Tasks had a circular dependency that Build.Tasks
   1.0.2 removed (task 038). Confirm 2.0.0 does not bring back a dependency on
   TimeWarp.SourceGenerators. If it does, stop and report.
6. Bump `<Version>` in `source/Directory.Build.props` only when `./bin/dev check-version` says
   the current package version is already published.

## Checklist

- [x] Breaking changes in 2.0.0 that affect this repo are recorded in Notes
- [x] 2.0.0 has no dependency on TimeWarp.SourceGenerators
- [x] Pin is 2.0.0 and restore succeeds
- [x] `./bin/dev build` reports 0 warnings and 0 errors
- [x] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: 83d187ba-085b-471b-8964-44a61c5cebc6 (2026-11-12)
- Implement (ganda task work, 2026-11-12): diffed the 1.0.2 and 2.0.0 nupkgs from nuget.org,
  bumped the pin, bumped `<Version>` to 1.0.0-beta.15 (check-version said beta.14 is published),
  build/test/audit green. `ganda repo audit --fix` created the git-ignored
  `.vscode/settings.json` (vscode-window-icon advisory); nothing to commit from it.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 4ef5c673-c6f6-48e0-99f1-393325313651, max-turns 500, Done — 2026-11-12T03:54:22Z
- Review oracle round 1: cursor (grok-4.7), headless ganda task work, effort 1, roster general, max-turns uncapped (no --max-turns flag; bound 80). Disposition clean, 0 findings (2026-11-12)
- Review oracle: review by reviewer-cursor (cursor, vendor xai, model grok-4.7-high), session not reported, max-turns uncapped (no --max-turns flag; bound 80), artifacts general.md, merged.md, disposition.md — 2026-11-12T04:01:14Z
- Installed ganda changed during this walk: was 53237256 bytes 49cdc3169c48d40a92a1d5813475016b386b944fbd51455697d6991558d3fb25; now 53037848 bytes 49c33cf8d22938f1049d03697eba49973c4fb9832bfe044a7121e1b0211073b4

## Review

- Round 1: Done

## Notes

Requested by Steven 2026-11-12. Previous bump of this package: task 038 (1.0.0 -> 1.0.2).

### What 2.0.0 changed against 1.0.2

Found by downloading both nupkgs from nuget.org and diffing them (nuspec, `build/` and
`buildTransitive/` props, readme). Package commit 2.0.0 = `8dd25a7c`.

1. **Task assembly retargeted `netstandard2.0` -> `net11.0`.** The `UsingTask` now loads
   `net11.0/TimeWarp.Build.Tasks.dll` and only when `MSBuildRuntimeType == Core`.
   - Effect here: none. `global.json` pins SDK 11.0.100, CI uses `setup-dotnet` 11.0.x on
     ubuntu-latest, and everything builds with `dotnet build` via `./bin/dev`.
2. **New hard error `TWBT0001`** in target `TimeWarpSetGitMetadata` when MSBuild is not .NET Core
   or the runtime is older than 11.0. Visual Studio and `MSBuild.exe` (.NET Framework) builds now
   fail unless `TimeWarpEnableGitMetadata=false`.
   - Effect here: none for CLI/CI builds. A contributor who builds this repo in full Visual Studio
     would hit TWBT0001. The repo already requires the .NET 11 SDK, so this is acceptable; no
     change made.
3. No MSBuild property, target, or output was renamed or re-defaulted. `TimeWarpEnableGitMetadata`,
   `CommitHash`, `CommitDate`, and the `AssemblyMetadata` attributes are unchanged. Verified in the
   generated `AssemblyInfo.cs` after the build (CommitHash and CommitDate still injected).
4. **Dependencies:** the 2.0.0 nuspec has no `<dependencies>` (still `developmentDependency`), and
   the task DLL references only `Microsoft.Build.*` and `System.*`. No dependency on
   TimeWarp.SourceGenerators, so the cycle removed in task 038 does not come back.

No change was needed in `Directory.Build.props`; the `PackageReference` stays as it is.

## Results

- `Directory.Packages.props`: `TimeWarp.Build.Tasks` 1.0.2 -> 2.0.0. Restored from nuget.org
  (all three `project.assets.json` resolve `TimeWarp.Build.Tasks/2.0.0`).
- `source/Directory.Build.props`: `<Version>` 1.0.0-beta.14 -> 1.0.0-beta.15, because
  `./bin/dev check-version` reported beta.14 already published.
- `./bin/dev build`: 0 warnings, 0 errors. `./bin/dev test`: 56 passed, 0 failed.
  `./bin/dev check-version`: new version, safe to release. `ganda repo audit`: passes all checks
  (after `--fix` created the git-ignored `.vscode/settings.json`).
- No diagnostic suppressed; no code change needed.

### How to validate

Smoke:

```bash
./bin/dev build
./bin/dev test
grep -o '"TimeWarp.Build.Tasks/[0-9.]*"' source/timewarp-source-generators/obj/project.assets.json
grep -h CommitHash source/timewarp-source-generators/obj/Release/*/*AssemblyInfo.cs
```

Expect: build prints `0 Warning(s)` and `0 Error(s)`; tests print `Failed: 0`; the assets file
shows `"TimeWarp.Build.Tasks/2.0.0"`; the AssemblyInfo line carries a non-empty commit hash
(git metadata injection still works under 2.0.0).

### Review disposition

- **Outcome:** clean. 1 round, effort 1, roster `general`.
- **Final counts:** bug 0, suggestion 0, nit 0. Open 0, fixed 0, wontfix 0.
- The reviewer compared `## Goal (implementer restatement)` with the original task and found no misread. The 2.0.0 nupkg matches the Notes (net11.0 task, `TWBT0001`, unchanged git-metadata properties, no package dependencies). Build (0 warnings, 0 errors) and test (`56/56`) were re-run. No exceptions or escalations.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.
