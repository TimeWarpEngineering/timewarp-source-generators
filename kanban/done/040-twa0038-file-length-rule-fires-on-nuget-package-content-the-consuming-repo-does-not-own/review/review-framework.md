# Review framework — task 040

**Date:** 2026-11-17
**Host task:** kanban/in-progress/040-twa0038-file-length-rule-fires-on-nuget-package-content-the-consuming-repo-does-not-own/
**Diff scope:** Round 2 reviews the delta since round-1 base `1c32654687df0ac719a501d95408d073950da209` (commit `4b196de`, 153 lines). Product paths: `source/timewarp-source-generators/source-file-ownership.cs` (`IsOutsideRoots`), `file-length-analyzer.cs` (design comment), `tests/timewarp-source-generators-analyzer-tests/file-length-analyzer-tests.cs` (one-key assertions), `readme.md`, `documentation/developer/reference/analyzers/file-length-analyzer.md`, `documentation/developer/reference/analyzers/overview.md`. Round 1 reviewed the branch vs merge-base `7be134c`.
**Plan / brief:** Re-verify round-1 M1 and M2 against the task and `## Goal (implementer restatement)`, and scan the fix delta for new defects. The goal is that TWA0038 reports only files the consuming repository owns.
**Effort:** 1 (general only). Round 1 was effort 2, general only.
**Reviewer roster:** general
**Session IDs:** Round 2 Grok 4.7 session `01a1e5ee-efca-70a0-9826-29f2dcc0f939`. Round 1 Grok 4.7 session `01a1e5e0-f8c4-7c10-9d1a-b2cc202f5db1`. Implementer session not reported (Claude Opus 5.5, 2026-11-16).

## Budget (by-diff)

- Lines changed: 153
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
