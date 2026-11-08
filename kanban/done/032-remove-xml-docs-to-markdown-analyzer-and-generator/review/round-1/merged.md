# Round 1 — merged findings
**Date:** 2026-11-09
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 0 | 0 |
| suggestion | 0 | 2 | 0 |
| nit | 0 | 1 | 0 |

## Gates verified by the reviewer

- `./bin/dev build`: succeeded, 0 warnings, 0 errors, packed `TimeWarp.SourceGenerators.1.0.0-beta.11.nupkg`
- `./bin/dev test`: printed the four console smoke lines; Passed 11, Failed 0
- The leftover-reference search outside `kanban/` and `spikes/` has no hits

## Issues

### M1 — Severity: suggestion — Status: fixed
- File: documentation/overview.md:20; documentation/developer/reference/overview.md:16; documentation/developer/reference/source-generators/overview.md:3; documentation/developer/conceptual/overview.md:14-15
- Description: The new docs say "This package does not ship a source generator." TW0001 `FileNameRuleAnalyzer` is still a `[Generator]` `IIncrementalGenerator` (source/timewarp-source-generators/file-name-rule-analyzer.cs:3-4). It reports diagnostics only and emits no source. The conceptual overview also dropped the accurate `IIncrementalGenerator` note.
- Suggestion: Say instead that the package generates no source. TW0001 runs as an incremental generator that only reports diagnostics, and TW0007 is a diagnostic analyzer. Use the same wording in all four places.
- Source: general
- Disposition notes: The four named pages now say the package generates no source. TW0001 runs as an incremental generator that only reports diagnostics, and TW0007 is a diagnostic analyzer. The wording is the same in each place.

### M2 — Severity: suggestion — Status: fixed
- File: documentation/developer/conceptual/analyzer-release-tracking.md:72-76
- Description: TW0002 and TW0003 must not be reused (task requirement). After the rows were removed, nothing in the product tree records that they are retired, so the gap between TW0001 and TW0007 invites reuse.
- Suggestion: Add a short "Retired ids" note: TW0002 (XmlDocsToMarkdownAnalyzer) and TW0003 (MarkdownDocsGenerator) were removed unshipped in task 032. Do not reuse them. Keep `AnalyzerReleases.Unshipped.md` as a table that the release-tracking analyzer accepts.
- Source: general
- Disposition notes: `analyzer-release-tracking.md` now has a Retired ids note for TW0002 and TW0003. `AnalyzerReleases.Unshipped.md` is unchanged and remains a rule table.

### M3 — Severity: nit — Status: fixed
- File: documentation/developer/conceptual/partial-properties-field-keyword.md:35
- Description: The doc says `field ?? ""` "satisfies definite assignment". The concern is nullable analysis (CS8618, a non-nullable property left null), not definite assignment.
- Suggestion: Reword it to say the getter never returns null, so nullable analysis does not warn.
- Source: general
- Disposition notes: The bullet now says the getter never returns null, so nullable analysis does not warn (CS8618).

## Duplicates / conflicts

- None. There was one reviewer, and its findings did not overlap.
