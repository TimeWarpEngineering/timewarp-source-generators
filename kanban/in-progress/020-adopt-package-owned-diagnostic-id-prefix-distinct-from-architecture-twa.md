# Document TW diagnostic prefix SSOT (no rename)

## Description

Roslyn diagnostic IDs for **this package** (`TimeWarp.SourceGenerators`) already use **`TW0001`–`TW0006`**.
They do **not** collide with **TimeWarp.Architecture** analyzers (`TWA0001`–`TWA00xx`, plus `TWE*` / `SG*`
in that monorepo): different ID strings, different packages.

Historical confusion was **documentation**, not shipping:

- **`TWA001`** / **`TWA*`** — wrong when used for this package’s kebab-case rule (Architecture owns `TWA*`)
- **`TW0001`** — what this package **actually ships** for `FileNameRuleAnalyzer`

### Decision (locked)

**Keep `TW*`.** No diagnostic ID rename.

Rationale:

- No actual Roslyn ID collision with Architecture `TWA*`
- Rename would be a breaking change for suppressions / `dotnet_diagnostic.TW000*.*` editorconfig keys with no functional benefit
- Rejected alternatives: `TWG` (opaque), `TWS` (ambiguous), `TWSG` (clear but churn-only)

This task is **docs + SSOT cleanup only** in this repo, plus notes for external follow-ups.

### Current inventory (shipped — do not rename)

| Id | Owner type | Rule |
|----|------------|------|
| **TW0001** | `FileNameRuleAnalyzer` | kebab-case `.cs` file names |
| **TW0002** | `XmlDocsToMarkdownAnalyzer` | XML docs → markdown |
| **TW0003** | `MarkdownDocsGenerator` | markdown docs gen / kebab file matching |
| **TW0004** | `InterfaceDelegationGenerator` | class must be partial |
| **TW0005** | `InterfaceDelegationGenerator` | class must implement delegated interface |
| **TW0006** | `InterfaceDelegationGenerator` | multiple fields delegate the same interface |

Editorconfig custom key: `dotnet_diagnostic.TW0001.excluded_files`.

## Requirements

- Document SSOT: **`TW*` = TimeWarp.SourceGenerators**; **`TWA*` = TimeWarp Architecture only**
- Explicit callout: do **not** configure or refer to this package’s rules as `TWA001` / `TWA*`
- Grep this repo for stale `TWA001` / wrong prefix wording; fix readme, how-tos, reference docs, overview, spikes, kanban notes if needed
- Confirm code + `AnalyzerReleases.*` already use only `TW0001`–`TW0006` (no code change expected)
- External follow-ups (**out of this repo**, track elsewhere or as links only):
  - timewarp-ganda `documentation/developer/standards/file-naming.md` (TWA001 wording)
  - timewarp-architecture AGENTS / task 133 notes if they still say SourceGenerators `TWA001` or opaque `TWG`

## Checklist

### Decision
- [x] Choose prefix: **keep `TW`** (no rename to `TWSG` / `TWG` / `TWS`)
- [x] Record decision in Notes (this file)

### Docs cleanup (this repo)
- [ ] Readme: SSOT that **`TW*` = TimeWarp.SourceGenerators**, not Architecture `TWA*`
- [ ] Explicit “do not use TWA001 for this package” callout where consumers would look (readme and/or file-name analyzer docs)
- [ ] Grep for `TWA001` / stale wrong-prefix wording; fix how-tos, reference docs, overview, spikes
- [ ] Confirm no `TWA*` diagnostic ids on the shipped surface (code + AnalyzerReleases)

### Verify
- [ ] Docs are consistent: all references to this package’s rules use `TW0001`–`TW0006`
- [ ] No code/id rename performed

### Out of scope (do not do here)
- [ ] ~~Rename diagnostic ids~~
- [ ] ~~Package version bump for breaking id change~~
- [ ] Fix ganda / architecture repos (external only)

## Notes

### Decision log

| Date | Choice | Why |
|------|--------|-----|
| 2026-07-29 | **Keep `TW*`**; docs-only | No real conflict with Architecture `TWA*`; rename is churn without benefit. Confusion was external/historical `TWA001` wording for the kebab rule. |

