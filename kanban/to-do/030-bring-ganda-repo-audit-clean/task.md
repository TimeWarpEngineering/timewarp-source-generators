# Bring ganda repo audit clean

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

- [ ] Re-run `ganda repo audit` and refresh this list if a row changed
- [ ] `ganda repo audit --fix` for the fixable rows, excluding a self-package pin (see `global-usings-analyzer`)
- [ ] Commit the `--fix` diff (`chore(audit): …`)
- [ ] `charset = utf-8` under `[*]`, quotes removed
- [ ] `global-usings-analyzer.severity = off` with the reason comment above
- [ ] Split `markdown-docs-generator.cs` under the 500 line cap
- [ ] Nuru and DevCli pins match the audit's latest; `dev build` passes
- [ ] `pull_request.branches` includes `feature/**`
- [ ] `ganda repo audit` exits 0 with no Failed row
- [ ] `## Results` and `### How to validate`
- [ ] Archive task 027 once this kitchen owns the journal ignore (one `.gitignore` edit)
- [ ] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

## Notes

- Predecessor: task **024** brought the audit clean on Nuru / DevCli **3.0.0-beta.76**. This task is the next drift.
- Task **027** (`routine-journals-gitignore` only) is still `to-do`. Its requirements are the journal row above. Archive 027 when this task is claimed so two kitchens do not edit `.gitignore`.
- Task **029** adds `UseTelemetry` to `tools/dev-cli/dev.cs` against the Nuru 3.x generator. Do not run 029 in parallel. This task owns the 4.x pin. 029 should land on top of that pin and re-check the `UseTelemetry` call against 4.x.
- Claim on-create (2026-11-08) failed `dev` self-install: `MSB3030` could not copy `dev.xml` from the runfile publish output. `bin/dev` may be missing in a fresh worktree until self-install works on the new DevCli pin. The claim still stands.
- Policy: `ganda repo audit --fix` output is the baseline. Commit it. The only downgrade allowed here is `global-usings-analyzer`, because this repo is the package the check wants consumers to reference.

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

## Session

- Created: grok `01a1bbd2-4185-7452-8535-389eb11a4f8f` (2026-11-08)
- Trigger: Steve asked to create a task to fix `ganda repo audit` on timewarp-source-generators
