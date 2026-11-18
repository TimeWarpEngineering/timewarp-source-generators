# Review framework — task 045

**Date:** 2026-11-19
**Host task:** kanban/in-progress/045-refresh-outdated-nu-get-packages-in-time-warp-engineering-timewarp-source-generators/
**Diff scope:** branch `task/045-refresh-outdated-nu-get-packages-in-time-warp` vs `master` (`master...HEAD`)
**Plan / brief:** Bump the four dev-only TimeWarp pins in `Directory.Packages.props` (Nuru and Nuru.DevCli to 4.0.0-beta.16, Amuru and Amuru.Tools to 3.0.0-beta.10). Do not change `<Version>` in `source/Directory.Build.props`.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** grok session 01a1efa6-a317-7bc2-926f-64028ee5dfae (reviewer). Implementer session not reported.

## Budget (by-diff)

- Lines changed: 103
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
