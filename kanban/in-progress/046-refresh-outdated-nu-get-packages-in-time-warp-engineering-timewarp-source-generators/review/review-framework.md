# Review framework — task 046

**Date:** 2026-11-26
**Host task:** kanban/in-progress/046-refresh-outdated-nu-get-packages-in-time-warp-engineering-timewarp-source-generators/
**Diff scope:** branch `task/046-refresh-outdated-nu-get-packages-in-time-warp` vs `master` (`master...HEAD`): `Directory.Packages.props` and the task kitchen. Product change is the two Nuru pins.
**Plan / brief:** `task.md` description and `## Goal (implementer restatement)`. Bump dev-only `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` in `Directory.Packages.props` from 4.0.0-beta.16 to 4.0.0-beta.17. Do not change `<Version>` in `source/Directory.Build.props`.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** grok session 01a215b2-67be-77a0-abce-d526f70215a8 (reviewer). Implementer: claude session d919282a-a5e4-44aa-9b2b-2d1317b55902.

## Budget (by-diff)

- Lines changed: 46
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims by reading the diff and the kitchen `## Results` and `### How to validate` record
- Do not re-run a full build, a full test run, an audit, or the hooks; those belong to the implementer's recorded results, the host audit node, and CI
- Exception: one targeted command may check one suspected bug; the finding names the command and the reason. A full build, full test run, or audit is never that command
- A missing or thin recorded result is a finding, not a reason to run the gates
- Prior rounds are immutable; new work goes in `round-(N+1)/`
