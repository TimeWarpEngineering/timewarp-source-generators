# Task 004: Create XMLDoc Markdown Template

## Description

Create a standardized markdown template structure that maps directly to C# XMLdoc documentation elements. This template will serve as the foundation for generating XMLdoc comments from markdown files.

## Requirements

- Template must support all common XMLdoc elements:
  - Summary
  - Parameters
  - Returns
  - Remarks
  - Example
  - Exception
  - See also/references
- Template should be intuitive for developers to write in markdown
- Template must maintain a clear 1:1 mapping with XMLdoc structure
- Template should support code blocks for examples
- Template should support linking to other types/members
- Template must be parseable in a consistent way for source generation

## Checklist

### Design
- [ ] Define markdown heading structure
- [ ] Map XMLdoc elements to markdown elements
- [ ] Design linking syntax for cross-references
- [ ] Create example template
- [ ] Add validation rules for template structure

### Implementation
- [ ] Create sample markdown files using template
- [ ] Verify template covers all XMLdoc scenarios
- [ ] Test template with complex documentation cases

### Documentation
- [ ] Document template structure
- [ ] Provide usage examples
- [ ] Document mapping between markdown and XMLdoc elements

### Review
- [ ] Consider Accessibility Implications
- [ ] Consider Documentation Maintainability
- [ ] Consider Template Usability
- [ ] Code Review

## Notes

XMLdoc elements to consider:
- `<summary>`
- `<param name="x">`
- `<returns>`
- `<remarks>`
- `<example>`
- `<exception cref="type">`
- `<see cref="member"/>`
- `<seealso cref="member"/>`
- `<value>`
- `<typeparam name="T">`
- `<inheritdoc/>`

## Implementation Notes

The template should balance being easy to write in markdown while maintaining enough structure to be reliably parsed for source generation.