### Collision context

| Prefix | Owner | Meaning |
|--------|-------|---------|
| **TW** | TimeWarp.SourceGenerators (this package) | Package diagnostics `TW0001`–`TW0006` |
| **TWA** | TimeWarp Architecture | Convention analyzers (`TWA0001` …) |
| **TWE** / **SG** | Architecture monorepo generators | Different package; not this repo |

Roslyn treats `TW0001` and `TWA0001` as distinct IDs. Suppressions and editorconfig keys do not cross-apply.

### Origin of the ask

timewarp-architecture task **133** (kebab gaps / enforcement) and audit research flagged wiring
`FileNameRuleAnalyzer` and warned about Architecture `TWA*` collision. Suggested rename target
**`TWG`** was a poor acronym and is **rejected**. Prefix-option table is closed: keep **`TW`**.

### Related

- `documentation/developer/how-to-guides/configure-file-name-analyzer.md`
- `documentation/developer/reference/analyzers/file-name-rule-analyzer.md`
- timewarp-flow ADR-0013 (kebab adoption; mentions source-generator enforcement)

## Implementation Notes

### Implementation plan (docs-only; 2026-07-29)

**Scope:** Docs + SSOT cleanup only. Keep `TW0001`–`TW0006`. No rename, no package version bump, no code changes.

#### Current-state findings
- Shipped surface already correct: source analyzers/generators + AnalyzerReleases.Unshipped.md use only TW0001–TW0006. No TWA* in live code.
- readme.md has partial SSOT one-liner; missing explicit “do not use TWA001” callout.
- File-name how-to/reference, overview, analyzers overview: IDs correct (TW0001) but no SSOT/anti-TWA001 callout.
- Spikes: nothing to fix.
- kanban/done/* historical TWA001/TW0003 wording: leave as archive (do not rewrite).

#### Files to edit
1. **readme.md** — Expand partial SSOT into Diagnostic ID prefixes subsection: table TW* = this package vs TWA* = Architecture only; explicit do-not-use TWA001 for this package; FileNameRule = TW0001.
2. **documentation/developer/reference/analyzers/file-name-rule-analyzer.md** — Callout: rule is TW0001, not TWA*.
3. **documentation/developer/how-to-guides/configure-file-name-analyzer.md** — Reinforce TW0001 editorconfig keys; troubleshooting if someone configured TWA001.
4. **documentation/overview.md** — Light SSOT sentence.
5. **documentation/developer/reference/analyzers/overview.md** — Short prefix note (recommended).
6. **This task file** — Checklist + Implementation Notes as work lands.

#### Do not touch
- source/** diagnostic IDs, AnalyzerReleases (confirm only)
- Package version
- kanban/done/** historical archives
- External repos (ganda file-naming.md, architecture AGENTS/task 133) — note only

#### External follow-ups (out of repo)
1. timewarp-ganda documentation/developer/standards/file-naming.md may still say TWA001 → should be TW0001
2. timewarp-architecture AGENTS / task 133 if they still claim SourceGenerators TWA001 or TWG rename

#### Verify greps (pass criteria)
- Zero TWA*/TWG/TW100x diagnostic ids in source/ and AnalyzerReleases
- Config examples use dotnet_diagnostic.TW0001 not TWA001
- Readme + file-name how-to/reference state TW* vs TWA* SSOT and do-not-use TWA001
- No code/id rename; no version bump

#### Sequence
1. Confirm shipped surface via grep
2. Strengthen readme SSOT
3. Add callouts to file-name reference, configure how-to, overview, analyzers overview
4. Repo-wide grep cleanup pass
5. Update task checklist; external items remain out of scope

#### Definition of done
Consumer docs clearly own TW* for this package and forbid treating rules as TWA001/TWA*; shipped surface confirmed TW0001–TW0006 only; task checklist complete for in-repo items.

## Session
- Orchestrator: grok (2026-07-29)
- Plan: plan agent 019fac34-2cd2-7d13-bc11-5dda239b039e (2026-07-29)
