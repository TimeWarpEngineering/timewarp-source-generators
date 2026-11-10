# Round 1 — merged findings
**Date:** 2026-11-11
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 1 | 0 |
| suggestion | 0 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: fixed
- File: .editorconfig:301
- Description: `ci-code-coverage.severity = off` silences the `ci-code-coverage` ganda audit check. The comment says only that `--fix` could not edit an MTP invocation. It does not say coverage is irrelevant: the dev-cli console-app suites can be measured, for example with `dotnet-coverage collect`. The line makes the audit look clean while CI still collects no coverage. It is also outside the .NET 11 move.
- Suggestion: Do one of these:
  - Add CI coverage around the dev-cli test run and drop the opt-out.
  - Remove the opt-out (the Warning does not block the audit) and file a follow-up task for coverage.
  - Have the operator decide coverage does not apply here. Then rewrite the comment with that reason and mark this finding `wontfix` with the rationale.
- Source: general
- Disposition notes: Removed `ci-code-coverage.severity = off`. The workflow test step sets Coverage on TestCommand, which restores pinned dotnet-coverage 18.12.0 and wraps each console run. Cobertura files land in `artifacts/coverage`. The workflow uploads that directory (`if-no-files-found: warn`). `dotnet run --file tools/dev-cli/dev.cs -- test --coverage` passed 56 of 56. `ganda repo audit` reports `ci-code-coverage` passed.

## Duplicates / conflicts

- None (single reviewer).
