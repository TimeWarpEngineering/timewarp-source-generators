# Add trusted-publishing probe mode to workflow.yml

## Description

org 458-009 probe (NuGet has no policy-enumeration API; probe = dispatch mode that runs only the nuget/login OIDC exchange and stops — success proves the workflow.yml policy matches; reference timewarp-nuru's workflow.yml).

Replayed onto `origin/master` after local 021 collided with the already-shipped multi-dot TW0001 task.

## Checklist

- [x] probe input added
- [x] login step condition extended
- [x] probe-result step added
- [x] pipeline step skipped in probe mode
- [x] YAML valid
- [x] Node 24 Action majors from origin/master kept (checkout@v6, setup-dotnet@v5, upload-artifact@v6)

## Results

- Added `workflow_dispatch.inputs.mode` (choice: merge/probe, default merge) to `.github/workflows/workflow.yml`.
- Extended the `ci` job's "NuGet login (OIDC Trusted Publishing)" step condition to also run on `workflow_dispatch` with `mode == 'probe'`.
- Added a "Trusted publishing probe result" step that echoes success when probe mode's OIDC login completes.
- Gated "Run CI Pipeline" to skip when `workflow_dispatch` + `mode == 'probe'`, so probe mode never builds or publishes.
- Kept origin's Node 24 pins (`checkout@v6`, `setup-dotnet@v5`, `upload-artifact@v6`).

### How to validate

**Smoke:** `gh workflow run workflow.yml -f mode=probe` after push → expect the "Trusted publishing probe result" step to run and go green.
**Expect:** a failure of the NuGet login step means the trusted-publishing policy is missing or misconfigured on NuGet.org for this repo + workflow.yml — not a bug in this change.

## Notes

Originally implemented 2026-08-08 on a stale master worktree as task 021. Renumbered to 023 because origin already used 021 for TW0001 multi-dot kebab.

## Session

- Replay: grok (2026-08-15)
