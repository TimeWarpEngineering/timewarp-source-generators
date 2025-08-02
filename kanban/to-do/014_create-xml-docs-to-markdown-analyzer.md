# Create XML Docs to Markdown Analyzer

## Description
Create an analyzer that detects existing triple-slash XML documentation comments in C# code and suggests moving them to markdown files. This will work in conjunction with the existing MarkdownDocsGenerator that converts markdown to XML docs.

## Acceptance Criteria
- [ ] Analyzer detects classes/interfaces with XML documentation comments **in non-generated files only**
- [ ] Analyzer ignores all generated files (*.g.cs, *.generated.cs, *.designer.cs, etc.)
- [ ] Analyzer reports diagnostic suggesting to move docs to markdown
- [ ] Code fix provider can extract XML docs to markdown format
- [ ] Extracted markdown follows the same format expected by MarkdownDocsGenerator
- [ ] XML documentation is removed from source after extraction
- [ ] Supports all common XML doc elements (summary, param, returns, remarks, etc.)

## Technical Details
- Diagnostic ID: TW0004
- Category: Documentation
- Severity: Info (configurable)
- Should handle kebab-case and PascalCase file naming conventions

## Implementation Steps
1. Create XmlDocsToMarkdownAnalyzer class
2. Implement syntax node analysis for type declarations
3. **Add logic to skip generated files (check file path for .g.cs, .generated.cs, etc.)**
4. Create XmlDocsToMarkdownCodeFixProvider
5. Implement XML to Markdown conversion logic
6. Handle file creation/updates for markdown docs
7. Add configuration options via .editorconfig
8. Write unit tests for analyzer and fix provider
9. Update documentation

## Important Notes
- The MarkdownDocsGenerator creates *.docs.g.cs files with triple-slash XML comments
- This analyzer should ONLY process source files written by developers
- Must coordinate with existing generated file patterns in the codebase

## Dependencies
- Requires understanding of existing MarkdownDocsGenerator format
- Should follow existing analyzer patterns in the codebase