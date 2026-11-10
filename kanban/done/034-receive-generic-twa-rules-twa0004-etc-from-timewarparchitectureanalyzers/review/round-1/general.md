# Round 1 — general
**Date:** 2026-11-10
**Scope reviewed:** branch vs master merge-base a752f8a (HEAD 62a0489), all 40 files

## Summary

The change moves TWA0004, TWA0023, and TWA0038 into this package. I diffed the three analyzers against `timewarp-architecture` master (cef55c09f). The ports are faithful: they differ only in namespace, `var` to explicit types, `name[1..]` to `Substring(1)` (netstandard2.0), and a shorter TWA0038 Design region. Ids, severities, enabled defaults, categories, messages, and the `max_lines` key all match. Every Architecture test method has a ported counterpart (5 / 10 / 15 / 12), and `./bin/dev test` passes 54 of 54. The docs, AnalyzerReleases.Unshipped, and releases.md say the ids moved and warn that a compilation with both packages reports each hit twice. The version is 1.0.0-beta.13. The `.githooks/*` and the pwsh workflow changes come from `ganda repo audit --fix` and are policy baseline. The workflow's other `run:` step (`echo "…"`) is valid pwsh.

One ported test lost an assertion that the Architecture version made, because its snippet is not unique. Two nits.

## Issues

### Issue 1 — Severity: suggestion
- File: tests/timewarp-source-generators-analyzer-tests/type-stem-identifier-exemptions-analyzer-tests.cs:170 (with type-stem-identifier-test-fixture.cs:65)
- Description: `OverrideAndExplicit_Should_ReportOnlyFreeNames` expects a third TWA0023 on the interface property `HttpClient Client { get; }` (line 20 of the snippet). `Locate` takes the first `IndexOf` match, and that text also occurs in `public abstract HttpClient Client { get; }` on line 8. So the third tuple re-finds the line-8 diagnostic. The flag count is still 3, but the test no longer checks which member the third diagnostic is on. If the analyzer flagged the explicit implementation `IHasClient.Client` instead of the interface member, this test would still pass. The Architecture test pinned it at `Feature.cs` 20:14-20. The acceptance criterion says the tests cover the cases the Architecture suite covered, so this assertion should be restored. The fixture's Design region says "Locate maps a unique snippet", but `Locate` does not check that the snippet is unique.
- Suggestion: Make `Locate` throw when the snippet occurs more than once (for example, compare `IndexOf` with `LastIndexOf`). Then give this case a unique snippet, such as `interface IHasClient\n{\n  HttpClient Client`, or the indented interface line. Fix any other case that the uniqueness check exposes. Also check that each expected diagnostic is matched by a different diagnostic.
- Status: open

### Issue 2 — Severity: nit
- File: source/timewarp-source-generators/type-stem-identifier-analyzer.cs:26
- Description: The Design region says "like TWA0009/TWA0021". Those ids are Architecture-specific and do not exist in this package, so a reader here cannot follow the reference.
- Suggestion: Write "Razor @code is out of v1 (GeneratedCodeAnalysisFlags.None)", or say that those rules are in TimeWarp.Architecture.Analyzers.
- Status: open

### Issue 3 — Severity: nit
- File: source/timewarp-source-generators/global-usings.cs:3
- Description: `global using System.Threading;` was placed between `System.Collections.Immutable` and `System.IO`, so the list is no longer in alphabetical order.
- Suggestion: Move it after `System.Text.RegularExpressions` (or wherever the repo's ordering puts it).
- Status: open
