# Review framework

## Budget (by-diff)

- Lines changed: 2122
- Effort: 3
- TCB hits: none
- Roster axes: general
- Turn cap: 200 (--max-turns; cursor uncapped)

## Framework — task 034

**Date:** 2026-11-10
**Host task:** kanban/in-progress/034-receive-generic-twa-rules-twa0004-etc-from-timewarparchitectureanalyzers/
**Diff scope:** branch `task/034-receive-generic-twa-rules-twa0004-etc-from-timewar` (HEAD 62a0489) vs merge-base with `master` (a752f8a)
**Plan / brief:** move TWA0004, TWA0023, and TWA0038 from TimeWarp.Architecture.Analyzers into TimeWarp.SourceGenerators with the same ids, defaults, and editorconfig keys; port the tests; update docs and release notes; set the version to 1.0.0-beta.13 (see `## Goal (implementer restatement)` in task.md)
**Effort:** 3 (by diff). Roster axis: general only
**Reviewer roster:** general (review oracle, Claude Opus 5.5)
**Session IDs:** review oracle under `ganda task work` (headless); implementer grok `01a1c422-c20a-75f1-9f7d-4fd4c78c2e0b`

### Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit. Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
