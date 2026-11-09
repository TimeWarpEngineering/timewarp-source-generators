# Round 1 — general
**Date:** 2026-11-10
**Scope reviewed:** branch vs merge-base with `master`, product paths: `.gitignore`, `.timewarp/suppression-baseline.json`, `Directory.Packages.props`, `tools/dev-cli/**`

## Summary

The change does three things. It applies `ganda repo audit --fix`: it seeds the suppression baseline, ignores `/.timewarp/signals/`, and moves the dev CLI pins. It moves Roslyn to 5.9.0. It ports the four dev CLI handlers from the removed Mediator `ICommandHandler` to `IRequestHandler<T, Unit>` with `ValueTask<Unit>`. The implementer restatement matches the task, with no misread. Every gate in the task was re-run in this worktree and passed. Risk is low inside the repo. The one external risk is the minimum host compiler for consumers, which the task accepted on purpose (see Follow-ups).

## Verification (re-run by reviewer)

| Gate | Result |
|------|--------|
| `./bin/dev build` | exit 0, 0 warnings, 0 errors; generator output `netstandard2.0`; nupkg `1.0.0-beta.12` packed |
| `./bin/dev test` | exit 0, `Passed: 11  Failed: 0` |
| `./bin/dev --help` | lists `release` and `check-version` |
| `./bin/dev check-version` | `Packages checked: TimeWarp.SourceGenerators`; exit 1 "already released", which is the existing package version as Results says |
| `ganda repo audit` | exit 0, "Repository passes all audit checks." |
| `ganda nuget outdated` | "All packages are up to date!" (10/10) |
| `git ls-files '.timewarp/signals/*'` | empty |
| `.vscode/settings.json` | untracked, ignored by `.gitignore:341` |
| `.timewarp/suppression-baseline.json` | tracked |
| Root `PackageReference` to `TimeWarp.SourceGenerators` | none added |

## Checks against the task

- The `Directory.Packages.props` pins match the task's New column, and `TimeWarp.Terminal` is at `2.0.0-beta.1`. `audit --fix` made that bump and the live outdated run calls it current, so policy says keep it. Results records this.
- The handler port is mechanical and correct. `RunStepAsync` now takes `Func<ValueTask<Unit>>` and each step is awaited once, so there is no double-await of a `ValueTask`.
- Removing the `INuGetPackageService` and `IRepoCheckVersionService` registrations is safe. No code under `tools/` still references either type, the build is clean, and `check-version` runs. `NuGetPackageService = new()` in `WorkflowCommand.Handler` uses the parameterless constructor. The Design regions in `dev.cs` and `workflow-command.cs` explain the NURU051 reason.
- The suppression baseline was seeded by `audit --fix`, which is policy. It records existing suppressions and adds no new `#pragma` or `NoWarn` to the product.

## Issues

None.

## Follow-ups (not findings)

- **Consumer host-compiler floor.** The generator now compiles against `Microsoft.CodeAnalysis.CSharp` 5.9.0. A consumer whose compiler is older than Roslyn 5.9 (for example the .NET 10 SDK or an older Visual Studio) gets CS8032, and the analyzers do not load. The task requires this major bump ("Do not revert a major bump. The Roslyn 5.9.0 move is in scope."), so this is not a defect on this id. Before the next `TimeWarp.SourceGenerators` release, consider noting the minimum SDK or Visual Studio version in the readme or the release notes. In-tree validation runs on SDK 11.0.100-rc.3, where the build is clean.
