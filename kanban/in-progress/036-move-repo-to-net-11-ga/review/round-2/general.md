# Round 2 — general
**Date:** 2026-11-11
**Scope reviewed:** delta `ae1d8c9..7c04bdc` (round-1 base to HEAD). Product files: `.config/dotnet-tools.json`, `.editorconfig`, `.github/workflows/workflow.yml`, `tools/dev-cli/endpoints/test-command.cs`, `tools/dev-cli/endpoints/workflow-command.cs`. Kitchen: `task.md`, round-1 ledger.

## Summary

The fix pass removes the `ci-code-coverage.severity = off` opt-out and adds real coverage. `dotnet-coverage` 18.12.0 is pinned as a local tool. `dev test --coverage` restores it and wraps each Release console run in `dotnet-coverage collect`, writing one Cobertura file per suite under `artifacts/coverage`. A missing output file fails the run. The in-process `workflow` command (pr/merge and release paths) sets `Coverage = true`, and the GitHub workflow uploads `artifacts/coverage` as artifact `coverage`. A plain `dev test` run is unchanged. This matches the round-1 suggestion (option 1) and the updated implementer restatement.

Checks run in this worktree:
- `dotnet run --file tools/dev-cli/dev.cs -- test --coverage --quiet`: tests completed successfully; both `artifacts/coverage/*.cobertura.xml` files were rewritten.
- The analyzer-tests report contains package `timewarp-source-generators` (line-rate 0.741), so the shipped generator is measured, not only the test assembly.
- `DOTNET_ROLL_FORWARD=Major dotnet dotnet-coverage collect … -- sh -c "exit 3"` exits 3, so a failing suite still fails `ExecuteAsync` under coverage.
- `ganda repo audit`: Passed 42, Failed 0, Skipped 3; `ci-code-coverage` PASS with no severity override.
- `nuget.config` lists only nuget.org, which hosts `dotnet-coverage`, so `dotnet tool restore` works on the ubuntu-latest runner.

M1 from round 1 is fixed.

## Issues

### Issue 1 — Severity: nit
- File: kanban/in-progress/036-move-repo-to-net-11-ga/task.md (## Results)
- Description: Results said the analyzer-tests report "includes package `timewarp-source-generators` at line-rate about 89.3%". 0.893 is the whole report's top-level rate, which includes the test assembly. The `timewarp-source-generators` package line-rate is 0.741.
- Suggestion: Quote the package rate, or say the 89.3% is the report total.
- Status: fixed (reviewer corrected the kitchen `## Results` text; no product change)
