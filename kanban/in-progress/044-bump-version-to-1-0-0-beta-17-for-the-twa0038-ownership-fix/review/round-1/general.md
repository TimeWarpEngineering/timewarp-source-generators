# Round 1 — general
**Date:** 2026-11-17
**Scope reviewed:** branch `task/044-bump-version-to-1-0-0-beta-17-for-the-twa0038` vs merge-base `ae6fc5c`. Product diff is `source/Directory.Build.props` and `documentation/releases.md`.

## Summary

The change sets `<Version>` to `1.0.0-beta.17` and moves the existing TWA0038 ownership notes under `## 1.0.0-beta.17` with that body left as it was. A new `## 1.0.0-beta.16` entry lists the dependency refresh that `v1.0.0-beta.15..v1.0.0-beta.16` shipped. The implementer restatement matches the original task. No misread. No product code changed.

`git diff v1.0.0-beta.15 v1.0.0-beta.16 -- Directory.Packages.props` shows exactly the pins the new beta.16 entry names: Roslyn `Microsoft.CodeAnalysis.Analyzers`, `Microsoft.CodeAnalysis.CSharp`, and `Microsoft.CodeAnalysis.CSharp.Workspaces` 5.9.0 to 5.12.0; `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` 4.0.0-beta.12 to 4.0.0-beta.13; `TimeWarp.Build.Tasks` 2.0.0 to 2.0.1-beta.1. The same range changes no analyzer source. `readme.md`, the analyzer reference pages, and `AnalyzerReleases.Unshipped.md` do not name beta.16 for the ownership fix or `exclude_paths`. The only other `<Version>` is `tools/dev-cli/source/Directory.Build.props` at `1.0.0-beta.1`, which this task does not own.

Recorded gates cover the diff: `./bin/dev check-version` safe to release, `./bin/dev build` 0 warnings and 0 errors packing `TimeWarp.SourceGenerators.1.0.0-beta.17.nupkg`, `./bin/dev test` 64/64, and `ganda repo audit` clean after `--fix` (43 passed, 0 failed). Those gates were not re-run.

## Issues

None.
