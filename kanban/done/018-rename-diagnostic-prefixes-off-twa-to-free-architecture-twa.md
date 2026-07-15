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

- [x] Apply TW0001–TW0006 mapping (or document deviation)
- [x] Rename **all** IDs in source (no TWA / TWG / TW100x leftovers)
- [x] AnalyzerReleases.Shipped.md / Unshipped.md
- [x] readme.md, documentation/, .editorconfig samples
- [x] Tests assert new IDs (test-console `.editorconfig` uses TW0001/TW0002; no unit-test project asserts IDs)
- [x] Grep: no TWA001, TWA002, TW1001–1003, TWG001 in code
- [x] Commit
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

## Results

### Summary

Renamed every live diagnostic ID in `TimeWarp.SourceGenerators` to contiguous **TW0001–TW0006**. Dropped TWA/TWG/TW1xxx so TimeWarp Architecture can own **TWA**. Single **TW** family for this package’s analyzers and generators.

| Old | New | Component |
|-----|-----|-----------|
| TWA001 | TW0001 | FileNameRuleAnalyzer (+ `excluded_files` key) |
| TWA002 | TW0002 | XmlDocsToMarkdownAnalyzer |
| TWG001 | TW0003 | MarkdownDocsGenerator |
| TW1001 | TW0004 | Interface delegation — partial |
| TW1002 | TW0005 | Interface delegation — not implement |
| TW1003 | TW0006 | Interface delegation — multiple fields |

### Files changed

**Source:** `file-name-rule-analyzer.cs`, `xml-docs-to-markdown-analyzer.cs`, `markdown-docs-generator.cs`, `interface-delegation-generator.cs`, `AnalyzerReleases.Unshipped.md`  
**Config:** root `.editorconfig`, test-console `.editorconfig`  
**Docs:** `readme.md`, `documentation/**` (overview, how-to, analyzer/SG refs, release tracking)  
**Open kanban:** 015, 016, 017 (fix + interface draft)

### Key decisions

- Left `AnalyzerReleases.Shipped.md` empty (always-unshipped policy).
- No dual-ID compatibility layer (intentional full rename).
- File-name docs corrected from historical TW0003 scheme to **TW0001**.
- `kanban/done/*` history left as archaeology; open tasks scrubbed.
- Phantom TW1004 removed from interface-delegation draft task.

### Verification

- `dotnet build source/timewarp-source-generators/... -c Release` — **succeeded** (0 warnings/errors); package 1.0.0-beta.8 produced.
- Grep old IDs on `source/`, `documentation/`, `readme.md`, `.editorconfig`, `tests/` — **zero matches**.
- All six new IDs present in source + Unshipped.
- Review (`72e360e`): 0 bugs; 1 suggestion (017 prose) fixed in `42f1108`.

### Note

Solution-wide restore of the test console can fail with NU1102 when CPM `Version=$(Version)` does not resolve for the test project (pre-existing; not introduced by this rename). Library project builds clean.

## Session

- Created: 2026-07-15
- Clarified: rename **ALL** IDs; do not leave TWA001/TWA002
- Prefix locked: **TW** (recommend contiguous TW0001–TW0006)
- Plan: 2026-07-15 (orchestrate-task phase 2–3)
- Implemented + reviewed: 2026-07-15 (orchestrate-task phases 4–5)
