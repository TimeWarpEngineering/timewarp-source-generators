# Round 2 — merged findings
**Date:** 2026-11-10
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 0 | 0 |
| suggestion | 0 | 1 | 0 |
| nit | 0 | 2 | 0 |

## Issues

### M1 — Severity: suggestion — Status: fixed
- File: tests/timewarp-source-generators-analyzer-tests/type-stem-identifier-test-fixture.cs:80, type-stem-identifier-exemptions-analyzer-tests.cs:170
- Description: (round 1) Non-unique snippet let the interface-member flag re-match the base property.
- Source: general (round 1)
- Disposition notes: Fixed in e8c0ae8. `Locate` throws on a duplicate snippet. `Flag` rejects a span that was already matched. The snippets are unique, and two guard tests were added. Verified in round 2.

### M2 — Severity: nit — Status: fixed
- File: source/timewarp-source-generators/type-stem-identifier-analyzer.cs:26
- Description: (round 1) Design region cited TWA0009/TWA0021 without saying they are Architecture-only.
- Source: general (round 1)
- Disposition notes: Fixed in e8c0ae8. The comment names TimeWarp.Architecture.Analyzers. Verified in round 2.

### M3 — Severity: nit — Status: fixed
- File: source/timewarp-source-generators/global-usings.cs:6
- Description: (round 1) `System.Threading` was out of alphabetical order.
- Source: general (round 1)
- Disposition notes: Fixed in e8c0ae8. Verified in round 2.

## New findings

None.

## Duplicates / conflicts

- None (single reviewer).

## Verification

- `./bin/dev test`: Passed 56, Failed 0 (reviewer rerun 2026-11-10 at HEAD e8c0ae8).
