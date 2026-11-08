# Review framework

## Budget (by-diff)

- Lines changed: 102
- Effort: 1
- TCB hits: none
- Roster axes: general
- Turn cap: 80 (--max-turns; cursor uncapped)

## Round 1 framing

**Date:** 2026-11-09
**Host task:** kanban/in-progress/031-remove-the-unused-interface-delegation-generator/
**Diff scope:** branch `task/031-remove-the-unused-interface-delegation-generator` vs merge-base `0b46155` (master); product commit `c60451d`
**Plan / brief:** remove `InterfaceDelegationGenerator`, the handwritten `ImplementsAttribute`, TW0004–TW0006 release rows, the delegation-only test-console sample, the readme section; fix the documented id range; archive task 026
**Effort:** 2 — roster axes: general
**Reviewer roster:** general
**Session IDs:** review oracle reviewer-claude (claude-opus-5-5, headless `ganda task work`)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`

## Round 2 framing

**Date:** 2026-11-09
**Diff scope:** delta `ed94ce5..3d0563a` (fix pass for M1; product change `source/Directory.Build.props`)
**Effort:** 1 — roster axes: general
**Reviewer roster:** general
**Session IDs:** review oracle reviewer-claude (claude-opus-5-5, headless `ganda task work`), round 2
