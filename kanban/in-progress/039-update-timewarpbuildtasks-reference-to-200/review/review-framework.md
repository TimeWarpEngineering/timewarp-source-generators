# Review framework

## Budget (by-diff)

- Lines changed: 161
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-12
**Host task:** kanban/in-progress/039-update-timewarpbuildtasks-reference-to-200/
**Diff scope:** branch `task/039-update-timewarpbuildtasks-reference-to-200` (`31fecf06`) vs `master`. Product commit `f5834efa` (`Directory.Packages.props`, `source/Directory.Build.props`). Kanban commits are out of the product review.
**Plan / brief:** `task.md` `## Goal (implementer restatement)` and `## Requirements`. Move `TimeWarp.Build.Tasks` from 1.0.2 to 2.0.0, record what the major bump changes, restore from nuget.org, keep a clean build and test with no suppressions, confirm 2.0.0 does not depend on `TimeWarp.SourceGenerators`, and bump `<Version>` only when the current package version is already published.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle (cursor, grok-4.7), headless ganda task work, max-turns uncapped (no `--max-turns` flag; bound 80)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
