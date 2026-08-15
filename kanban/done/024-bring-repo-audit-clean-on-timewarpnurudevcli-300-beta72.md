# Bring repo audit-clean on TimeWarp.Nuru.DevCli 3.0.0-beta.72

## Description

Org wave (timewarp-nuru 458-010 remediation + DevCli adoption). Passing
`ganda repo audit` means adopting the current Nuru/DevCli toolkit: `dev release`,
promotion gates, attestation verifier, trusted-publishing probe, derived package sets.

Replayed onto `origin/master` after local 022 collided with the Node 24 Actions task.

## Checklist

- [x] Pin TimeWarp.Nuru and TimeWarp.Nuru.DevCli to current latest (3.0.0-beta.76)
- [x] Register `IPackableProjectService` / drop `GitTagCheckService`
- [x] Extra DevCli NoWarns for current DevCli
- [x] `[ganda.audit] kebab-path-names.prune` for local package cache
- [x] `ganda repo audit` passes
- [x] Smoke: `dev --help` shows `release`; `dev check-version` derives packable set
- [x] `dev build` — 0 warnings / 0 errors

## Notes

Originally implemented 2026-08-08 on a stale master worktree as task 022 targeting
beta.72. Renumbered to 024 because origin already used 022 for the Node 24 Actions bump.

On replay (2026-08-15) audit reported latest **3.0.0-beta.76**, so the pin is 76
rather than frozen 72. Kebab path renames from the first pass already landed on
origin via PR #34.

`dev check-version` correctly reports `1.0.0-beta.10` already published. This
task does not bump the product package.

## Results

Repo is audit-clean on TimeWarp.Nuru / DevCli **3.0.0-beta.76**.

- Hand-pinned Nuru + DevCli to `3.0.0-beta.76`
- DI: removed `GitTagCheckService`, added `IPackableProjectService`/`PackableProjectService`
- Kept `INuGetPackageService` — `workflow` still injects it
- Added `[ganda.audit] kebab-path-names.prune = .nuget-cache,generated,.generated,build,_ai,spikes`
- `dev self-install` produced `bin/dev` with `release`

### How to validate

```bash
grep -E 'TimeWarp\.Nuru' Directory.Packages.props
# Expect: both 3.0.0-beta.76

ganda repo audit
# Expect: Repository passes all audit checks.

./bin/dev --help | grep release
./bin/dev check-version
# Expect: Packages checked: TimeWarp.SourceGenerators
# (version == published beta.10 is expected until a product bump)

./bin/dev build
# Expect: 0 warnings / 0 errors
```

## Session

- First pass (stale master): grok (2026-08-08)
- Replay onto origin/master: grok (2026-08-15)
