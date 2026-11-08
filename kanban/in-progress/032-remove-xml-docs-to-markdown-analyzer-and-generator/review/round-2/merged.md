# Round 2 — merged findings
**Date:** 2026-11-09
**Sources:** general

## Counts

Final counts across rounds 1 and 2 (carried IDs plus new):

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 0 | 0 |
| suggestion | 0 | 2 | 0 |
| nit | 0 | 1 | 0 |

## Gates verified by the reviewer

- `./bin/dev build`: succeeded, 0 warnings, 0 errors
- `./bin/dev test`: Passed 11, Failed 0

## Resolved prior

### M1 — Severity: suggestion — Status: fixed
- File: documentation/overview.md; documentation/developer/reference/overview.md; documentation/developer/reference/source-generators/overview.md; documentation/developer/conceptual/overview.md
- Disposition notes: Verified in `3185140`. All four pages say the package generates no source, that TW0001 runs as an incremental generator that only reports diagnostics, and that TW0007 is a diagnostic analyzer. This matches `file-name-rule-analyzer.cs:3-4` and `global-usings-rule-analyzer.cs:18`.

### M2 — Severity: suggestion — Status: fixed
- File: documentation/developer/conceptual/analyzer-release-tracking.md:82-84
- Disposition notes: Verified. The Retired ids note names TW0002 and TW0003 and says not to reuse them. `AnalyzerReleases.Unshipped.md` is unchanged and lists only TW0001 and TW0007.

### M3 — Severity: nit — Status: fixed
- File: documentation/developer/conceptual/partial-properties-field-keyword.md:37
- Disposition notes: Verified. The bullet now describes nullable analysis (CS8618).

## New issues

None.

## Duplicates / conflicts

- None.
