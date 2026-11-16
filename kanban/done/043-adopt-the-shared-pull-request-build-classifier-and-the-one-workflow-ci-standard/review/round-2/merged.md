# Round 2 — merged findings
**Date:** 2026-11-16
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Resolved prior

### M1 — Severity: bug — Status: fixed
- File: .timewarp/dev.jsonc:14
- Description: Packed `readme.md` and `assets/logo.png` are declared under `pullRequestBuild.buildPaths` (`readme.md`, `assets/**`). The Design comment in `tools/dev-cli/endpoints/workflow-command.cs` (lines 11–12) names those additions. DevCli 4.0.0-beta.15 applies `BuildPaths` before the default non-build roots, and `assets/**` matches `assets/logo.png`. The fix-pass spot check for a `readme.md`-only pull request printed `Build path changed: readme.md. Full build.`
- Suggestion: none
- Source: general
- Disposition notes: fixed in `1accebacf3b1c2541d0b13ea919cf48ae5065fb4`

## Issues

<!-- No new issues. -->

## Duplicates / conflicts

- One reviewer. Nothing to collapse.
