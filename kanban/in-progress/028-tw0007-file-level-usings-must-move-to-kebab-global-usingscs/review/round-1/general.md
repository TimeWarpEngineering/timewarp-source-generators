# Round 1 — general
**Date:** 2026-09-20
**Scope reviewed:** origin/master...HEAD commit b7a6221 (TW0007 GlobalUsingsRuleAnalyzer)

## Summary

TW0007 ships as a `DiagnosticAnalyzer` that registers on every `UsingDirectiveSyntax`, closing the BDSoftware GlobalUsingsAnalyzer gap for TimeWarp namespace-first file-scoped usings. Skips, kebab `global-usings.cs` default, EditorConfig keys, disabled-by-default warning descriptor, Unshipped row, version `1.0.0-beta.11`, docs, and 11/11 CompilationWithAnalyzers cases all match the brief. Risk is low: opt-in diagnostic-only change with no code-fix and no BDSoftware dependency.

## Issues

No issues found.

### Verified (falsifiable)

| Claim | Result |
|-------|--------|
| Namespace-first file-scoped usings visited / warn | `RegisterSyntaxNodeAction(..., SyntaxKind.UsingDirective)` in `global-usings-rule-analyzer.cs:57`; test `NamespaceFirstFileScopedUsing_Should_ReportTw0007` PASS |
| Usings before namespace / block-namespace warn | `UsingsBeforeNamespace_Should_ReportTw0007`, `BlockNamespaceUsing_Should_ReportTw0007` PASS |
| Skips global / static / alias / configured global-usings file | Early returns at analyzer lines 64–81; matching no-warn tests PASS |
| Generated / build-output / `excluded_files` skips | Path guards + `DefaultExceptions` + `ExcludedFilesOption`; `ExcludedFiles_Should_NotReport` PASS |
| Kebab default filename `global-usings.cs` | `DefaultFileName` const; root + test-console `.editorconfig`; descriptor test PASS |
| `isEnabledByDefault: false`; default severity Warning when enabled | Descriptor lines 27–34; `DisabledByDefault_Should_NotReportWithoutEnablement` PASS |
| EditorConfig keys `severity` / `filename` / `excluded_files` | Under `[*.cs]`; constants `FileNameOption` / `ExcludedFilesOption`; PascalCase filename test PASS |
| AnalyzerReleases.Unshipped.md | TW0007 Usage Warning row present |
| Version `1.0.0-beta.11` | `source/Directory.Build.props` |
| No BDSoftware / GlobalUsingsAnalyzer package | Absent from `Directory.Packages.props` and csproj refs; old `globalusingsanalyzer000*` keys removed from root `.editorconfig` |
| Workspaces for tests only; code-fix out of scope | `Microsoft.CodeAnalysis.CSharp.Workspaces` in Packages.props + analyzer-tests csproj; no CodeFix types in `source/` |
| Tests / harness | 11/11 PASS via `dotnet run` analyzer-tests; `test-command.cs` runs test-console then analyzer-tests; slnx includes both |
| Docs / cleanup | readme, releases.md, reference + how-to; file-level using dropped from `interface-delegation-generator.cs`; source + test-console trees clean of file-level non-global usings |
