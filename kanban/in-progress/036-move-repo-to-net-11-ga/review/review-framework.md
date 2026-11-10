# Review framework

## Budget (by-diff)

- Lines changed: 108
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

# Review framework — task 036

**Date:** 2026-11-11
**Host task:** kanban/in-progress/036-move-repo-to-net-11-ga/
**Diff scope:** branch `task/036-move-repo-to-net-11-ga` (HEAD ae1d8c9) vs `master` merge base. 11 files: CI workflow, `Directory.Build.props`, `source/Directory.Build.props`, test and spike csproj files, spike `Program.cs` and `findings.md`, `documentation/releases.md`, `.editorconfig`, and the task kitchen.
**Plan / brief:** move the repo to the .NET 11 GA SDK (task.md `## Goal (implementer restatement)` and `## Requirements`)
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle (claude-opus-5-5, headless `ganda task work`)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit. Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
