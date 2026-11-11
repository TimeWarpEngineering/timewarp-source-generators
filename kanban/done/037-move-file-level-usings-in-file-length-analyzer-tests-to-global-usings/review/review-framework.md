# Review framework

## Budget (by-diff)

- Lines changed: 73
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-11
**Host task:** kanban/in-progress/037-move-file-level-usings-in-file-length-analyzer-tests-to-global-usings/
**Diff scope:** branch `task/037-move-file-level-usings-in-file-length-analyzer-tes` vs `master` (product commit `4e63dc7`; kanban commits excluded from product review)
**Plan / brief:** `task.md` `## Goal (implementer restatement)` and `## Requirements`: move the three file-level usings from `file-length-analyzer-tests.cs` to `global-usings.cs` in alphabetical order, resolve any resulting ambiguity in the analyzer-tests project, and optionally move `TimeWarp.Terminal` in the partial-property spike to a `global-usings.cs`
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle (cursor, gemini-4-carbon), headless ganda task work, max-turns uncapped (no `--max-turns` flag; bound 80)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
