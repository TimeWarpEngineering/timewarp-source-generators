# Update TimeWarp.Build.Tasks reference to 1.0.1

## Goal (implementer restatement)

The repo still pins `TimeWarp.Build.Tasks` at 1.0.0 in `Directory.Packages.props`. Move that central pin to 1.0.1 and leave the existing `PackageReference` in `Directory.Build.props` (`PrivateAssets="all"`) as it is. Restore and build using the sources already configured (nuget.org). If 1.0.1 is not on those sources, stop and report it; do not add a NuGet feed. Fix any build break or new diagnostic that 1.0.1 introduces. Bump `<Version>` in `source/Directory.Build.props` only when `./bin/dev check-version` says the current package version is already published.

Acceptance: the pin is `1.0.1` and restore succeeds; `./bin/dev test` passes; `ganda repo audit` is clean.

## Description

`TimeWarp.Build.Tasks` 1.0.1 is available. This repo pins 1.0.0 in `Directory.Packages.props`
(line 18); the reference itself is in `Directory.Build.props` (line 88, `PrivateAssets="all"`).

## Requirements

1. Set `<PackageVersion Include="TimeWarp.Build.Tasks" Version="1.0.1" />` in
   `Directory.Packages.props`.
2. Restore and build. TimeWarp packages may come from the org GitHub Packages feed
   (https://nuget.pkg.github.com/TimeWarpEngineering/index.json) as well as nuget.org. If 1.0.1
   does not restore from the configured sources, stop and report rather than adding a feed.
3. Fix anything 1.0.1 breaks or newly reports in the build.
4. Bump `<Version>` in `source/Directory.Build.props` only if the pre-PR version check requires it.

## Checklist

- [x] Pin is 1.0.1 and restore succeeds
- [x] `./bin/dev test` passes; `ganda repo audit` clean

## Scope change (2026-11-11)

Steven, 2026-11-11: TimeWarp.SourceGenerators and TimeWarp.Build.Tasks have a circular
dependency. Build.Tasks is dropping its source generator reference and will ship that as 1.0.2.
This task now targets **1.0.2**, not 1.0.1. Do not merge PR #48 at 1.0.1.

- [ ] `TimeWarp.Build.Tasks` 1.0.2 is published (not on nuget.org as of 2026-11-11; latest 1.0.1)
- [ ] Pin is 1.0.2 in `Directory.Packages.props`; restore, `./bin/dev test`, `ganda repo audit` pass
- [ ] PR #48 title and body say 1.0.2

## Session

- Created: c5b005f2-d490-4e08-a7fa-e19f46a0a9e6 (2026-11-11)
- Implementation: implementer oracle (2026-11-11). Pin moved to 1.0.1. Restore used nuget.org, the only configured source. Build reported 0 warnings. `./bin/dev test` passed 56 of 56. `./bin/dev check-version` says source `1.0.0-beta.14` is one prerelease ahead of NuGet `1.0.0-beta.13`, so `<Version>` stays. `ganda repo audit --fix` wrote a local gitignored `.vscode/settings.json` window title. The following audit passed 42 checks and failed 0. The Windows taskbar icon was not applied because this walk is WSL.
- Implement oracle: implemented by implementer-cursor (cursor, vendor xai, model grok-4.7-high), session not reported, max-turns uncapped (no --max-turns flag; bound 500), Done — 2026-11-11T08:02:45Z
- Review oracle round 1: cursor (gemini-4-carbon), headless ganda task work, effort 1, roster general, max-turns uncapped (no --max-turns flag; bound 80). Disposition clean, 0 findings (2026-11-11)
- Review oracle: review by reviewer-cursor (cursor, vendor google, model gemini-4-carbon-high), session not reported, max-turns uncapped (no --max-turns flag; bound 80), artifacts general.md, merged.md, disposition.md — 2026-11-11T08:12:31Z

## Review

- Round 1: Done

## Notes

Requested by Steven 2026-11-11.

## Results

`Directory.Packages.props` pins `TimeWarp.Build.Tasks` at 1.0.1. The `PackageReference` in `Directory.Build.props` is unchanged (`PrivateAssets="all"`). Restore resolved `TimeWarp.Build.Tasks/1.0.1` from nuget.org. `./bin/dev build` succeeded with 0 warnings and 0 errors. 1.0.1 did not add a diagnostic or a break, so no other product files changed. `./bin/dev check-version` exits 0 for `1.0.0-beta.14`, so `source/Directory.Build.props` keeps that version.

`./bin/dev test` printed `Passed: 56  Failed: 0  Total: 56`. `ganda repo audit` then passed every check (42 passed, 0 failed, 3 skipped).

### How to validate

**Smoke**

```bash
grep TimeWarp.Build.Tasks Directory.Packages.props
./bin/dev build
./bin/dev test
./bin/dev check-version
ganda repo audit
```

**Expect**

The pin line is `Version="1.0.1"`. Build prints `Build succeeded.` with `0 Warning(s)` and `0 Error(s)`. Tests print `Passed: 56  Failed: 0`. `check-version` prints `Version in source: 1.0.0-beta.14`, `Latest NuGet version: 1.0.0-beta.13`, and exits 0. `ganda repo audit` exits 0. A fresh tree may still warn on `vscode-window-icon` until `ganda repo audit --fix` writes the gitignored `.vscode/settings.json`.

**Not in scope:** publishing `TimeWarp.SourceGenerators`. The version check already passes, so this task does not bump it.

### Review disposition

- **Outcome:** clean. 1 round, effort 1, roster `general`.
- **Final counts:** bug 0, suggestion 0, nit 0. Open 0, fixed 0, wontfix 0.
- The reviewer verified the restatement against the original task (no misread), verified `TimeWarp.Build.Tasks` `1.0.1` metadata and `.nuspec` exclusion, and re-ran build (0 warnings, 0 errors), test (`56/56`), spike and runfile builds, and `ganda repo audit` (all 42 local checks passed). No exceptions or escalations.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.
