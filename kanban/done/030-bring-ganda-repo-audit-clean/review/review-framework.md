# Review framework

## Budget (by-diff)

- Lines changed: 1590
- Effort: 3
- TCB hits: none
- Roster axes: general
- Turn cap: 200 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-08
**Host task:** kanban/in-progress/030-bring-ganda-repo-audit-clean/
**Diff scope:** branch `task/030-bring-ganda-repo-audit-clean` (HEAD `36a03b1`) vs merge-base `c225708` with `origin/master`
**Plan / brief:** task.md `## Goal (implementer restatement)` and `## Requirements`. Make `ganda repo audit` pass, move Nuru/DevCli to 4.0.0-beta.11, split `markdown-docs-generator.cs` under 500 lines, and turn `global-usings-analyzer` off with a reason comment
**Effort:** 3 (by-diff budget); roster axis general only
**Reviewer roster:** general
**Session IDs:** review oracle Claude (claude-opus-5-5), ganda task work headless

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
- `ganda repo audit --fix` output (hook scaffolds, glossary template, package pins) is policy baseline, not a finding
