# Fix TW0001 Analyzer to Ignore Generated Files

## Description

The TW0001 file naming analyzer was checking files in build output directories (`obj/`, `bin/`) which contain auto-generated build artifacts. The analyzer should skip validation for these directories as they are not user-written source code.

Folder name on creation still said **TW0003** (the old FileNameRuleAnalyzer id). After 018/020 the rule is **TW0001**; current TW0003 is MarkdownDocsGenerator and is unrelated.

## Problem

The analyzer reported errors for generated files such as:

- `.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` in `obj/`
- `timewarp-code.AssemblyInfo.cs` in `obj/`

These files are created by the build system and their naming conventions are controlled by the .NET SDK, not the user.

## Acceptance Criteria

- [x] TW0001 analyzer skips files in `/obj/` directories
- [x] TW0001 analyzer skips files in `/bin/` directories
- [x] TW0001 analyzer skips files in other common build output directories
- [x] User source files continue to be validated correctly
- [x] Tests verify that generated files are ignored
- [x] Tests verify that regular source files are still checked

Path skip shipped in beta.10. No dedicated analyzer unit-test project: coverage is `dev test` (test-console under TW0001) plus release-note consumer callout. Edge case `obj.cs` in source still analyzed (path-segment skip, not basename).

## Technical Details

Implemented in `FileNameRuleAnalyzer` (`IsBuildOutputOrGeneratedPath`):

1. Normalize separators
2. Skip `/obj/`, `/bin/`, `/artifacts/generated/`
3. Skip `TemporaryGeneratedFile_*`
4. Keep basename exceptions (`*.g.cs`, `*AssemblyInfo.cs`, `*.AssemblyAttributes.cs`, …)

## Implementation Location

`source/timewarp-source-generators/file-name-rule-analyzer.cs`

## Test Cases

- Files in `obj/` / `bin/` / nested `obj`/`bin` ignored
- Regular source files still validated
- Source file named `obj.cs` still checked

## References

- Original issue: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-code/Cramer-2025-07-31-spike/analysis/tw0003-analyzer-issue.md` (not present on this machine)
- PR [#35](https://github.com/TimeWarpEngineering/timewarp-source-generators/pull/35)
- Commit `19fb2c7` — `fix(TW0001): skip obj/bin generated trees; ship 1.0.0-beta.10`

## Notes

Closed by task **025** without claiming this id (`ganda kanban show 017` was the other 017).

## Session

- Created: 2025-08-03
- Shipped: 2026-07-29 (PR #35 / beta.10)
- Board close-out: Grok `01a03b95-a63c-7423-b3e5-ccb1746483e7` via task 025 (2026-08-26)

## Results

### What was implemented

TW0001 `FileNameRuleAnalyzer` skips build-output and generated paths so consumers can enable the rule without false positives on SDK attributes, gRPC stubs, and similar intermediates.

### Files changed (shipped on master)

| Path | Role |
|------|------|
| `source/timewarp-source-generators/file-name-rule-analyzer.cs` | `IsBuildOutputOrGeneratedPath` |
| `source/Directory.Build.props` | Version `1.0.0-beta.10` |
| `documentation/releases.md` | beta.10 consumer callout |

### Key decisions / deviations

- Path skip, not only basename globs (PR #20 already merged default `*.AssemblyInfo.cs` / `*.AssemblyAttributes.cs` exceptions).
- Diagnostic id is **TW0001**, not TW0003.

### Test outcomes

PR #35 CI run succeeded. Package `1.0.0-beta.10` is on `master`.

### How to validate

**Automated**

```bash
./bin/dev build
./bin/dev test
# Expect: 0 errors; test-console kebab/multi-dot fixtures compile
```

**Smoke**

Enable `dotnet_diagnostic.TW0001.severity = error` and build a project that emits `obj/` SDK attributes / protobuf stubs.

**Expect:** no TW0001 on paths under `obj/`, `bin/`, or `artifacts/generated/`; authored PascalCase sources still report.

**Not in scope:** dedicated unit tests for `IsBuildOutputOrGeneratedPath` (no analyzer unit-test project).
