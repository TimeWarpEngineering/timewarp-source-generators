# Review framework

## Budget (by-diff)

- Lines changed: 371
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-13
**Host task:** kanban/in-progress/041-retarget-the-source-generator-project-from-netstandard2-0-to-net11-0/
**Diff scope:** branch `task/041-retarget-the-source-generator-project-from` (`fb5facf9`) vs `master` (`cf2f4abd`). Product commit `cb48bb64`. Kanban land commits are out of the product review.
**Plan / brief:** `task.md` `## Goal (implementer restatement)` and `## Requirements`. Retarget `timewarp-source-generators.csproj` to `net11.0` with no multi-target and no `netstandard2.0` fallback, suppress only the wrong-target-framework Roslyn diagnostic, drop netstandard-only polyfills, keep the packed analyzer DLL under `analyzers/dotnet/cs`, document the Visual Studio limit, and commit it as a breaking change.
**Effort:** 2 (general only; by-diff roster)
**Reviewer roster:** general
**Session IDs:** review oracle (cursor, grok-4.7), headless ganda task work, max-turns uncapped (no `--max-turns` flag; bound 120)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
- Build, test, and audit stay on the implementer's recorded Results. This round does not re-run those gates.
