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

## Results

Architecture 486's table marks TWA0004, TWA0023, and TWA0038 generic. Those three now live in `TimeWarp.SourceGenerators` under `TimeWarp.SourceGenerators`, with the same ids, default severities, and editorconfig keys. TWA0023 still matches `TypeStemIdentifierAttribute` by simple name. The attribute type stays in `TimeWarp.Architecture.Attributes`. No other TWA id moved.

Package version is `1.0.0-beta.13`. `dev check-version` reports source one prerelease ahead of NuGet `1.0.0-beta.12`. NuGet publish is `dev release` from a clean master after this pull request merges. Until that publish, Architecture.Analyzers still reports the same three ids, so this repo does not delete them there.

`./bin/dev test` passed: the smoke console ran, and the analyzer console passed 54 cases (0 failed). `ganda repo audit` exited 0 (41 passed, 0 failed, 3 skipped).

### How to validate

**Smoke**

```bash
./bin/dev test
grep -n 'TWA0004' source/timewarp-source-generators/AnalyzerReleases.Unshipped.md documentation/releases.md
grep -n 'DiagnosticId = "TWA0004"' source/timewarp-source-generators/purpose-region-analyzer.cs
grep -n 'DiagnosticId = "TWA0023"' source/timewarp-source-generators/type-stem-identifier-analyzer.cs
grep -n 'DiagnosticId = "TWA0038"' source/timewarp-source-generators/file-length-analyzer.cs
```

**Expect**

- `./bin/dev test` ends with `Tests completed successfully!` and `Passed: 54  Failed: 0`.
- Unshipped release tracking and `documentation/releases.md` name TWA0004, TWA0023, and TWA0038.
- The three analyzer files still use those diagnostic ids.
- `source/Directory.Build.props` has `<Version>1.0.0-beta.13</Version>`.

## Notes

Filed by Jenzi after Steven approved filing the three linked tasks (2026-11-09). Goal: remove the
false Architecture ↔ State cycle from the Core repo dependency tree by moving style-only rules
here.
