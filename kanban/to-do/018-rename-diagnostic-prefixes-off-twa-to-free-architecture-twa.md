# Rename ALL diagnostic IDs off TWA / TW / TWG to one product prefix

## Description

Rename **every** diagnostic ID in `TimeWarp.SourceGenerators` to a **single** product prefix.
Nothing stays on **TWA**, **TW1xxx**, **TWG**, or stale **TW000x**.

Architecture owns **TWA** = TimeWarp Architecture (`TWA0001`–`TWA0010` and growing). This package
must stop using TWA entirely so the prefix means one product only.

This is a deliberate full rename, not an optional collision fix. Technical non-overlap of
`TWA001` vs `TWA0001` is irrelevant — we still rename **all** of them.

## Target prefix (recommended)

**TWS** = TimeWarp Source-generators (or document another single prefix if preferred).

One prefix for analyzers **and** generators that report diagnostics. No TWAA/TWAG role suffixes.

### Full mapping (draft — adjust numbers if you want contiguous ranges)

| Current (leave none) | Suggested | Feature |
|----------------------|-----------|---------|
| TWA001 | **TWS0001** | FileNameRuleAnalyzer (kebab-case) |
| TWA002 | **TWS0002** | XmlDocsToMarkdownAnalyzer |
| TWG001 | **TWS0003** | MarkdownDocsGenerator (if it still reports) |
| TW1001 | **TWS1001** | Interface delegation — class must be partial |
| TW1002 | **TWS1002** | Interface delegation — doesn’t implement interface |
| TW1003 | **TWS1003** | Interface delegation — multiple fields same interface |

(Or renumber 1001–1003 into TWS0004–0006 — pick one scheme and stick to it.)

Also scrub historical **TW0001–TW0004 / TW0003** mentions in kanban/docs so they don’t reintroduce confusion.

## Requirements

- **Zero** remaining `TWA*`, `TW1*`, `TWG*`, or `TW000*` diagnostic IDs in **live code**.
- Update: all DiagnosticId / descriptor IDs, AnalyzerReleases.*.md, readme, documentation,
  .editorconfig samples, tests, any help links.
- Document: **TWS** (or chosen prefix) = this package only; Architecture = **TWA**.
- Build green after rename.

## Checklist

- [ ] Lock prefix (TWS) and complete ID mapping table in Notes/Results
- [ ] Rename **all** IDs in source (no TWA leftovers)
- [ ] AnalyzerReleases.Shipped.md / Unshipped.md
- [ ] readme.md, documentation/, .editorconfig samples
- [ ] Tests assert new IDs
- [ ] Grep repo: no TWA001, TWA002, TW1001–1003, TWG001 in code
- [ ] Commit

## Notes

- Architecture already renamed TWPA → TWA (timewarp-architecture).
- Full rename of this package is intentional product hygiene, not “only if IDs collide.”
- Worktree: `/home/steve/worktrees/github.com/TimeWarpEngineering/timewarp-source-generators/Cramer-2026-06-30-dev`

## Session

- Created: 2026-07-15
- Clarified: rename **ALL** IDs; do not leave TWA001/TWA002
