# Review framework

## Budget (by-diff)

- Lines changed: 201
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-10
**Host task:** kanban/in-progress/035-bring-ganda-repo-audit-clean-and-update-nuget-packages/
**Diff scope:** branch `task/035-bring-ganda-repo-audit-clean-and-update-nuget-pack` vs merge-base with `master` (commits `04e747a`, `214a89e`; kanban commits excluded from product review)
**Plan / brief:** task.md `## Goal (implementer restatement)` and `## Requirements`: audit exit 0, every outdated central package at the `ganda nuget outdated` New column, dev CLI builds on Nuru beta.12 / Mediator 15
**Effort:** 2 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle (claude, claude-opus-5-5), headless ganda task work

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
