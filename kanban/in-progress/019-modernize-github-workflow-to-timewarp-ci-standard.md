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

### Implementation plan (2026-07-15)

**Chosen approach: Option A** — full `dev workflow` ownership (peer consensus).  
**Peer template:** timewarp-options-validation (single package + GeneratePackageOnBuild).

#### Target

```
YAML (thin): checkout → setup-dotnet 10 → [release] nuget/login OIDC →
  dotnet run --file tools/dev-cli/dev.cs -- workflow [--api-key]
  → upload artifacts/packages/*.nupkg (always)

PR/merge: clean → build → test console
Release:  tag/version + NuGet not-published → clean → build → test → push
```

#### Key decisions

| Topic | Decision |
|-------|----------|
| Option A vs B | **A** — extend workflow-command (in-process; no `./bin/dev`) |
| Tests | Rewrite `dev test` to build+run test console; remove dead path with YAML rewrite |
| Test console ref | **T1:** Analyzer `ProjectReference` (fix PackageReference/`Version=$(Version)` chicken-and-egg) |
| Cache | Remove cache step; keep `RestorePackagesPath=.nuget-cache/` (no peer cache) |
| Auth | OIDC only on `release`; retire `PUBLISH_TO_NUGET_ORG` |
| Path filters | Add `tools/**`, `Directory.Packages.props`, `msbuild/**`, `source/Directory.Build.props`, `nuget.config`; drop `*.props`/`*.targets` |
| Pack | No separate pack step — `GeneratePackageOnBuild` on Release build |

#### Ordered steps

1. Fix test console → Analyzer ProjectReference (+ drop self PackageVersion if unused)
2. Rewrite `test-command.cs` → build+run console
3. Rewrite `workflow-command.cs` → Option A (modes, tag/version, NuGet check, push)
4. Optional: pin `build-command` to source project
5. Rewrite `workflow.yml` thin peer pipeline
6. Local verify: `dotnet run --file tools/dev-cli/dev.cs -- workflow`
7. Commit

#### Files

- `.github/workflows/workflow.yml`
- `tools/dev-cli/endpoints/workflow-command.cs`
- `tools/dev-cli/endpoints/test-command.cs`
- `tests/.../timewarp-source-generators-test-console.csproj`
- `Directory.Packages.props` (self PackageVersion cleanup)
- Optionally `build-command.cs`

#### Risks

- OIDC Trusted Publishing must exist for package on nuget.org (ops)
- Do not shell `./bin/dev` from workflow (CI uses file-based run)
- `dotnet test` alone is false-green today — must use console

## Session

- Created: 2026-07-15 (post `ganda repo audit --fix` + workflow review)
- Plan: 2026-07-15 (orchestrate-task phase 2–3)
