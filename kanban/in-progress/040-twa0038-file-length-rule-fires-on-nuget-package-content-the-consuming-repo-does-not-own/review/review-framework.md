# Review framework — task 040

**Date:** 2026-11-17
**Host task:** kanban/in-progress/040-twa0038-file-length-rule-fires-on-nuget-package-content-the-consuming-repo-does-not-own/
**Diff scope:** branch `task/040-twa0038-file-length-rule-fires-on-nuget-package` vs merge-base `7be134c` (3 commits, 478 lines). Product paths: `source/timewarp-source-generators/` (ownership helper, analyzer, build props, csproj), analyzer tests, `readme.md`, `documentation/releases.md`, `AnalyzerReleases.Unshipped.md`.
**Plan / brief:** TWA0038 should report only files the consuming repository owns. Skip NuGet package content, keep generated-code skipping, add `dotnet_diagnostic.TWA0038.exclude_paths`, and document the rule. See `## Goal (implementer restatement)` in `task.md` and the original Description.
**Effort:** 2 (general only; budget roster axes: general)
**Reviewer roster:** general
**Session IDs:** Grok 4.7 session `01a1e5e0-f8c4-7c10-9d1a-b2cc202f5db1` (review). Implementer session not reported (Claude Opus 5.5, 2026-11-16).

## Budget (by-diff)

- Lines changed: 478
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims by reading the diff and the kitchen `## Results` and `### How to validate` record
- Do not re-run a full build, a full test run, an audit, or the hooks; those belong to the implementer's recorded results, the host audit node, and CI
- Exception: one targeted command may check one suspected bug; the finding names the command and the reason. A full build, full test run, or audit is never that command
- A missing or thin recorded result is a finding, not a reason to run the gates
- Prior rounds are immutable; new work goes in `round-(N+1)/`
