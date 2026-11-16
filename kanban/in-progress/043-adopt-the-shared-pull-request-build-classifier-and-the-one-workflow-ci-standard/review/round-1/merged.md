# Round 1 — merged findings
**Date:** 2026-11-16
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 1 | 0 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: open
- File: .timewarp/dev.jsonc:1
- Description: Requirement 5 says build inputs outside `source`, `tests`, `tools`, and `msbuild` go in `PullRequestBuildConfig`. `source/timewarp-source-generators/timewarp-source-generators.csproj` packs `$(RepositoryRoot)readme.md`. `source/Directory.Build.props` sets `PackageReadmeFile` to `readme.md` and `PackageIcon` to `logo.png`, and packs `$(RepositoryRoot)assets/logo.png`. In TimeWarp.Nuru.DevCli 4.0.0-beta.15, `assets` is a default non-build root and a path ending in `.md` outside the four protected roots is non-build. `.timewarp/dev.jsonc` has only `checkVersionConfig`, so a pull request that changes the packed readme or the packed icon skips clean, build, and test. The Design comment in `tools/dev-cli/endpoints/workflow-command.cs` (lines 11–12) records that omission as intentional. The same requirement on timewarp-build-tasks 013 and timewarp-nuru 595 declares `readme.md` and the packed icon path under `pullRequestBuild.buildPaths`. The `push` paths list was left as the task requires and is outside this finding.
- Suggestion: Add `pullRequestBuild.buildPaths` for `readme.md` and `assets/**` (the packed icon lives at `assets/logo.png`) in `.timewarp/dev.jsonc`, with a comment that those files are packed. Update the Design comment in `workflow-command.cs` so it names those additions.
- Source: general
- Disposition notes:

## Duplicates / conflicts

- One reviewer. Nothing to collapse.
