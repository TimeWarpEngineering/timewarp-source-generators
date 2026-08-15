# Bring repo audit-clean on TimeWarp.Nuru.DevCli 3.0.0-beta.72

## Description

Org wave (timewarp-nuru 458-010 remediation + DevCli 3.0.0-beta.72 adoption —
they are the same wave: the audit's `nuru` check went red org-wide when
beta.72 shipped, by design). Passing `ganda repo audit` now means adopting the
full release toolkit: `dev release`, promotion gates, attestation verifier,
trusted-publishing probe, derived package sets.

Replayed onto `origin/master` after local 022 collided with the Node 24 Actions task.

## Checklist

- [ ] Pin TimeWarp.Nuru and TimeWarp.Nuru.DevCli at 3.0.0-beta.72
- [ ] Register `IPackableProjectService` / drop `GitTagCheckService`
- [ ] Extra DevCli NoWarns for beta.72
- [ ] `[ganda.audit] kebab-path-names.prune` for local package cache
- [ ] `ganda repo audit` passes
- [ ] Smoke: `dev --help` shows `release`; `dev check-version` derives packable set

## Notes

Originally implemented 2026-08-08 on a stale master worktree as task 022. Renumbered to 024 because origin already used 022 for the Node 24 Actions bump.

Kebab path renames from the first pass already landed on origin via PR #34.

## Session

- Replay: grok (2026-08-15)
