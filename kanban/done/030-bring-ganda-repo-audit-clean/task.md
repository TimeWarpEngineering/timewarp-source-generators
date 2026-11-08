# Bring ganda repo audit clean

## Goal (implementer restatement)

`ganda repo audit` on this repo currently fails. The goal is a clean audit (exit 0, no Failed row) on the claimed task branch, with the product still building.

Acceptance:

- Re-read the audit at the start and treat that live table as the list, because pins move.
- Apply `ganda repo audit --fix` for every fixable row except `global-usings-analyzer`, and commit that diff. Do not revert a major Nuru or DevCli bump.
- For `global-usings-analyzer`, turn the check off in `[ganda.audit]` with the reason comment from the kitchen. Do not add a root `PackageReference` to `TimeWarp.SourceGenerators`, because this repo is that package.
- Put unquoted `charset = utf-8` under `[*]`.
- Split `markdown-docs-generator.cs` by responsibility so every file is at most 500 lines. No `part-N` partial names. Analyzer behavior stays.
- Bump `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` to the version the audit calls latest, and make `./bin/dev build` succeed. `dev --help` still shows `release`, and `dev check-version` still sees `TimeWarp.SourceGenerators`.
- `pull_request.branches` includes `feature/**`.
- After the journal ignore is in `.gitignore`, archive task 027 so it does not edit the same file.
- Record `## Results` and a non-empty `### How to validate` (Smoke + Expect). Stop before `kanban done` and `gh pr create`.

## Description

`ganda repo audit` on `timewarp-source-generators` master fails.
Snapshot from 2026-11-08 (`origin/master`, clean): **Passed 25, Failed 12, Skipped 3**, exit 1.
Ten rows are Error. Two are Warning (`memsearch-scaffold`, `vscode-window-icon`). Both count as Failed.

Bring the repo to a passing audit. Run `ganda repo audit --fix` for the rows it can fix, commit that result, then hand-fix the rest. A major package bump from `--fix` is the baseline (do not revert it). Downgrade a check under `[ganda.audit]` only when the check does not apply, with a comment that names the reason and this task id.

Re-read `ganda repo audit` at the start. Pins and "latest" versions move. The list below is the 2026-11-08 snapshot.

## Requirements

- `ganda repo audit` exits 0. The results table has no Failed row.
- `./bin/dev build` succeeds after the Nuru / DevCli pin move (0 errors).
- Every `[ganda.audit]` downgrade has a comment with the reason and task 030.
- Do not implement on `master`. Work stays in the claimed task worktree.

### Fix with `ganda repo audit --fix`

These rows reported Fixable YES. Commit the `--fix` output. Do not hand-revert it.

- `glossary` — `glossary.md` is missing.
- `kanban-porcelain-gitignore` — root `.gitignore` missing porcelain lines (`*.journal.json`, `*.next.md`, `*.progress.log`, `**/review/round-*/base.sha`, `**/review/round-*/closed`, `oracle-*.log`, `task-work.progress.log`, `mission.heartbeat`).
- `required-gitignore-entries` — missing `.local/`, `bin/dev`, `bin/dev.dbg`, `bin/dev.exe`.
- `routine-journals-gitignore` — same journal globs, plus `stacked-task-set*.resume.json`. One `*.journal.json` line covers the journal basenames. Do not commit journal contents. `git ls-files '*.journal.json'` must be empty.
- `memsearch-scaffold` (Warning) — leftover `.memsearch.toml`; outdated `.githooks/post-commit.cs`, `post-merge.cs`, `post-checkout`, `pre-commit`, `pre-push`.
- `vscode-settings-gitignore` — `.vscode/settings.json` is tracked. `--fix` runs `git rm --cached` and adds the ignore. Keep the working copy. Do not commit `settings.json`.
- `vscode-window-icon` (Warning) — `.vscode/settings.json` does not set `peacock.color`. Set it in the working copy after the file is untracked.
- `workflow-file` — `.github/workflows/workflow.yml` `pull_request.branches` is `[master]`. It must include `feature/**`.
- `nuru` — `Directory.Packages.props` pins `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` at `3.0.0-beta.76`. Audit latest on this date: **4.0.0-beta.11** (nuget.org). Bump **both** packages to the version the audit reports when the work starts. This is a 3.x → 4.x move. Update `tools/dev-cli` if the 4.x API no longer compiles. `dev --help` must still show `release`, and `dev check-version` must still see `TimeWarp.SourceGenerators`.

### Hand fixes (`--fix` will not do these)

