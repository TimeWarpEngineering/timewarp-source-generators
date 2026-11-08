# Review framework

## Budget (by-diff)

- Lines changed: 1755
- Effort: 3
- TCB hits: none
- Roster axes: general
- Turn cap: 200 (--max-turns; cursor uncapped)

## Framework — task 032

**Date:** 2026-11-09
**Host task:** kanban/in-progress/032-remove-xml-docs-to-markdown-analyzer-and-generator/
**Diff scope:** branch `task/032-remove-xml-docs-to-markdown-analyzer-and-generator` vs merge-base `5e02ad4` (product commit `b1d71df`, kitchen commit `8211895`)
**Plan / brief:** task.md Requirements: retire TW0002 analyzer and TW0003 markdown-docs generator, their docs, fixtures and package tags; keep TW0001/TW0007
**Effort:** 3 (by-diff budget); roster axes: general
**Reviewer roster:** general
**Session IDs:** reviewer-claude `b2d23b6c-a334-41ff-b78e-8d293fc9912e`

### Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
