### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
TW0001 | Naming | Info | FileNameRuleAnalyzer, disabled by default; multi-dot kebab basenames accepted (every segment kebab-case)
TW0002 | Documentation | Info | XmlDocsToMarkdownAnalyzer
TW0003 | SourceGenerator | Info | MarkdownDocsGenerator - Enhanced to support kebab-case file matching
TW0007 | Usage | Warning | GlobalUsingsRuleAnalyzer, disabled by default; visits every using including file-scoped namespace-first; default filename kebab global-usings.cs
