# Add trusted-publishing probe mode to workflow.yml

## Description

org 458-009 probe (NuGet has no policy-enumeration API; probe = dispatch mode that runs only the nuget/login OIDC exchange and stops — success proves the workflow.yml policy matches; reference timewarp-nuru's workflow.yml).

Replayed onto `origin/master` after local 021 collided with the already-shipped multi-dot TW0001 task.

## Checklist

- [ ] probe input added
- [ ] login step condition extended
- [ ] probe-result step added
- [ ] pipeline step skipped in probe mode
- [ ] YAML valid
- [ ] Node 24 Action majors from origin/master kept (checkout@v6, setup-dotnet@v5, upload-artifact@v6)

## Notes

Originally implemented 2026-08-08 on a stale master worktree as task 021. Renumbered to 023 because origin already used 021 for TW0001 multi-dot kebab.

## Session

- Replay: grok (2026-08-15)
