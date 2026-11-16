# Round 2 — general
**Date:** 2026-11-16
**Scope reviewed:** fix delta `0851645f42db5c5051a8fcf49bbf0c2a6cf221d1` .. `1accebacf3b1c2541d0b13ea919cf48ae5065fb4`. Product files: `.timewarp/dev.jsonc`, Design comment in `tools/dev-cli/endpoints/workflow-command.cs`. Recorded gates in `## Results` (fix pass) and `### How to validate` were read and not re-run.

## Summary

The fix declares `readme.md` and `assets/**` under `pullRequestBuild.buildPaths` and updates the Design comment that had called the omission intentional. In TimeWarp.Nuru.DevCli 4.0.0-beta.15, `BuildPaths` are applied before the default non-build roots, `assets/**` is the `assets` tree, and `readme.md` is the root file only. That covers the packed readme and `assets/logo.png`. The same tree also includes the unpacked `assets/timewarp-source-generators-avatar.svg`; that is the shape M1 asked for, and it fails closed. The recorded fix-pass gates are build, test, audit, and a `readme.md`-only spot check that printed `Build path changed: readme.md. Full build.` The Goal restatement now names these paths. The earlier misread stays in `## Learnings`. No remaining mismatch with requirements 1–7. No new issues.

## Resolved prior

### M1 — Severity: bug — Status: fixed
- File: .timewarp/dev.jsonc:14
- Description: `.timewarp/dev.jsonc` lists `readme.md` and `assets/**` with a comment that both are packed. `workflow-command.cs` lines 11–12 name those `pullRequestBuild.buildPaths` additions. `EntryRoot("assets/**")` is `assets`, and `IsUnder("assets/logo.png", "assets")` is true, so the default non-build root no longer skips the icon. `readme.md` matches the exact build-path entry, which overrides the default `.md` skip. The fix-pass record covers the config load: the spot-check message is the classifier's build-path line, which a root `.md` produces only when `BuildPaths` matches.
- Suggestion: none
- Status: fixed

## Issues

<!-- No new issues. -->
