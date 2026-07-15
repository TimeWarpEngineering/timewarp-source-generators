# Rename ALL diagnostic IDs to TW#### (single product prefix)

## Description

Rename **every** diagnostic ID in `TimeWarp.SourceGenerators` to **TW** + four digits
(`TW0001`–`TW0006` mapping below). Drop **TWA**, **TWG**, and **TW1xxx** entirely.

Architecture owns **TWA** = TimeWarp Architecture. This package owns **TW** = TimeWarp
(generic tooling / `TimeWarp.SourceGenerators`).

This is a deliberate full rename for product clarity — not an optional collision fix.

## Target prefix (locked recommendation)

**TW** = TimeWarp (this package / generic tooling). Short, already used for interface-delegation
IDs, matches ecosystem “product acronym + digits.” Architecture keeps **TWA**; this repo does
**not** use TWA at all.

One prefix for analyzers **and** generators. No TWAA/TWAG role suffixes.

### Full mapping (contiguous TW####)

| Current (leave none) | New | Feature |
|----------------------|-----|---------|
| TWA001 | **TW0001** | FileNameRuleAnalyzer (kebab-case) |
| TWA002 | **TW0002** | XmlDocsToMarkdownAnalyzer |
| TWG001 | **TW0003** | MarkdownDocsGenerator (if it still reports) |
| TW1001 | **TW0004** | Interface delegation — class must be partial |
| TW1002 | **TW0005** | Interface delegation — doesn’t implement interface |
| TW1003 | **TW0006** | Interface delegation — multiple fields same interface |

Also scrub historical **TW000x** kanban/doc references so they match the new table (not old
TW0003 = kebab under a different scheme).

## Requirements

- **Zero** remaining `TWA*`, `TW1xxx` (old 1001–1003), or `TWG*` diagnostic IDs in **live code**.
- Single family: **TW0001+** under this package only.
- Update: all DiagnosticId / descriptor IDs, AnalyzerReleases.*.md, readme, documentation,
  .editorconfig samples, tests, any help links.
- Document: **TW** = TimeWarp.SourceGenerators; **TWA** = TimeWarp Architecture (other product).
- Build green after rename.

## Checklist

- [ ] Apply TW0001–TW0006 mapping (or document deviation)
- [ ] Rename **all** IDs in source (no TWA / TWG / TW100x leftovers)
- [ ] AnalyzerReleases.Shipped.md / Unshipped.md
- [ ] readme.md, documentation/, .editorconfig samples
- [ ] Tests assert new IDs
- [ ] Grep: no TWA001, TWA002, TW1001–1003, TWG001 in code
- [ ] Commit

## Notes

- Architecture already renamed TWPA → TWA (timewarp-architecture).
- Prefix choice: **TW** (not TWS) — simpler brand for the generic generators package.
- Full rename is intentional product hygiene, not “only if IDs collide.”
- Worktree: `/home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-source-generators/Cramer-2026-06-30-dev`

## Session

- Created: 2026-07-15
- Clarified: rename **ALL** IDs; do not leave TWA001/TWA002
- Prefix locked: **TW** (recommend contiguous TW0001–TW0006)
