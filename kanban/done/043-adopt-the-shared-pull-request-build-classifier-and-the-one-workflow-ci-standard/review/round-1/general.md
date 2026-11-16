# Round 1 — general
**Date:** 2026-11-16
**Scope reviewed:** branch `task/043-adopt-the-shared-pull-request-build-classifier-and` vs `master` (three-dot). Product files in the diff, plus the pack inputs and `.timewarp/dev.jsonc` the classifier reads. Recorded gates in `## Results` and `### How to validate` were read and not re-run.

## Summary

The pull-request path calls `PullRequestBuildClassifier.DecideAsync(RepoRoot, Ct)`, prints `decision.Message`, and returns while `Environment.ExitCode` is still 0 when `RequiresBuild` is false. Release stays on `RunReleaseWorkflowAsync`. Push and other non-`pull_request` events go through the same method, and the pinned classifier (TimeWarp.Nuru.DevCli 4.0.0-beta.15) answers build for every event that is not `pull_request`. `workflow.yml` drops `paths` on `pull_request`, keeps `branches: [master, 'feature/**']`, keeps `fetch-depth: 0`, and keeps the `push` paths list with a why comment. Pins are Nuru and DevCli 4.0.0-beta.15. The recorded gates cover the change: `./bin/dev build` (0 warnings, 0 errors), `./bin/dev test` (56 passed, 0 failed), and `ganda repo audit` passing `workflow-file` and `dev-workflow-classifier`. No local copy of the rule is in `tools/dev-cli/`. The gap is requirement 5: the package packs `readme.md` and `assets/logo.png`, and the default rule treats both as non-build, but `.timewarp/dev.jsonc` declares no `pullRequestBuild` additions.

## Issues

### Issue 1 — Severity: bug
- File: .timewarp/dev.jsonc:1
- Description: Requirement 5 says build inputs outside `source`, `tests`, `tools`, and `msbuild` go in `PullRequestBuildConfig`. `source/timewarp-source-generators/timewarp-source-generators.csproj` packs `$(RepositoryRoot)readme.md`. `source/Directory.Build.props` sets `PackageReadmeFile` to `readme.md` and `PackageIcon` to `logo.png`, and packs `$(RepositoryRoot)assets/logo.png`. In TimeWarp.Nuru.DevCli 4.0.0-beta.15, `assets` is a default non-build root and a path ending in `.md` outside the four protected roots is non-build. `.timewarp/dev.jsonc` has only `checkVersionConfig`, so a pull request that changes the packed readme or the packed icon skips clean, build, and test. The Design comment in `tools/dev-cli/endpoints/workflow-command.cs` (lines 11–12) records that omission as intentional. The same requirement on timewarp-build-tasks 013 and timewarp-nuru 595 declares `readme.md` and the packed icon path under `pullRequestBuild.buildPaths`. The `push` paths list was left as the task requires and is outside this finding.
- Suggestion: Add `pullRequestBuild.buildPaths` for `readme.md` and `assets/**` (the packed icon lives at `assets/logo.png`) in `.timewarp/dev.jsonc`, with a comment that those files are packed. Update the Design comment in `workflow-command.cs` so it names those additions.
- Status: open
