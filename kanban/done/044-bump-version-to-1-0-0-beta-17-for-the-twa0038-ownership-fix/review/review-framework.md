# Review framework — task 044

**Date:** 2026-11-17
**Host task:** kanban/in-progress/044-bump-version-to-1-0-0-beta-17-for-the-twa0038-ownership-fix/
**Diff scope:** branch `task/044-bump-version-to-1-0-0-beta-17-for-the-twa0038` vs merge-base `ae6fc5c` (local `master`). Product files: `source/Directory.Build.props`, `documentation/releases.md`. Kanban move of task 044 is housekeeping on the same branch.
**Plan / brief:** `task.md` description, requirements, and `## Goal (implementer restatement)`. Move `<Version>` to `1.0.0-beta.17`, keep the task 040 TWA0038 notes under that heading, and record what `v1.0.0-beta.15..v1.0.0-beta.16` actually shipped.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** implementer-claude 56431ef2-3cd7-4bf8-bffe-a75e8b3146ac; reviewer grok 01a1e9a1-b181-70a3-9fe0-5b25965c6740

## Budget (by-diff)

- Lines changed: 131
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
