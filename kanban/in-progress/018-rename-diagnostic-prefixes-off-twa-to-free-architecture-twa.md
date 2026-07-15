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

### Implementation plan (2026-07-15)

**Confirmed:** TWG001 still reports on generator load — rename to TW0003, do not drop.

#### Live source

| File | Change |
|------|--------|
| `file-name-rule-analyzer.cs` | TWA001→TW0001; also `dotnet_diagnostic.TW0001.excluded_files` config key |
| `xml-docs-to-markdown-analyzer.cs` | TWA002→TW0002 |
| `markdown-docs-generator.cs` | TWG001→TW0003 |
| `interface-delegation-generator.cs` | TW1001→TW0004, TW1002→TW0005, TW1003→TW0006 |
| `AnalyzerReleases.Unshipped.md` | Replace all six rows with TW0001–TW0006 |
| `AnalyzerReleases.Shipped.md` | Leave empty (always-unshipped policy) |

#### Config / docs / open kanban

1. Root + test-console `.editorconfig` (TWA001/TWA002 → TW0001/TW0002)
2. `readme.md` + TW vs TWA product-prefix note
3. `documentation/**` — especially `configure-file-name-analyzer.md` (old TW0003=kebab → **TW0001**)
4. Open tasks 015, 016, 017 (fix + interface draft) — scrub wrong scheme IDs
5. Leave `kanban/done/*` history as-is (optional one-line notes only)

#### Order

1. Source descriptors + Unshipped (same change set — RS2008)
2. EditorConfig + readme
3. documentation/
4. Open kanban scrub
5. Build + grep verify
6. Commit

#### Verification

```bash
dotnet build timewarp-source-generators.slnx -c Release
dotnet build tests/timewarp-source-generators-test-console/ -c Release
rg -n 'TWA00[12]|TWG001|TW100[1-3]' source/ documentation/ readme.md .editorconfig tests/
rg -n 'TW000[1-6]' source/timewarp-source-generators/
```

Pass: build green; zero old IDs on live product surface; Unshipped matches code.

#### Out of scope

- Renaming 017 folder name; implementing 015/016 code-fixes; unit-test project for diagnostics;
  severity/behavior changes; Unshipped→Shipped move; dual-ID compatibility layer.

#### Gotchas

- Config key is a literal string separate from `DiagnosticId` — both must change.
- Do not blind-sed `TW0003` in docs after rename (it correctly means MarkdownDocs load).
- Change source + Unshipped together.

## Session

- Created: 2026-07-15
- Clarified: rename **ALL** IDs; do not leave TWA001/TWA002
- Prefix locked: **TW** (recommend contiguous TW0001–TW0006)
- Plan: 2026-07-15 (orchestrate-task phase 2–3)