- `editorconfig-charset` (Error, not fixable) — `charset` is `"utf-8"` under the `[*.{cs,...}]` glob. The check wants `charset = utf-8` (no quotes) under `[*]`.
- `source-file-line-cap` (Error, not fixable) — `source/timewarp-source-generators/markdown-docs-generator.cs` is **532** lines. Cap is **500**. Split by responsibility. No partial-class `part-N` names. Behavior stays; analyzer tests still pass.
- `global-usings-analyzer` (Error, fixable, **do not take the default fix**). The check looks for the string `TimeWarp.SourceGenerators` in root `Directory.Packages.props` and root `Directory.Build.props`. `--fix` would pin the NuGet package and add a root `PackageReference`. This repo **is** that package. A root reference would make the generator project consume the published package. Set:

  ```ini
  # This repo is the TimeWarp.SourceGenerators package. A root PackageReference
  # would consume the published package from the project that produces it.
  # TW0007 is implemented and tested here (task 028). Task 030.
  global-usings-analyzer.severity = off
  ```

  Leave the existing `dotnet_diagnostic.TW0007.filename = global-usings.cs` line. Do not add a `PackageReference` to `TimeWarp.SourceGenerators`.

## Checklist

- [x] Re-run `ganda repo audit` and refresh this list if a row changed
- [x] `ganda repo audit --fix` for the fixable rows, excluding a self-package pin (see `global-usings-analyzer`)
- [x] Commit the `--fix` diff (`chore(audit): …`)
- [x] `charset = utf-8` under `[*]`, quotes removed
- [x] `global-usings-analyzer.severity = off` with the reason comment above
- [x] Split `markdown-docs-generator.cs` under the 500 line cap
- [x] Nuru and DevCli pins match the audit's latest; `dev build` passes
- [x] `pull_request.branches` includes `feature/**`
- [x] `ganda repo audit` exits 0 with no Failed row
- [x] `## Results` and `### How to validate`
- [x] Archive task 027 once this kitchen owns the journal ignore (one `.gitignore` edit)
- [x] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

## Review

- Round 1: Done

## Notes

- Predecessor: task **024** brought the audit clean on Nuru / DevCli **3.0.0-beta.76**. This task is the next drift.
- Task **027** (`routine-journals-gitignore` only) was archived from origin-home on 2026-11-08 (`09a725d land kanban 027`) after this branch committed the journal ignore. Archive refuses on the 030 branch because that branch is not 027's task branch. The to-do copy remains on this branch; master owns the archived kitchen, and this branch did not edit that file.
- Task **029** adds `UseTelemetry` to `tools/dev-cli/dev.cs` against the Nuru 3.x generator. Do not run 029 in parallel. This task owns the 4.x pin. 029 should land on top of that pin and re-check the `UseTelemetry` call against 4.x.
- Claim on-create (2026-11-08) failed `dev` self-install: `MSB3030` could not copy `dev.xml` from the runfile publish output. `bin/dev` may be missing in a fresh worktree until self-install works on the new DevCli pin. The claim still stands.
- Policy: `ganda repo audit --fix` output is the baseline. Commit it. The only downgrade allowed here is `global-usings-analyzer`, because this repo is the package the check wants consumers to reference.
- Live re-read before `--fix` (this worktree, 2026-11-08): **Passed 27, Failed 10, Skipped 3**. `kanban-porcelain-gitignore` and `routine-journals-gitignore` already passed from the uncommitted gitignore lines. Nuru latest was still **4.0.0-beta.11**. `--fix` also pinned `TimeWarp.Amuru` and `TimeWarp.Amuru.Tools` to **3.0.0-beta.1** and `TimeWarp.Terminal` to **1.0.2**. Amuru 3 is net11.0 only, so the dev CLI and the git hooks target net11.0, and CI installs `11.0.100-rc.3.26522.148` beside `10.0.x`.

### How to validate

```bash
ganda repo audit
# Expect: exit 0, no Failed row

grep -E 'TimeWarp\.Nuru' Directory.Packages.props
# Expect: Nuru and DevCli at the version the audit called latest

./bin/dev --help | grep release
./bin/dev check-version
./bin/dev build
# Expect: build succeeds

# charset
grep -n 'charset' .editorconfig
# Expect: charset = utf-8 under [*]

# no self-package reference
grep -n 'PackageReference Include="TimeWarp.SourceGenerators"' Directory.Build.props Directory.Packages.props
# Expect: no PackageReference (PackageId in source/Directory.Build.props is the product id, and is fine)

wc -l source/timewarp-source-generators/markdown-docs-generator.cs
# Expect: the split files are each <= 500 lines; the old monolith is gone or under the cap
```

