# Bring ganda repo audit clean and update NuGet packages

## Goal (implementer restatement)

`ganda repo audit` fails, and central package versions are behind nuget.org. One task brings the audit to exit 0 and moves every outdated `PackageVersion` to the version `ganda nuget outdated` calls latest. The product and the dev CLI still build and test.

Acceptance:

- Re-read `ganda repo audit` and `ganda nuget outdated` at the start. Treat those live results as the list. Pins move.
- Apply `ganda repo audit --fix` for every fixable row and commit that diff. Do not revert a package bump it makes.
- Then update every package `ganda nuget outdated` still reports, including packages the audit does not name. Use `ganda nuget outdated --update --force`. The version in that command's New column wins.
- `./bin/dev build` and `./bin/dev test` succeed after the pins move. `dev --help` still shows `release`, and `dev check-version` still sees `TimeWarp.SourceGenerators`.
- `ganda repo audit` exits 0 with no Failed row.
- Record `## Results` and a non-empty `### How to validate` (Smoke + Expect). Stop before `kanban done` and `gh pr create`.

## Description

`ganda repo audit` on `timewarp-source-generators` master fails.
Snapshot from 2026-11-09 (`origin/master`): **Passed 35, Failed 4, Skipped 3**, exit 1.
Three rows are Error. One is Warning (`vscode-window-icon`). The warning counts as Failed.

The same day, `ganda nuget outdated --update --dry-run` would move **7** packages (3 major, 4 prerelease). Three packages were already current. Update all seven. Re-check the three that were current in case a newer version appears before the work starts.

Predecessor: task **030** brought the audit clean on Nuru / DevCli **4.0.0-beta.11**. This task is the next drift, and it also takes the Roslyn 5 and Amuru bumps the `nuru` check does not name.

## Requirements

- `ganda repo audit` exits 0. The results table has no Failed row.
- Every outdated package in `Directory.Packages.props` matches the latest `ganda nuget outdated` reports when the work starts.
- `./bin/dev build` succeeds (0 errors) and `./bin/dev test` passes.
- Do not add a root `PackageReference` to `TimeWarp.SourceGenerators`. This repo is that package. Leave `global-usings-analyzer.severity = off` as task 030 set it.
- Do not implement on `master`. Work stays in the claimed task worktree.
- Do not commit `.vscode/settings.json` or anything under `.timewarp/signals/`.

### Audit snapshot (2026-11-09)

All four Failed rows reported Fixable YES.

- `analyzer-suppressions` (Error) — `.timewarp/suppression-baseline.json` is missing. `--fix` seeds it from the current tree. Commit that file.
- `nuru` (Error) — `TimeWarp.Nuru` is **4.0.0-beta.11**, latest **4.0.0-beta.12**. Bump it with the rest of the NuGet set below. `TimeWarp.Nuru.DevCli` is the same pair and must move together even if the audit row names only `TimeWarp.Nuru`.
- `timewarp-signals-gitignore` (Error) — `.timewarp/signals/` is not ignored, so walk signal logs dirty origin-home. Add `/.timewarp/signals/`. Do not commit the journals. `git ls-files '.timewarp/signals/*'` must be empty.
- `vscode-window-icon` (Warning) — `.vscode/settings.json` is missing. `--fix` writes the gitignored window title / peacock color. Keep the working copy. Do not commit `settings.json`.

### NuGet snapshot (2026-11-09)

`ganda nuget outdated` against nuget.org. `--update` would apply the New column.

| Package | Current | New |
|---------|---------|-----|
| `Microsoft.CodeAnalysis.Analyzers` | 3.11.0 | 5.9.0 (stable, major) |
| `Microsoft.CodeAnalysis.CSharp` | 4.11.0 | 5.9.0 (stable, major) |
| `Microsoft.CodeAnalysis.CSharp.Workspaces` | 4.11.0 | 5.9.0 (stable, major) |
| `TimeWarp.Nuru` | 4.0.0-beta.11 | 4.0.0-beta.12 |
| `TimeWarp.Nuru.DevCli` | 4.0.0-beta.11 | 4.0.0-beta.12 |
| `TimeWarp.Amuru` | 3.0.0-beta.1 | 3.0.0-beta.2 |
| `TimeWarp.Amuru.Tools` | 3.0.0-beta.1 | 3.0.0-beta.2 |

Already current on that run: `Microsoft.CodeAnalysis.BannedApiAnalyzers` 5.6.0, `TimeWarp.Build.Tasks` 1.0.0, `TimeWarp.Terminal` 1.0.2. Bump any of these if the live run says they are outdated.

The generator project targets `netstandard2.0`. The Roslyn 4.11 → 5.9 move can break analyzer APIs. Fix the generator and its tests so they compile on the new packages. Do not leave those three packages on 3.11 / 4.11. If a bumped package no longer supports the current target framework, raise that target only as far as the package requires and record why in `## Results`.

Nuru 4.0.0-beta.12 or Amuru 3.0.0-beta.2 may change the dev CLI. Update `tools/dev-cli` so `./bin/dev build` succeeds. `dev --help` must still show `release`, and `dev check-version` must still see `TimeWarp.SourceGenerators`.

## Checklist

- [ ] Re-run `ganda repo audit` and `ganda nuget outdated` and refresh this list if a row changed
- [ ] `ganda repo audit --fix` for the fixable rows, and commit that diff
- [ ] `ganda nuget outdated --update --force` so every outdated package matches the live New column
- [ ] Generator, analyzer tests, and dev CLI compile on the new pins
- [ ] `/.timewarp/signals/` is ignored and no signal journal is tracked
- [ ] `.vscode/settings.json` exists locally and is not committed
- [ ] `.timewarp/suppression-baseline.json` is committed
- [ ] No new root `PackageReference` to `TimeWarp.SourceGenerators`
- [ ] `ganda repo audit` exits 0 with no Failed row
- [ ] `./bin/dev build` and `./bin/dev test` succeed
- [ ] `## Results` and `### How to validate`
- [ ] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

## Session

- Created: 1998795 (2026-11-09)
- Trigger: Steve asked to create one task that fixes the `ganda repo audit` failures and updates every outdated NuGet package

## Notes

- Policy: `ganda repo audit --fix` is the baseline for the four audit rows. Commit it. `ganda nuget outdated --update --force` is the baseline for package versions. If the two disagree on a version, the outdated command's New column wins, and the audit must still pass.
- Do not revert a major bump. The Roslyn 5.9.0 move is in scope.
