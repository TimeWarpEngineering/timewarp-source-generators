# Add trusted-publishing probe mode to workflow.yml

## Description

org 458-009 probe (NuGet has no policy-enumeration API; probe = dispatch mode that runs only the nuget/login OIDC exchange and stops — success proves the workflow.yml policy matches; reference timewarp-nuru's workflow.yml).

## Checklist

- [x] probe input added
- [x] login step condition extended
- [x] probe-result step added
- [x] pipeline step skipped in probe mode
- [x] YAML valid

## Results

- Added `workflow_dispatch.inputs.mode` (choice: merge/probe, default merge) to `.github/workflows/workflow.yml`.
- Extended the `ci` job's "NuGet login (OIDC Trusted Publishing)" step condition to also run on `workflow_dispatch` with `mode == 'probe'`.
- Added a new "Trusted publishing probe result" step that echoes success when probe mode's OIDC login completes.
- Gated the existing "Run CI Pipeline" step to skip when `workflow_dispatch` + `mode == 'probe'`, so probe mode never builds or publishes.
- No changes needed to the "Upload Artifacts" step; it already tolerates no files via `if-no-files-found: ignore`.

### How to validate

**Smoke:** `gh workflow run workflow.yml -f mode=probe` after push → expect the "Trusted publishing probe result" step to run and go green.
**Expect:** a failure of the NuGet login step means the trusted-publishing policy is missing or misconfigured on NuGet.org for this repo + workflow.yml — not a bug in this change.
