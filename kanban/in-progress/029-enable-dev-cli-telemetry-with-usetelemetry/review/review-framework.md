# Review framework

## Budget (by-diff)

- Lines changed: 222
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Scope

**Date:** 2026-11-09
**Host task:** kanban/in-progress/029-enable-dev-cli-telemetry-with-usetelemetry/
**Diff scope:** branch `task/029-enable-dev-cli-telemetry-with-usetelemetry` (HEAD 928bc7f) vs merge-base with `master` (836f59b). Product files: `tools/dev-cli/dev.cs`, `tools/dev-cli/endpoints/incoming-trace-context.cs`, `tools/dev-cli/Directory.Build.props`, `tools/dev-cli/telemetry.md`. Kanban move of `task.md` (to-do → in-progress) is bookkeeping only.
**Plan / brief:** `task.md` `## Goal (implementer restatement)` and `## Requirements`: `UseTelemetry` with literal service name, `IncomingTraceContext.Restore()` around `RunAsync`, no repo ActivitySource, `telemetry.md`, capabilities/build/AOT still work.
**Effort:** 2 (by-diff budget), roster axes: general
**Reviewer roster:** general (review oracle, claude-opus-5-5)
**Session IDs:** review oracle session (claude, headless `ganda task work`), 2026-11-09

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
