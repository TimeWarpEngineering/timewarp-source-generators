# Review framework

## Budget (by-diff)

- Lines changed: 63
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-11
**Host task:** kanban/in-progress/038-update-timewarpbuildtasks-reference-to-101/
**Diff scope:** branch `task/038-update-timewarpbuildtasks-reference-to-101` vs `master` (product commit `f151503`; kanban commits excluded from product review)
**Plan / brief:** `task.md` `## Goal (implementer restatement)` and `## Requirements`: update `<PackageVersion Include="TimeWarp.Build.Tasks" />` in `Directory.Packages.props` from `1.0.0` to `1.0.1`, keep `<PackageReference Include="TimeWarp.Build.Tasks" PrivateAssets="all" />` in `Directory.Build.props`, restore and build from configured NuGet sources (`nuget.org`) without adding feeds, fix any build break or new diagnostic introduced by `1.0.1`, and bump `<Version>` in `source/Directory.Build.props` only if `./bin/dev check-version` requires it
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** review oracle (cursor, gemini-4-carbon), headless ganda task work, max-turns uncapped (no `--max-turns` flag; bound 80)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
