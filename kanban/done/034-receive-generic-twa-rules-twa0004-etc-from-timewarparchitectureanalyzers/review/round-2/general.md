# Round 2 — general
**Date:** 2026-11-10
**Scope reviewed:** fix delta `62a0489..e8c0ae8` (commit e8c0ae8 "fix: require a unique span for each TWA0023 flag"). Round-1 findings M1–M3 re-verified, and the delta scanned for new defects.

## Summary

The fix pass changes only the TWA0023 test harness, one Design-region comment, and the order of the global usings. `Locate` now throws when a snippet occurs twice. `Flag` throws when two expected flags resolve to the same span. The override/explicit test uses snippets that each occur once. Two new cases lock both throws. Risk is low and there is no analyzer logic change. `./bin/dev test` reran: 56 passed, 0 failed.

## Prior findings re-verified

- **M1 (suggestion): fixed.** `type-stem-identifier-test-fixture.cs:80` throws `Snippet is not unique` on a second occurrence. `:54` adds a `HashSet` of matched spans, so each expected flag must be a different diagnostic. In `OverrideAndExplicit_Should_ReportOnlyFreeNames`, `"  HttpClient Client { get; }"` (two leading spaces) now matches only the interface member: the abstract base line has `abstract HttpClient`, with one space before `HttpClient`. `"abstract void Run(HttpClient client)"` matches only the base, not the override. Both new guard tests passed.
- **M2 (nit): fixed.** `type-stem-identifier-analyzer.cs:26-27` says TWA0009 and TWA0021 live in TimeWarp.Architecture.Analyzers.
- **M3 (nit): fixed.** `global using System.Threading;` now follows `System.Text.RegularExpressions` in alphabetical order.

## Issues

None. Checked these and found no problem:
- The other `Flag` callers still pass under the new uniqueness rule: the full suite is green.
- `SameSpanTwice_Should_Throw` passes the count check (2 diagnostics, `Client` and `client`), then fails on the duplicate span as intended. It does not pass by accident through some other exception, because the `when` filter matches the message text `already matched`.
