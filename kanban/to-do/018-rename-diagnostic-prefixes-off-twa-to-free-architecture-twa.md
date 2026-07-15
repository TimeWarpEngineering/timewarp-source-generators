# Rename diagnostic prefixes off TWA to free Architecture TWA

## Description

`timewarp-architecture` has renamed its diagnostic prefix **TWPA → TWA** (**T**ime**W**arp **A**rchitecture). Architecture IDs are now `TWA0001`–`TWA0010` (and will grow under `TWA####`).

This package currently uses **TWA001** / **TWA002** (plus **TW1001**–**TW1003** and **TWG001**), which **collides** with Architecture’s product prefix. Rename this package’s diagnostics to a **distinct** prefix before either product freezes IDs on NuGet.

## Context

| Product | Package | Intended prefix |
|---------|---------|-----------------|
| TimeWarp Architecture | `TimeWarp.Architecture.Analyzers` / `.Generators` | **TWA** = TimeWarp Architecture |
| This repo | `TimeWarp.SourceGenerators` | **Not TWA** — free TWA for Architecture |

Suggested target prefix (pick one and document):

- **TWS** = TimeWarp Source-generators / tools (recommended), or  
- **TW** only, with exclusive number ranges owned by this package  

Do **not** invent TWAA/TWAG role suffixes. Ecosystem convention is one product prefix + digits.

### Live IDs to rename (code today)

| Current | Feature |
|---------|---------|
| TWA001 | FileNameRuleAnalyzer (kebab-case) |
| TWA002 | XmlDocsToMarkdownAnalyzer |
| TW1001 | Interface delegation — class must be partial |
| TW1002 | Interface delegation — class doesn’t implement interface |
| TW1003 | Interface delegation — multiple fields for same interface |
| TWG001 | MarkdownDocsGenerator (Unshipped table) |

Stale kanban/docs still mention TW0001–TW0004 / TW0003 — clean those to the new IDs while at it.

## Requirements

- Single product prefix for this package (prefer **TWS####** for all rules, or document a single alternate).
- Update: analyzer/generator source, AnalyzerReleases.*.md, readme, documentation, .editorconfig samples, tests.
- No remaining `TWA####` IDs owned by this package.
- Note Architecture collision and chosen mapping in readme or overview.
- Build green after rename.

## Checklist

- [ ] Lock new prefix (recommend TWS) and ID mapping table
- [ ] Rename all DiagnosticId / descriptor IDs in source
- [ ] Update AnalyzerReleases.Shipped.md / Unshipped.md
- [ ] Update readme.md, documentation/, .editorconfig samples
- [ ] Fix stale TW000x references in kanban notes if desired
- [ ] Tests / manual verify diagnostics fire under new IDs
- [ ] Commit

## Notes

- Architecture rename commit (timewarp-architecture): `refactor(analyzers): rename diagnostic prefix TWPA to TWA`.
- Related: Architecture packages NuGet as `TimeWarp.Architecture.*` (task 092); this package stays `TimeWarp.SourceGenerators`.
- Worktree: `/home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-source-generators/Cramer-2026-06-30-dev`

## Session

- Created: 2026-07-15 (after Architecture TWPA→TWA; free TWA for that product)
