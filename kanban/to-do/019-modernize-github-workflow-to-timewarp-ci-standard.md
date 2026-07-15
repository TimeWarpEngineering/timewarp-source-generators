# Modernize GitHub workflow to TimeWarp CI standard

## Description

Bring `.github/workflows/workflow.yml` in line with peer TimeWarp repos (nuru, builder,
options-validation, terminal): thin YAML, OIDC NuGet Trusted Publishing, and
`tools/dev-cli` as the CI entry point.

The file was renamed from `ci-cd.yml` → `workflow.yml` and the SDK bumped to `10.0.x` for
the standards audit. The pipeline body is still the old hand-rolled PowerShell flow and is
**not** up to current CI standards.

## Requirements

### Workflow YAML (thin pipeline)

Match the peer pattern:

1. Checkout with `fetch-depth: 0`
2. Setup .NET `10.0.x`
3. `nuget/login@v1` (OIDC) on release — `permissions: contents: read` + `id-token: write`
4. Run CI via dev-cli, e.g.  
   `dotnet run --file tools/dev-cli/dev.cs -- workflow`  
   (release: pass `--api-key` from nuget login when supported)
5. Upload `artifacts/packages/*.nupkg` (`if-no-files-found: ignore`, prefer `if: always()`)

### Path filters

Ensure changes that affect CI/build retrigger the workflow:

- Keep: `source/**`, `tests/**`, `.github/workflows/**`, `Directory.Build.props`
- Add: `tools/**`, `Directory.Packages.props`, `msbuild/**` (or equivalent)
- Drop or justify loose `*.props` / `*.targets` if superseded by explicit paths

### NuGet cache

Either:

- Cache `.nuget-cache/` (matches `RestorePackagesPath` in `Directory.Build.props`), or
- Stop setting a custom `RestorePackagesPath` and keep caching `~/.nuget/packages`

Current cache of `~/.nuget/packages` is ineffective while restore uses `.nuget-cache/`.

### Dev CLI (`workflow` command)

Today `tools/dev-cli/endpoints/workflow-command.cs` only runs  
`clean → build → test` via `./bin/dev` and has **no** pack/push/`--api-key` path.

Either:

- **A (preferred):** Extend `dev workflow` so it can own PR + release (mode/event detection,
  pack, version check, NuGet push with optional API key) like peer repos, **or**
- **B:** Keep release pack/push as explicit YAML steps until A is done; still invoke
  `dev` for build/test so the thin-YAML shape is partial but usable

Document which option was chosen in Results.

### Tests

- Remove the dead TODO path `tests/timewarp-source-generators-tests/`
- Wire real verification (test console and/or `dev test` / `dev verify-samples`) so CI
  fails on regressions — or document intentional deferral with a follow-up task

### Auth

- Prefer OIDC Trusted Publishing (`nuget/login@v1`, user `TimeWarp.Enterprises`)
- Retire `secrets.PUBLISH_TO_NUGET_ORG` unless still required as fallback

## Checklist

- [ ] Rewrite `.github/workflows/workflow.yml` to thin peer-style pipeline
- [ ] Add OIDC permissions + `nuget/login@v1` for release
- [ ] Fix path filters (`tools/**`, `Directory.Packages.props`, etc.)
- [ ] Fix NuGet cache path vs `RestorePackagesPath`
- [ ] Extend or document `dev workflow` for pack/push (option A or B)
- [ ] Enable real test/verify step; remove dead test path
- [ ] Drop legacy `PUBLISH_TO_NUGET_ORG` push when OIDC works
- [ ] Validate PR path: build (+ tests) without publish
- [ ] Validate release path: version check + pack + push (or dry-run notes)
- [ ] Commit

## Notes

### Current workflow issues (review 2026-07-15)

| Issue | Detail |
|-------|--------|
| Legacy body | Inline pwsh: build project, optional version search, `dotnet nuget push` with secret |
| Auth | `secrets.PUBLISH_TO_NUGET_ORG` — peers use OIDC |
| Path filters | Missing `tools/**`, `Directory.Packages.props` |
| Cache | Caches `~/.nuget/packages`; restore uses `.nuget-cache/` |
| Tests | Commented; wrong folder name |
| `dev workflow` | Needs prebuilt `./bin/dev`; no pack/push/`--api-key` |

### Reference workflows

- `timewarp-nuru` / `timewarp-builder` / `timewarp-options-validation` / `timewarp-terminal`
  → `.github/workflows/workflow.yml`

### What’s already good

- Filename `workflow.yml` (audit)
- SDK `10.0.x`
- Triggers: push/PR → master, release published, workflow_dispatch
- Version still lives in `source/Directory.Build.props` (`GeneratePackageOnBuild` →
  `artifacts/packages/`)

## Session

- Created: 2026-07-15 (post `ganda repo audit --fix` + workflow review)
