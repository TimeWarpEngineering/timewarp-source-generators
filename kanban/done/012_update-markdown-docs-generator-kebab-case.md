# Task 012: Update MarkdownDocsGenerator to Support Kebab-Case File Matching

## Description

- Update the MarkdownDocsGenerator to support matching markdown files with kebab-case naming
- Currently only matches exact class names (e.g., TestClass.md for TestClass.cs)
- Need to support kebab-case files (e.g., test-class.md for test-class.cs)

## Requirements

- Generator must check the actual source file name to determine matching strategy
- Support both exact match (TestClass.md) and kebab-case match (test-class.md)
- Maintain backward compatibility for existing projects
- Match markdown files based on the source file's naming convention

## Checklist

### Design
- [x] Research how to access file path from GeneratorSyntaxContext
- [x] Design matching algorithm that handles both conventions
- [x] Consider performance implications of multiple matching attempts

### Implementation
- [x] Access source file path from ctx.Node.SyntaxTree.FilePath
- [x] Extract file name without extension from source file
- [x] Implement kebab-case conversion utility (ConvertToKebabCase, IsKebabCase methods)
- [x] Update matching logic to try both strategies:
  1. If source file is kebab-case, look for kebab-case markdown
  2. Always fall back to class name match for compatibility
- [x] Handle edge cases (works with partial classes)

### Testing
- [x] Add test for kebab-case source file with kebab-case markdown (kebab-case-test.cs)
- [x] Add test for PascalCase source file with PascalCase markdown (PascalCaseTest.cs)
- [x] Add test for mixed scenarios (test-class.cs with TestClass.md)
- [x] Verify backward compatibility (existing tests still pass)

### Documentation
- [x] Update MarkdownDocsGenerator documentation in overview.md
- [x] Add examples of both naming conventions
- [x] Document the matching behavior in release notes

### Review
- [x] Consider performance implications of double matching (minimal impact)
- [x] Ensure no breaking changes for existing users (fallback ensures compatibility)
- [x] Code tested and working

## Notes

- The generator currently uses: `Path.GetFileNameWithoutExtension(md.Path).Equals(className, StringComparison.OrdinalIgnoreCase)`
- Need to access the source file's actual name, not just the class name
- Consider caching converted names for performance

## Implementation Notes

- The GeneratorSyntaxContext has access to the syntax tree via ctx.Node.SyntaxTree
- SyntaxTree has a FilePath property that gives the full path to the source file
- Can extract the file name and check if it's kebab-case to determine matching strategy