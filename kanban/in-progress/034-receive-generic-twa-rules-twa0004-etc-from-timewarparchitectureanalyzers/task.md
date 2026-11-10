# Receive generic TWA rules (TWA0004 etc.) from TimeWarp.Architecture.Analyzers

## Goal (implementer restatement)

Architecture task 486 finished the classification. Three rules are generic and move here unchanged: TWA0004 (every non-generated C# file has a `#region Purpose` block; warning, on), TWA0023 (an identifier ends with its type stem, interfaces drop a leading `I`; warning, off), and TWA0038 (a file over 500 lines; warning, on; `dotnet_diagnostic.TWA0038.max_lines` overrides the limit). Every other TWA id stays in TimeWarp.Architecture.Analyzers. The opt-out attribute for TWA0023 stays in TimeWarp.Architecture.Attributes and is matched by the simple name `TypeStemIdentifierAttribute`.

Acceptance: this package reports those three ids with the same defaults and editorconfig keys; tests cover the cases the Architecture suite covered; AnalyzerReleases, the analyzer reference, and the changelog say the ids moved from TimeWarp.Architecture.Analyzers; the package version is `1.0.0-beta.13`. NuGet publish is `dev release` from a clean master after this pull request merges. This walk does not publish.

## Description

Host the generic C# code-style analyzers that are moving out of
`TimeWarp.Architecture.Analyzers` (`timewarp-architecture` convention analyzers package).

`TimeWarp.SourceGenerators` already ships generic style rules (for example TW0007) and is already
referenced by all core repos. Receiving the moved TWA rules here lets non-Architecture repos
(especially `timewarp-state`) drop their Architecture.Analyzers reference while keeping the same
diagnostic IDs in `.editorconfig`.

## Depends on (cross-repo)

Blocked by **timewarp-architecture 486** classification: do not move any analyzer until 486's
inventory table marks it **generic**. Candidates called out there include at least TWA0004
(purpose region) and TWA0038 (file length); the full move set is 486's first deliverable.

## Related tasks

- `timewarp-architecture` **486** — Split TimeWarp.Architecture.Analyzers (classification + move out)
- `timewarp-state` **153** — Drop Architecture.Analyzers once this package releases the moved rules

## Requirements

1. Wait for architecture 486's completed classification table (generic vs Architecture-specific).
2. Port the **generic** analyzers into this repo (tests included).
3. Keep the existing TWA diagnostic IDs (or ship aliases / a deprecation period that preserves
   `.editorconfig` keys).
4. Release a version of `TimeWarp.SourceGenerators` that includes the moved rules.
5. Document the new IDs alongside existing TW* rules and note the migration from
   Architecture.Analyzers.

## Checklist

- [x] Blocked until architecture 486 classification is done
- [x] Port classified-generic analyzers (TWA0004, TWA0023, TWA0038)
- [x] Preserve diagnostic IDs / aliases for `.editorconfig`
- [x] Add analyzer tests
- [x] Update docs / AnalyzerReleases / changelog
- [ ] Release a version consumers (including State task 153) can bump to

## Session

- Created: 1546826 (2026-11-09)
- Implementation: grok `01a1c422-c20a-75f1-9f7d-4fd4c78c2e0b` (2026-11-10). Ported the three generic analyzers. Audit `--fix` refreshed `.githooks/*` to the attest baseline and wrote a gitignored `.vscode/settings.json` window title. The CI `Run CI Pipeline` step is pwsh, with top-level `defaults.run.shell`.
- Implement oracle: implemented by implementer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1c422-c20a-75f1-9f7d-4fd4c78c2e0b, max-turns 500, Done — 2026-11-10T06:47:28Z
- Review oracle round 1: Claude Opus 5.5 (headless `ganda task work`). Effort 3, general reviewer only, turn cap 200. 2026-11-10. Three findings open (M1 suggestion, M2 and M3 nits). A fix pass was requested.
- Review oracle: review by reviewer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 89dcd2ef-ee8f-4092-a9a3-71ae2a26f662, max-turns 200, artifacts general.md, merged.md — 2026-11-10T06:51:41Z
- Fix pass (implement-fix-2, 2026-11-10): M1 makes `Locate` throw when a snippet occurs twice and makes `Flag` refuse a second match of the same span. The override test now uses unique snippets for the abstract `Run` parameter and the interface `Client` property. Two cases lock those throws. M2 names TimeWarp.Architecture.Analyzers for TWA0009 and TWA0021. M3 moves `global using System.Threading` after `System.Text.RegularExpressions`. `./bin/dev test`: Passed 56, Failed 0.
- Review oracle round 2: Claude Opus 5.5 (`claude-opus-5-5`, headless `ganda task work`). Effort 2, general reviewer only, turn cap 120. 2026-11-10. Reviewed the delta `62a0489..e8c0ae8`. M1, M2, and M3 are fixed, with 0 open and no new findings. Disposition `clean`.

## Results

Architecture 486's table marks TWA0004, TWA0023, and TWA0038 generic. Those three now live in `TimeWarp.SourceGenerators` under `TimeWarp.SourceGenerators`, with the same ids, default severities, and editorconfig keys. TWA0023 still matches `TypeStemIdentifierAttribute` by simple name. The attribute type stays in `TimeWarp.Architecture.Attributes`. No other TWA id moved.

Package version is `1.0.0-beta.13`. `dev check-version` reports source one prerelease ahead of NuGet `1.0.0-beta.12`. NuGet publish is `dev release` from a clean master after this pull request merges. Until that publish, Architecture.Analyzers still reports the same three ids, so this repo does not delete them there.

`./bin/dev test` passed: the smoke console ran, and the analyzer console passed 54 cases (0 failed). `ganda repo audit` exited 0 (41 passed, 0 failed, 3 skipped).

### Review

**Disposition: clean.** There were 2 rounds, each with one general reviewer. Round 1 ran at effort 3 and round 2 at effort 2. Final counts: bug 0, suggestion 1 fixed, nit 2 fixed, 0 open, 0 wontfix. Round 1 (2026-11-10) raised M1, M2, and M3, and fix pass e8c0ae8 implemented them. Round 2 (2026-11-10) verified each fix on the delta `62a0489..e8c0ae8` and found no new issues. `./bin/dev test` reran with 56 passed, 0 failed.

- M1: `Locate` throws when the snippet occurs more than once. `Flag` throws when a second expected flag resolves to a span already matched. `OverrideAndExplicit_Should_ReportOnlyFreeNames` uses `abstract void Run(HttpClient client)` and `  HttpClient Client { get; }` so the abstract parameter and the interface property are each checked once. `DuplicateSnippet_Should_Throw` and `SameSpanTwice_Should_Throw` lock those throws. The abstract `Run` snippet was also shared with the override; that flag is unique now too.
- M2: the TWA0023 Design region says TWA0009 and TWA0021 live in TimeWarp.Architecture.Analyzers.
- M3: `global using System.Threading;` sits after `System.Text.RegularExpressions`.
- The analyzer ports match timewarp-architecture cef55c09f. Round 1 reran `./bin/dev test` at 54 passed. After this fix pass the analyzer console is 56 passed, 0 failed, and `ganda repo audit` exited 0 (41 passed, 0 failed, 3 skipped).
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/round-2/general.md`, `review/round-2/merged.md` (final ledger), `review/disposition.md`.

### How to validate

**Smoke**

```bash
./bin/dev test
grep -n 'TWA0004' source/timewarp-source-generators/AnalyzerReleases.Unshipped.md documentation/releases.md
grep -n 'DiagnosticId = "TWA0004"' source/timewarp-source-generators/purpose-region-analyzer.cs
grep -n 'DiagnosticId = "TWA0023"' source/timewarp-source-generators/type-stem-identifier-analyzer.cs
grep -n 'DiagnosticId = "TWA0038"' source/timewarp-source-generators/file-length-analyzer.cs
grep -n 'Snippet is not unique' tests/timewarp-source-generators-analyzer-tests/type-stem-identifier-test-fixture.cs
grep -n 'TimeWarp.Architecture.Analyzers' source/timewarp-source-generators/type-stem-identifier-analyzer.cs
grep -n 'global using' source/timewarp-source-generators/global-usings.cs
```

**Expect**

- `./bin/dev test` ends with `Tests completed successfully!` and `Passed: 56  Failed: 0`.
- Unshipped release tracking and `documentation/releases.md` name TWA0004, TWA0023, and TWA0038.
- The three analyzer files still use those diagnostic ids.
- `source/Directory.Build.props` has `<Version>1.0.0-beta.13</Version>`.
- `Locate` throws with `Snippet is not unique`.
- The TWA0023 Design region names TimeWarp.Architecture.Analyzers for TWA0009 and TWA0021.
- `global using System.Threading;` follows `global using System.Text.RegularExpressions;`.

## Review

- Round 1: findings M1, M2, M3 implemented on this fix pass.
- Round 2: Done
## Notes

Filed by Jenzi after Steven approved filing the three linked tasks (2026-11-09). Goal: remove the
false Architecture ↔ State cycle from the Core repo dependency tree by moving style-only rules
here.
