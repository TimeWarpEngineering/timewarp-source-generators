# Review framework — task 020

**Date:** 2026-07-29
**Host task:** `kanban/in-progress/020-adopt-package-owned-diagnostic-id-prefix-distinct-from-architecture-twa/`
**Diff scope:** Branch `Cramer/2026-07-29/dev` docs commit `acfa159` vs prior plan commit — files:
- `readme.md`
- `documentation/overview.md`
- `documentation/developer/how-to-guides/configure-file-name-analyzer.md`
- `documentation/developer/reference/analyzers/file-name-rule-analyzer.md`
- `documentation/developer/reference/analyzers/overview.md`
- Task bookkeeping under this folder (`task.md`) — lower priority

**Plan / brief:** Docs-only SSOT: document that this package owns **`TW0001`–`TW0006`**; Architecture owns **`TWA*`**; do not configure FileNameRule as `TWA001`. No diagnostic ID rename, no code change, no package version bump.

**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** orchestrator grok; implementer subagent 019fac36-0b8c-7330-a335-289def7d172f; review round-1 general TBD

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
- Scope is documentation correctness vs the locked plan (keep TW*, anti-TWA001 callouts, no code touch)
