# Round 2 — merged findings
**Date:** 2026-11-17
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 1 | 0 |
| nit | 0 | 0 | 0 |

## Resolved prior

### M1 — Severity: bug — Status: fixed
- File: source/timewarp-source-generators/source-file-ownership.cs:151
- Description: Round 1: `IsOutsideRoots` treated a file as unowned when it sat outside every non-null root, so `ProjectDir` alone silenced TWA0038 for owned linked files. The fix requires both `ProjectDir` and `RepositoryRoot`. If either is null the method returns false (owned, aside from the package-root and build-output rules). `OutsideRootsFile_Should_BeClean` expects a warning for `/opt/vendor/widget.cs` with only `ProjectDir`, for `/repo/source/shared/widget.cs` with only `ProjectDir`, and for `/opt/vendor/widget.cs` with only `RepositoryRoot`. The both-keys case still expects `/opt/vendor/widget.cs` to be clean. Design comments in `source-file-ownership.cs` and `file-length-analyzer.cs`, `readme.md`, and the restatement say "both".
- Suggestion: Applied as suggested.
- Source: general
- Disposition notes: Fixed in `4b196de`. Re-verified on the delta. No gate re-run; the fix-pass `## Results` record covers build, test 64/64, and audit.

### M2 — Severity: suggestion — Status: fixed
- File: documentation/developer/reference/analyzers/file-length-analyzer.md:28
- Description: Round 1: the reference "Not reported" list and the overview blurb omitted the package root, files outside both roots, and `exclude_paths`. The page now lists all three, states that the outside-roots rule applies only when both keys are visible, and shows an `exclude_paths` example. `overview.md` names the same skips.
- Suggestion: Applied as suggested.
- Source: general
- Disposition notes: Fixed in `4b196de`.

## Issues

<!-- No new findings. -->

## Duplicates / conflicts

- Single reviewer. Prior M1 and M2 carried forward. No new overlaps.
