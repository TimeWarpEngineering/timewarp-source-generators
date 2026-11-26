# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators.
Filter: (all)
Outdated:
  TimeWarp.Nuru 4.0.0-beta.16 -> 4.0.0-beta.17 (dev-only: tools/dev-cli/Directory.Build.props)
  TimeWarp.Nuru.DevCli 4.0.0-beta.16 -> 4.0.0-beta.17 (dev-only: tools/dev-cli/Directory.Build.props)
Every outdated pin is dev-only. Bump them. Do not change <Version> in source/Directory.Build.props: the wave merges this task and does not release this repo.
Edit Directory.Packages.props in this repo only.

## Goal (implementer restatement)

Bump the two dev-only Nuru pins in `Directory.Packages.props` from 4.0.0-beta.16 to 4.0.0-beta.17
(TimeWarp.Nuru and TimeWarp.Nuru.DevCli, consumed only by tools/dev-cli).

Acceptance criteria:
- `Directory.Packages.props` pins both packages at 4.0.0-beta.17; nothing else in the repo pins beta.16.
- `<Version>` in source/Directory.Build.props is unchanged (no release).
- The dev CLI still compiles and `./bin/dev build` passes; `ganda repo audit` passes.

## Session

- Minted by ganda task wave run 20261126T021837Z-0cb631
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session d919282a-a5e4-44aa-9b2b-2d1317b55902, max-turns 500, Done — 2026-11-26T02:32:15Z
- Review oracle: grok session 01a215b2-67be-77a0-abce-d526f70215a8 (2026-11-26), effort 1, roster general, turn cap 80
- Review oracle: review by reviewer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a215b2-67be-77a0-abce-d526f70215a8, max-turns 80, artifacts general.md, merged.md, disposition.md — 2026-11-26T02:35:48Z

## Results

Bumped `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` from 4.0.0-beta.16 to 4.0.0-beta.17 in
`Directory.Packages.props` (dev-only, used by tools/dev-cli). `<Version>` in
source/Directory.Build.props is unchanged. Re-ran `dev self-install` so `./bin/dev` uses the new packages.

Gates:
- `./bin/dev build` — pass (0 warnings, 0 errors)
- Tests: no test file covers tools/dev-cli; the change edits only a package pin, so no test runfile was run (CI runs the full suite).
- `ganda repo audit` — pass; 1 advisory warning `vscode-window-icon` (.vscode/settings.json missing), non-blocking and pre-existing, left as is.

### How to validate

Smoke: `dotnet run --file tools/dev-cli/dev.cs -- self-install && ./bin/dev --capabilities && ./bin/dev build`
Expect: the dev CLI publishes against Nuru 4.0.0-beta.17, `--capabilities` exits 0, and the build reports 0 errors.

### Review

- Round 1, effort 1, reviewer roster: general (grok session 01a215b2-67be-77a0-abce-d526f70215a8, 2026-11-26).
- Counts: bug 0 open / 0 fixed / 0 wontfix; suggestion 0; nit 0. Final open count 0.
- Disposition: clean. The implementer restatement matches the original task. No wontfix and no escalation.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

## Review

- Round 1: Done