## Results

`ganda repo audit` exits 0 with no Failed row (**Passed 36, Failed 0, Skipped 3**). `global-usings-analyzer` is off under `[ganda.audit]` with the reason comment, and there is no root `PackageReference` to `TimeWarp.SourceGenerators`.

`--fix` is committed as `0eac4eb` (`chore(audit): apply ganda repo audit --fix`). That commit keeps the Amuru 3 and Terminal bumps. Hand fixes are `a936c58`.

Nuru and DevCli are **4.0.0-beta.11**. Nuru 4 moved `ICommand`, `ICommandHandler`, and `Unit` to `TimeWarp.Mediator`, and handlers return `Task<Unit>`. The dev CLI compiles on net11.0. `dotnet run --file tools/dev-cli/dev.cs -- --help` lists `release`. `check-version` prints `Packages checked: TimeWarp.SourceGenerators` and exits 1 because `1.0.0-beta.11` is already on NuGet. That exit is the release guard, not a missed package.

`markdown-docs-generator.cs` is 136 lines. Markdown parsing lives in `markdown-docs-generator.markdown-to-xml.cs` (409 lines). `./bin/dev build` succeeded with 0 warnings. `./bin/dev test` passed the test console and 11/11 analyzer tests. The Nuru 4 dev CLI `build` command also succeeded.

Task 027 is archived on master (`09a725d`). This walk did not run `ganda kanban done` or `gh pr create`.

CI installs the net11 RC next to net10 so `dotnet run --file tools/dev-cli/dev.cs` can restore Amuru 3. .NET 11 was still `11.0.0-rc.3` on 2026-11-08.

### Review disposition

- **Disposition:** clean. Rounds: 1. Effort 3, roster: general.
- **Final counts:** bug 0, suggestion 0, nit 0 (0 open, 0 fixed, 0 wontfix).
- **Live checks by the reviewer:** `ganda repo audit` exit 0 (Passed 36, Failed 0, Skipped 3). `./bin/dev build` reported 0 warnings and 0 errors. `./bin/dev test` passed 11/11. `check-version` lists `TimeWarp.SourceGenerators`. The generator split is confirmed as a pure move.
- **Follow-up (not a finding):** after .NET 11 GA, change the CI SDK line `11.0.100-rc.3.26522.148` to `11.0.x`.
- **Paths:** `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

### How to validate

**Smoke**

```bash
ganda repo audit
grep -E 'TimeWarp\.Nuru' Directory.Packages.props
./bin/dev --help | grep release
./bin/dev check-version
./bin/dev build
./bin/dev test
grep -n 'charset' .editorconfig
grep -n 'PackageReference Include="TimeWarp.SourceGenerators"' Directory.Build.props Directory.Packages.props || true
wc -l source/timewarp-source-generators/markdown-docs-generator.cs source/timewarp-source-generators/markdown-docs-generator.markdown-to-xml.cs
```

**Expect**

- `ganda repo audit` exits 0 and the table has no Failed row.
- `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` are `4.0.0-beta.11`.
- Help text includes `release`. `check-version` lists `TimeWarp.SourceGenerators`.
- `./bin/dev build` succeeds with 0 errors. `./bin/dev test` passes the analyzer cases.
- `.editorconfig` has `charset = utf-8` under `[*]` and no quoted charset.
- No `PackageReference` to `TimeWarp.SourceGenerators`.
- Both markdown generator files are at most 500 lines.

## Session

- Created: grok `01a1bbd2-4185-7452-8535-389eb11a4f8f` (2026-11-08)
- Trigger: Steve asked to create a task to fix `ganda repo audit` on timewarp-source-generators
- Implementation: grok `01a1bbe3-2e77-79f0-ae81-48feae04ec9c` (2026-11-08)
- Implement oracle: implemented by implementer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1bbe3-2e77-79f0-ae81-48feae04ec9c, max-turns 500, Done — 2026-11-08T16:21:32Z
- Review oracle: tw-implementation-review round 1, Claude (claude-opus-5-5), ganda task work headless, effort 3, roster general, turn cap 200. Disposition clean — 2026-11-08
- Review oracle: review by reviewer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 96240dfd-27eb-43b9-ac1e-aea0e6bdce7d, max-turns 200, artifacts general.md, merged.md, disposition.md — 2026-11-08T16:24:11Z
