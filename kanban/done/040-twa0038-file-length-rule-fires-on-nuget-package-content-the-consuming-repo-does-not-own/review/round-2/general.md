# Round 2 — general
**Date:** 2026-11-17
**Scope reviewed:** delta since round-1 base `1c32654687df0ac719a501d95408d073950da209` (commit `4b196de`). `IsOutsideRoots`, the one-key test assertions, the design comments, the readme sentence, and the analyzer reference pages.

## Summary

`IsOutsideRoots` now returns unowned only when both `ProjectDir` and `RepositoryRoot` are set and the rooted path is under neither. With either key absent the rule is off, so a linked file outside the project still warns and package content still depends on the package-root rule. The ProjectDir-only assertion now expects a warning, and the test adds the linked-file and RepositoryRoot-only cases. The reference "Not reported" list and the overview blurb name the package root, outside both roots, and `exclude_paths`. The restatement now matches the task's "outside ProjectDir and the repository root" wording, including the absent-key case. No new defects on this delta. I did not re-run build, test, or audit. The fix-pass record covers them: `./bin/dev build` 0/0, `./bin/dev test` 64/64, `ganda repo audit` pass.

## Issues

<!-- No new issues. Prior M1 and M2 are fixed; see merged.md. -->
