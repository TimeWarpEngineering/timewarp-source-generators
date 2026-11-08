# Review framework

## Budget (by-diff)

- Lines changed: 164
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Framework — task 032

**Date:** 2026-11-09
**Host task:** kanban/in-progress/032-remove-xml-docs-to-markdown-analyzer-and-generator/
**Diff scope:** branch `task/032-remove-xml-docs-to-markdown-analyzer-and-generator` vs merge-base `5e02ad4` (product commit `b1d71df`, kitchen commit `8211895`)
**Plan / brief:** task.md Requirements: retire TW0002 analyzer and TW0003 markdown-docs generator, their docs, fixtures and package tags; keep TW0001/TW0007
**Effort:** 3 (by-diff budget); roster axes: general
**Reviewer roster:** general
**Session IDs:** reviewer-claude `b2d23b6c-a334-41ff-b78e-8d293fc9912e`

### Round 2

- **Diff scope:** delta `8211895..3185140` (fix pass, docs only)
- **Effort:** 1 (by-diff budget above); roster: general
- **Session IDs:** reviewer-claude `1c7e1c84-7b5d-467c-8c76-e7672f636eb0`

### Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
