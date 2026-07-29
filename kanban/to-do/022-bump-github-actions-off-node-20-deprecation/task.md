# Bump GitHub Actions off Node 20 deprecation

## Description

CI logs warn:

> Node.js 20 is deprecated. The following actions target Node.js 20 but are being
> forced to run on Node.js 24: `actions/checkout@v4`, `actions/setup-dotnet@v4`,
> `actions/upload-artifact@v4`.

See: https://github.blog/changelog/2025-09-19-deprecation-of-node-20-on-github-actions-runners/

Bump official actions in `.github/workflows/workflow.yml` to majors that declare
`using: node24`.

## Requirements

- No Node 20 deprecation from first-party `actions/*` steps in this workflow.
- `nuget/login@v1` already on Node 24 — leave unless it regresses.
- Pin majors consistent with TimeWarp peers where practical (ganda: checkout@v6, setup-dotnet@v5).

## Checklist

- [x] Bump `actions/checkout` to Node 24 major (`@v6`)
- [x] Bump `actions/setup-dotnet` to Node 24 major (`@v5`)
- [x] Bump `actions/upload-artifact` to Node 24 major (`@v6` — `@v5` still node20)
- [x] Commit; open PR / CI green

## Notes

### Target pins (verified via action.yml `using:`)

| Action | Was | Node | New | Node |
|--------|-----|------|-----|------|
| checkout | v4 | 20 | v6 | 24 |
| setup-dotnet | v4 | 20 | v5 | 24 |
| upload-artifact | v4 | 20 | v6 | 24 |

`upload-artifact@v5` still uses node20 — skip to **v6**.

## Session

- Created: 2026-07-29 — CI deprecation notice after beta.9 release
