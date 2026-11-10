# Round 1 — merged findings
**Date:** 2026-11-10
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 0 | 0 | 0 |
| suggestion | 1 | 0 | 0 |
| nit | 2 | 0 | 0 |

## Issues

### M1 — Severity: suggestion — Status: open
- File: tests/timewarp-source-generators-analyzer-tests/type-stem-identifier-exemptions-analyzer-tests.cs:170 (fixture `Locate`, type-stem-identifier-test-fixture.cs:65)
- Description: The third expected flag uses the snippet `HttpClient Client { get; }`. That text first occurs on the abstract base property (line 8), not on the interface member (line 20). So the test re-matches the line-8 diagnostic and no longer checks the interface location that the Architecture test pinned at 20:14-20. `Locate` does not enforce the "unique snippet" its Design region promises.
- Suggestion: Make `Locate` throw on a snippet that is not unique, and use a unique snippet for the interface member. Check that each expected diagnostic is matched by a different diagnostic.
- Source: general
- Disposition notes:

### M2 — Severity: nit — Status: open
- File: source/timewarp-source-generators/type-stem-identifier-analyzer.cs:26
- Description: The Design region cites TWA0009/TWA0021. Those are Architecture-only ids that this package does not ship.
- Suggestion: Drop the reference, or say those rules are in TimeWarp.Architecture.Analyzers.
- Source: general
- Disposition notes:

### M3 — Severity: nit — Status: open
- File: source/timewarp-source-generators/global-usings.cs:3
- Description: `global using System.Threading;` breaks the alphabetical order of the global usings.
- Suggestion: Move it into its alphabetical position.
- Source: general
- Disposition notes:

## Duplicates / conflicts

- None (single reviewer).

## Verified without findings

- Analyzer ports diffed against timewarp-architecture master cef55c09f: the logic is the same.
- Ids, severities, enabled defaults, categories, messages, and `dotnet_diagnostic.TWA0038.max_lines` are unchanged.
- The `TypeStemIdentifierAttribute` simple-name match is kept. There is no reference to Attributes.
- Test parity: the Architecture suite's 5 / 10 / 15 / 12 methods all have ported counterparts.
- `./bin/dev test`: 54 passed, 0 failed (run by the reviewer 2026-11-10).
- Docs, AnalyzerReleases.Unshipped, releases.md, and the readme state the move and the duplicate-report caveat. Version is 1.0.0-beta.13.
- `.githooks/*` and the workflow pwsh default come from audit --fix (policy). The other `run:` step is valid pwsh.
- The goal restatement matches the task. Requirement 4 (release) is deferred to `dev release` after merge, by convention. This is not a misread.
