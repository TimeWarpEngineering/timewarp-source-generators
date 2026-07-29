### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
TW0001 | Naming | Info | FileNameRuleAnalyzer, disabled by default; multi-dot kebab basenames accepted (every segment kebab-case)
TW0002 | Documentation | Info | XmlDocsToMarkdownAnalyzer
TW0003 | SourceGenerator | Info | MarkdownDocsGenerator - Enhanced to support kebab-case file matching
TW0004 | InterfaceDelegation | Error | Class must be partial for interface delegation
TW0005 | InterfaceDelegation | Error | Class does not implement the delegated interface
TW0006 | InterfaceDelegation | Error | Multiple fields delegate the same interface
