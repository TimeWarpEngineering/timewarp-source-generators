# Task 011: Create Kebab-Case File Name Rule

## Description

- Create a file name rule analyzer that enforces kebab-case naming convention for C# files
- Implement DiagnosticAnalyzer to check file names and report violations
- Configure editorconfig settings to allow severity customization (suggest/warning/error)
- Support exception patterns for files that should not follow kebab-case (e.g., generated files, special configuration files)

## Requirements

- File names must use kebab-case format (e.g., `my-component.cs`, `user-service.cs`)
- Analyzer should report diagnostic TW0003 for files that don't follow the convention
- Support severity configuration via .editorconfig
- Support exception patterns via .editorconfig to exclude certain files
- Follow existing analyzer patterns in the codebase

## Checklist

### Design
- [x] Research existing source generator structure
- [x] Plan analyzer implementation approach
- [x] Define diagnostic messages and rule metadata
- [x] Design exception configuration mechanism

### Implementation
- [x] Create FileNameRuleAnalyzer.cs implementing DiagnosticAnalyzer
- [x] Add support for reading exception patterns from .editorconfig
- [x] Add TW0003 rule to AnalyzerReleases.Unshipped.md
- [x] Update .editorconfig with TW0003 severity settings and exception patterns
- [x] Ensure analyzer is included in the project build

### Testing
- [x] Manual testing with test console app
- [x] Test valid kebab-case names (kebab-case-test.cs, test-class.cs)
- [x] Test invalid names (PascalCaseTest.cs)
- [x] Test exception patterns (generated files are excluded)
- [x] Verify diagnostic messages and locations

### Documentation
- [x] Document analyzer in file-name-rule-analyzer.md
- [x] Document exception pattern configuration
- [x] Add examples of valid/invalid file names
- [x] Update documentation/developer/reference/analyzers/

### Review
- [x] Verify analyzer works in build process
- [x] Test different severity configurations (set to Info, disabled by default)
- [x] Test exception patterns work correctly
- [x] Code implemented and tested

## Notes

- Rule ID: TW0003
- Category: Naming
- Default Severity: Warning
- Title: "File name should use kebab-case"
- Message Format: "File '{0}' should use kebab-case naming convention (e.g., 'my-file.cs')"
- Description: "C# file names should use kebab-case format with hyphens separating words, all lowercase"

### Exception Configuration
The analyzer will support exception patterns via .editorconfig:
```
# Default exceptions for common patterns
dotnet_diagnostic.TW0003.excluded_files = *.g.cs;*.Generated.cs;*.designer.cs;GlobalUsings.cs;Directory.Build.props;Directory.Build.targets;Program.cs;Startup.cs;AssemblyInfo.cs
```

## Implementation Notes

- Kebab-case pattern: lowercase letters, numbers, and hyphens only
- First character must be a letter
- No consecutive hyphens
- Must end with .cs extension
- Exception patterns will use glob-style matching
- Common exceptions to consider:
  - Generated files (*.g.cs, *.Generated.cs, *.designer.cs)
  - Framework-specific files (Program.cs, Startup.cs)
  - Build configuration files (Directory.Build.props, Directory.Build.targets)
  - Assembly metadata files (AssemblyInfo.cs, GlobalUsings.cs)