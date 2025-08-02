# Create Kebab-Case File Name Code Fix

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