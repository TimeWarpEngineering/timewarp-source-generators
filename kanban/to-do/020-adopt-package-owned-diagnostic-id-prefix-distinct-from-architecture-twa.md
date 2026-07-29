# Adopt package-owned diagnostic ID prefix distinct from Architecture TWA

## Description

Roslyn diagnostic IDs for **this package** (`TimeWarp.SourceGenerators`) must never collide with
**TimeWarp.Architecture** analyzers (`TWA0001`–`TWA00xx`, plus generator ids `TWE*` / `SG*` in that
monorepo).

Consumers (Ganda docs, architecture AGENTS, older notes) have mixed references:
- **`TWA001`** — wrong / historical for the kebab-case file-name rule (Architecture owns `TWA*`)
- **`TW0001`** — what this repo **actually ships today** for `FileNameRuleAnalyzer`

This task freezes a **package-owned prefix SSOT**, renames all shipped ids if needed, and updates
docs / AnalyzerReleases / editorconfig keys so consumers configure the correct id.

### Current inventory (this branch)

| Id | Owner type | Rule |
|----|------------|------|
| **TW0001** | `FileNameRuleAnalyzer` | kebab-case `.cs` file names |
| **TW0002** | `XmlDocsToMarkdownAnalyzer` | XML docs → markdown |
| **TW0003** | `MarkdownDocsGenerator` | markdown docs gen / kebab file matching |
| **TW0004** | `InterfaceDelegationGenerator` | class must be partial |
| **TW0005** | `InterfaceDelegationGenerator` | class must implement delegated interface |
| **TW0006** | `InterfaceDelegationGenerator` | multiple fields delegate same interface |

Editorconfig custom key today: `dotnet_diagnostic.TW0001.excluded_files`.

### Why not `TWG`

`TWG` was a throwaway suggestion (“TimeWarp Generators”) and **does not read as a product name**.
Do **not** adopt it.

### Prefix options (pick one before implement)

| Prefix | Expansion | Pros | Cons |
|--------|-----------|------|------|
| **`TW`** (status quo) | TimeWarp (generic) | Already on all 6 rules; no consumer break if we only document | Too short; other future `TimeWarp.*` packages may want `TW*`; looks unrelated to package id |
| **`TWSG`** (recommended if renaming) | **T**ime**W**arp **S**ource **G**enerators | Matches NuGet / project name; unambiguous next to Architecture `TWA*` | Breaking rename of all ids + editorconfig keys; slightly long |
| **`TWS`** | TimeWarp Source…? | Shorter | Ambiguous (State? Software? Source?) — **reject** |

**Recommendation:** Prefer **`TWSG0001`…** if we want the id to **name the package** without colliding with Architecture. If we want **zero churn**, keep **`TW0001`…** and treat this task as **docs + external ref cleanup only** (Ganda `file-naming.md` still says `TWA001` in places).

Decide on the task before coding; record the choice in Notes.

## Requirements

- One documented SSOT for the package diagnostic prefix (readme + analyzer reference docs).
- All analyzer/generator diagnostic ids use that prefix consistently (code, `AnalyzerReleases.*`, docs, tests, sample `.editorconfig`).
- Distinct from Architecture: **no `TWA*` ids** in this package.
- If renaming: bump package version with release notes listing old → new id map (breaking for suppressions / editorconfig).
- External follow-ups (out of this repo, linked only):
  - timewarp-ganda `documentation/developer/standards/file-naming.md` (TWA001 wording)
  - timewarp-architecture AGENTS / task 133 notes if they still say SourceGenerators `TWA001` or opaque `TWG`

## Checklist

### Decision
- [ ] Choose prefix: keep **`TW`** *or* rename to **`TWSG`** (or other explicit expansion written in Notes)
- [ ] Write old→new id table in Notes / release notes draft

### Implementation (if rename)
- [ ] Update diagnostic ids in:
  - `file-name-rule-analyzer.cs` (+ `dotnet_diagnostic.*.excluded_files` key)
  - `xml-docs-to-markdown-analyzer.cs`
  - `markdown-docs-generator.cs`
  - `interface-delegation-generator.cs`
- [ ] Update `AnalyzerReleases.Unshipped.md` / Shipped as appropriate for Roslyn release tracking
- [ ] Grep repo for `TW000` / `TWA001` / old ids; fix docs, readme, how-tos, spikes, tests
- [ ] Package version bump + changelog / release note

### Implementation (if keep `TW`)
- [ ] Document in readme: **`TW*` = TimeWarp.SourceGenerators**; **not** Architecture `TWA*`
- [ ] Explicit “do not use TWA001 for this package” callout
- [ ] Still fix any internal stale `TWA001` / `TW0003` history inconsistencies if present

### Verify
- [ ] Package builds; sample / test console still reports expected ids
- [ ] No `TWA` diagnostic ids remain in this package’s shipped surface

## Notes

### Collision context

Architecture monorepo:

| Prefix | Meaning |
|--------|---------|
| **TWA** | TimeWarp **Architecture** convention analyzers (`TWA0001` partial-class shape, …) |
| **TWE** / **SG** | Architecture **generators** / resilience diagnostics (different package) |

SourceGenerators package must stay off that namespace. Historical confusion: Ganda docs and some
notes called the kebab rule **`TWA001`** even though this package uses **`TW0001`**.

### Origin of the ask

timewarp-architecture task **133** (kebab gaps / enforcement) and audit research flagged wiring
`FileNameRuleAnalyzer` and warned about Architecture `TWA*` collision. Suggested rename target
**`TWG`** was a poor acronym — superseded by this decision table.

### Related

- `documentation/developer/how-to-guides/configure-file-name-analyzer.md`
- `documentation/developer/reference/analyzers/file-name-rule-analyzer.md`
- timewarp-flow ADR-0013 (kebab adoption; mentions source-generator enforcement)

## Implementation Notes

_(empty until work starts)_
