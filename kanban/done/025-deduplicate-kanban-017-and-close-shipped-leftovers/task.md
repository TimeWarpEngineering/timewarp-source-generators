# Deduplicate Kanban 017 And Close Shipped Leftovers

## Description

Board hygiene only. Do not claim or launch the existing **017** cards.

`ganda kanban show 017` / `path 017` resolved to **to-do** `017-create-interface-delegation-generator` because two kitchens shared id 017. `FindTask` returns `FirstOrDefault` in column order (to-do before in-progress), so the in-progress filename-skip card was invisible to the CLI.

Shipped work was left in the wrong column:

- **in-progress 017** — TW0001 skip of `obj/` / `bin/` (folder still said TW0003). Landed in PR [#35](https://github.com/TimeWarpEngineering/timewarp-source-generators/pull/35) as `1.0.0-beta.10`.
- **to-do 022** — GitHub Actions Node 24 majors. Same PR [#35](https://github.com/TimeWarpEngineering/timewarp-source-generators/pull/35).

Same collision pattern as the 021→023 / 022→024 renumbers after a stale-master replay.

## Requirements

- Do **not** `ganda kanban done 017` / `claim 017` / `move 017` (ambiguous id).
- Filename-skip 017 keeps id **017**, moves to done, folder name uses TW0001.
- Interface-delegation 017 is re-id'd to reserved **026** and stays in to-do (generator exists; card never got Results).
- 022 moves to done with Results pointing at PR #35.
- After publish, `ganda kanban show 017` is the filename-skip (done) card only.
- Kanban/** only. No product code.

## Checklist

- [x] Reserve 025 (this card) and 026 (interface-delegation re-id); claim **025** only
- [x] Close filename-skip 017 → done (Results + TW0001 folder name)
- [x] Re-id interface-delegation 017 → **026** (stay to-do; note former id)
- [x] Close 022 → done (Results)
- [x] Confirm `ganda kanban path/show 017` is unique
- [x] `ganda kanban done 025` after publish (column move lands via PR; cannot publish from done)

## Notes

- Existing 017s were unclaimed; this task is the only claim.
- Path skip already lives in `file-name-rule-analyzer.cs` (`IsBuildOutputOrGeneratedPath`). Not re-implemented here.
- 026 stays a reserved stub on `refs/ganda/claims` until someone picks up the published kitchen (`claim 026` then converts the stub; title is ignored when the kitchen exists).
- Sibling moves published from to-do; this kitchen then moved to **done** (publish cannot run from done).

## Session

- Created: 1277286 (2026-08-26)
- Cockpit: Grok `01a03b95-a63c-7423-b3e5-ccb1746483e7` (2026-08-26)
- Done-column pickup: 1285505 (2026-08-26)

## Results

Board-only close-out on `task/025-deduplicate-kanban-017-and-close-shipped-leftovers`:

| Old path | New path |
|----------|----------|
| `in-progress/017-fix-tw0003-analyzer-ignoring-generated-files/` | `done/017-fix-tw0001-analyzer-ignoring-generated-files/` |
| `to-do/017-create-interface-delegation-generator/` | `to-do/026-create-interface-delegation-generator/` |
| `to-do/022-bump-github-actions-off-node-20-deprecation/` | `done/022-bump-github-actions-off-node-20-deprecation/` |

### How to validate

```bash
ganda kanban board timewarp-source-generators --show-done
# Expect: in-progress empty; to-do includes 026 (not 017 or 025); done includes 017, 022, and 025

ganda kanban path 017 --repo timewarp-source-generators
# Expect: .../kanban/done/017-fix-tw0001-analyzer-ignoring-generated-files

ganda kanban path 026 --repo timewarp-source-generators
# Expect: .../kanban/to-do/026-create-interface-delegation-generator

ganda kanban who --repo timewarp-source-generators
# After publish 025: no holder for 025; 026 may still show reserved until first pickup
```
