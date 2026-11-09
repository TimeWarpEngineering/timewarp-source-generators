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

- [x] Re-run `ganda repo audit` and `ganda nuget outdated` and refresh this list if a row changed
- [x] `ganda repo audit --fix` for the fixable rows, and commit that diff
- [x] `ganda nuget outdated --update --force` so every outdated package matches the live New column
- [x] Generator, analyzer tests, and dev CLI compile on the new pins
- [x] `/.timewarp/signals/` is ignored and no signal journal is tracked
- [x] `.vscode/settings.json` exists locally and is not committed
- [x] `.timewarp/suppression-baseline.json` is committed
- [x] No new root `PackageReference` to `TimeWarp.SourceGenerators`
- [x] `ganda repo audit` exits 0 with no Failed row
- [x] `./bin/dev build` and `./bin/dev test` succeed
- [x] `## Results` and `### How to validate`
- [x] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

## Results

The live audit matched the 2026-11-09 snapshot: four Failed rows, all fixable. `ganda repo audit --fix` seeded `.timewarp/suppression-baseline.json`, ignored `/.timewarp/signals/`, wrote the gitignored `.vscode/settings.json`, and moved the dev CLI pins. That diff is commit `04e747a`. A second `ganda nuget outdated` then moved the three Roslyn packages to 5.9.0. After both steps, `ganda nuget outdated` reports all 10 central packages up to date.

`ganda repo audit --fix` also set `TimeWarp.Terminal` to `2.0.0-beta.1`. The pre-fix outdated run had called `1.0.2` current. The post-fix outdated run calls `2.0.0-beta.1` current, so the pin stayed.

`Microsoft.CodeAnalysis.CSharp` 5.9.0 still ships `netstandard2.0`. The generator target stayed `netstandard2.0`. The analyzer sources and tests compiled without API edits. `./bin/dev build` is 0 warnings and 0 errors. `./bin/dev test` passes, including the 11 TW0007 analyzer tests.

Nuru 4.0.0-beta.12 pulls TimeWarp.Mediator 15, which removed `ICommandHandler`. The four local handlers implement `IRequestHandler<T, Unit>` and return `ValueTask<Unit>`. `check-version` and `release` now take `NuGetVersionService`, so `IRepoCheckVersionService` is no longer registered. `NuGetPackageService` is constructed in the workflow handler. Its greediest public constructor needs `HttpMessageHandler`, and registering that type would select the test seam. The parameterless constructor keeps the process-wide cache and avoids NURU051.

`./bin/dev --help` lists `release`. `./bin/dev check-version` names `TimeWarp.SourceGenerators` and exits 1 because `1.0.0-beta.12` is already on nuget.org. That gate is the existing package version, not these pins.

### How to validate

Smoke:

```bash
ganda repo audit
ganda nuget outdated
./bin/dev build
./bin/dev test
./bin/dev --help
./bin/dev check-version
git ls-files '.timewarp/signals/*'
git check-ignore -v .vscode/settings.json
```

Expect:

- `ganda repo audit` exits 0. The table has no Failed row (this run: Passed 39, Failed 0, Skipped 3).
- `ganda nuget outdated` prints `All packages are up to date!` for the ten central packages, including Roslyn 5.9.0, Nuru and DevCli `4.0.0-beta.12`, Amuru and Amuru.Tools `3.0.0-beta.2`, and Terminal `2.0.0-beta.1`.
- `./bin/dev build` exits 0 with 0 errors. The generator output is `netstandard2.0`.
- `./bin/dev test` exits 0. The analyzer suite prints `Passed: 11  Failed: 0`.
- `./bin/dev --help` lists the `release` command.
- `./bin/dev check-version` prints `Packages checked: TimeWarp.SourceGenerators`. Exit 1 with "already released" means `1.0.0-beta.12` is still the package version.
- `git ls-files '.timewarp/signals/*'` prints nothing. `.vscode/settings.json` is ignored. `.timewarp/suppression-baseline.json` is tracked.

## Session

- Created: 1998795 (2026-11-09)
- Trigger: Steve asked to create one task that fixes the `ganda repo audit` failures and updates every outdated NuGet package
- Implementation: grok task-work (2026-11-09). Audit `--fix` commit `04e747a`. Roslyn 5.9.0 and the Mediator 15 handler move are in the following commit.
- Implement oracle: implemented by implementer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1c134-f8e8-73a0-8f30-fb2c41982ed1, max-turns 500, Done — 2026-11-09T17:02:28Z

## Notes

- Policy: `ganda repo audit --fix` is the baseline for the four audit rows. Commit it. `ganda nuget outdated --update --force` is the baseline for package versions. If the two disagree on a version, the outdated command's New column wins, and the audit must still pass.
- Do not revert a major bump. The Roslyn 5.9.0 move is in scope.
