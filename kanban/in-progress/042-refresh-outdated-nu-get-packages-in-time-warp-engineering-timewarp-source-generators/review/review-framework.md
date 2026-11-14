# Review framework — task 042

**Date:** 2026-11-14
**Host task:** kanban/in-progress/042-refresh-outdated-nu-get-packages-in-time-warp-engineering-timewarp-source-generators/
**Diff scope:** branch `task/042-refresh-outdated-nu-get-packages-in-time-warp` vs merge-base `b42f1e045981d7dff4a5b09d5c9b57a9e30cbc4e` (`master` at branch point). Product commits `c7b9bd4` and `348ba41`. Product paths: `Directory.Packages.props`, `source/Directory.Build.props`. Local `master` is two commits ahead (`e945cc6`, `56ce05b`); those commits only add and move this task file.
**Plan / brief:** `task.md` description and `## Goal (implementer restatement)`. Bump the five named central pins and set `<Version>` to `1.0.0-beta.16`.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** implementer-claude, session not reported (2026-11-14); reviewer grok `01a1d8fb-2b4d-7a41-9a94-8d4d7de72796` (2026-11-14)

## Budget (by-diff)

- Lines changed: 87
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
