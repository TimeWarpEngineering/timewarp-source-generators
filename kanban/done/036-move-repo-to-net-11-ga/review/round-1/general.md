# Round 1 — general
**Date:** 2026-11-11
**Scope reviewed:** branch `task/036-move-repo-to-net-11-ga` (ae1d8c9) vs `master`, all product files in the diff

## Summary

The branch moves the repo from net10 and the 11.0 RC to the .NET 11 GA SDK. CI installs only `11.0.x`. The repo default, both test projects, and the spike target `net11.0`. The shipped generator stays on `netstandard2.0`. The `preview` LangVersion pins are gone, and the package version is bumped to `1.0.0-beta.14` because `check-version` showed beta.13 already on NuGet. All of this matches the task and the implementer restatement.

Checks run in this worktree:
- `./bin/dev test`: Passed 56, Failed 0.
- The spike prints `test`.
- `./bin/dev check-version` says the version is new.
- `ganda repo audit`: the only failure is `kanban-landed`, the host-owned kitchen landing check.
- `git grep` finds no `net10`, `10.0.x`, `11.0.*-rc`, `rc.N`, `LangVersion>preview`, or `net9` in product files. The one hit is the new workflow comment, which mentions net10 on purpose.

There is one problem. The change turns off the `ci-code-coverage` audit check and does not add coverage.

## Issues

### Issue 1 — Severity: bug
- File: .editorconfig:301
- Description: `ci-code-coverage.severity = off` silences the `ci-code-coverage` ganda audit check ("CI collects coverage and uploads…", a Warning). Its comment says the checker has no MTP invocation to edit. That only explains why `--fix` could not wire coverage automatically. It does not show that coverage is irrelevant here: the two console-app suites run by `tools/dev-cli` can still be measured, for example by running the test step under `dotnet-coverage collect`. The line's only effect is to make the audit come back clean while CI still collects no coverage. It is also outside the .NET 11 move the task asked for. Compare the `global-usings-analyzer` opt-out above it, which explains why that check does not apply to this repo.
- Suggestion: Pick one:
  1. Add coverage collection to CI around the dev-cli test run, then drop the opt-out.
  2. Remove the opt-out so the Warning stays visible, which does not block the audit. File a follow-up kanban task to add coverage.
  3. If the operator decides coverage does not apply to this repo, keep the opt-out. Rewrite the comment to give that reason and name the decider, and mark this finding `wontfix` with that rationale.
- Status: open
