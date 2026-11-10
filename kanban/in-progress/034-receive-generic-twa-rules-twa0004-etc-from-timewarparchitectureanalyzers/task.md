# Receive generic TWA rules (TWA0004 etc.) from TimeWarp.Architecture.Analyzers

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

- [ ] Blocked until architecture 486 classification is done
- [ ] Port classified-generic analyzers (start with TWA0004; include others 486 marks generic)
- [ ] Preserve diagnostic IDs / aliases for `.editorconfig`
- [ ] Add analyzer tests
- [ ] Update docs / AnalyzerReleases / changelog
- [ ] Release a version consumers (including State task 153) can bump to

## Session

- Created: 1546826 (2026-11-09)

## Notes

Filed by Jenzi after Steven approved filing the three linked tasks (2026-11-09). Goal: remove the
false Architecture ↔ State cycle from the Core repo dependency tree by moving style-only rules
here.
