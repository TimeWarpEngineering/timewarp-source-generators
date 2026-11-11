# Round 2 — merged findings
**Date:** 2026-11-11
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 1 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed (carried from round 1)
- File: .editorconfig:301 (removed)
- Description: the `ci-code-coverage` severity opt-out silenced the audit while CI collected no coverage.
- Source: general (round 1)
- Disposition notes: Re-verified on `7c04bdc`. The opt-out is gone. `dev test --coverage` restores pinned `dotnet-coverage` 18.12.0 and writes one Cobertura file per console; the workflow command sets `Coverage = true`; `workflow.yml` uploads `artifacts/coverage`. Suite passes under coverage, child exit codes propagate, and `ganda repo audit` reports `ci-code-coverage` PASS (42 passed, 0 failed).

### M2 — Severity: nit — Status: fixed
- File: kanban/in-progress/036-move-repo-to-net-11-ga/task.md (## Results)
- Description: Results attributed the report-total line-rate (89.3%) to package `timewarp-source-generators`; that package's line-rate is 74.1%.
- Suggestion: quote the package rate.
- Source: general
- Disposition notes: Reviewer corrected the kitchen Results text in round 2. No product change.

## Duplicates / conflicts

- None (single reviewer).
