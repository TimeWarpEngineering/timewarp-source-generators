### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
TW0001 | Naming | Info | FileNameRuleAnalyzer, disabled by default; multi-dot kebab basenames accepted (every segment kebab-case)
TW0007 | Usage | Warning | GlobalUsingsRuleAnalyzer, disabled by default; visits every using including file-scoped namespace-first; default filename kebab global-usings.cs
TWA0004 | Documentation | Warning | PurposeRegionAnalyzer, enabled by default; moved from TimeWarp.Architecture.Analyzers with the same id
TWA0023 | Naming | Warning | TypeStemIdentifierAnalyzer, disabled by default; moved from TimeWarp.Architecture.Analyzers with the same id
TWA0038 | Maintainability | Warning | FileLengthAnalyzer, enabled by default; moved from TimeWarp.Architecture.Analyzers with the same id; max_lines editorconfig key
