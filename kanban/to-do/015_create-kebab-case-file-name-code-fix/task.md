# Create Kebab-Case File Name Code Fix

## Status: Blocked - Requires Separate Assembly

### Issue Discovered
Code fix providers cannot be in the same assembly as source generators due to RS1038 error. The Microsoft.CodeAnalysis.Workspaces assembly (required for code fixes) is not provided during command line compilation scenarios.

## Description
Create a code fix provider for the FileNameRuleAnalyzer (TW0003) that automatically renames files from PascalCase or other naming conventions to kebab-case format.

## Acceptance Criteria
- [ ] Code fix provider offers to rename file to kebab-case when diagnostic is reported
- [ ] Correctly converts PascalCase to kebab-case (e.g., MyTestClass.cs → my-test-class.cs)
- [ ] Handles edge cases (consecutive capitals, numbers, existing hyphens)
- [ ] Updates all references to the file in the project
- [ ] Preserves file extension
- [ ] Works with version control (git mv if applicable)
- [ ] Provides preview of the new file name before applying

## Technical Details
- Diagnostic ID: TW0003
- Fix Provider: FileNameRuleCodeFixProvider
- Should integrate with existing FileNameRuleAnalyzer

## Implementation Steps
1. Create FileNameRuleCodeFixProvider class
2. Register the code fix for TW0003 diagnostic
3. Implement PascalCase to kebab-case conversion logic
4. Handle file renaming through Roslyn workspace APIs
5. Ensure all project references are updated
6. Add support for bulk fixes (fix all occurrences)
7. Write unit tests for various naming patterns
8. Test integration with source control

## Test Cases
- PascalCase: `MyTestClass.cs` → `my-test-class.cs`
- Mixed case: `myTestClass.cs` → `my-test-class.cs`
- With numbers: `MyTest123Class.cs` → `my-test-123-class.cs`
- Acronyms: `XMLParser.cs` → `xml-parser.cs`
- Already kebab-case: `my-test.cs` → no change

## Dependencies
- Requires FileNameRuleAnalyzer (TW0003) to be working
- Should follow existing code fix provider patterns in the codebase

## Implementation Progress
- [x] Created FileNameRuleCodeFixProvider class
- [x] Implemented PascalCase to kebab-case conversion logic
- [x] Added file renaming through Roslyn workspace APIs
- [ ] Blocked: Cannot include in same assembly as source generators

## Next Steps
To complete this task, one of the following approaches is needed:
1. Create a separate project for code fix providers (e.g., `timewarp-source-generators.codefixes`)
2. Distribute code fixes as a separate NuGet package
3. Restructure the solution to support both analyzers and code fixes properly

The code fix provider implementation is complete and stored in this folder for when the project structure supports it.