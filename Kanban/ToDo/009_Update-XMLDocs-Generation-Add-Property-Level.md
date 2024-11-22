# Update XMLDocs Generation - Add Property Level Documentation

## Description
Extend the MarkdownDocsGenerator to support property-level documentation generation from markdown files. This will allow developers to document class properties with XML documentation comments, similar to how class and method documentation is currently supported.

## Requirements

### Property Documentation Format
- Support property documentation in markdown files under a "Properties" section
- Each property should have:
  - A description
  - A "Value" subsection explaining the property's value
- Example format in markdown:
```markdown
## Properties

### PropertyName
Description of the property

#### Value
Description of the property's value
```

### Generator Updates
1. Update the MarkdownDocsGenerator to:
   - Collect property declarations from class syntax
   - Process property documentation sections from markdown
   - Generate appropriate XML documentation comments for properties
   - Handle property signatures correctly (accessibility, type, get/set modifiers)

2. Support various property types:
   - Auto-implemented properties
   - Properties with backing fields
   - Read-only properties
   - Computed properties

### Expected Output
The generator should produce XML documentation comments for properties in the following format:
```csharp
/// <summary>
/// Property description from markdown
/// </summary>
/// <value>
/// Value description from markdown
/// </value>
public Type PropertyName { get; set; }
```

## Test Cases
1. Test with TestClass.md which includes documentation for:
   - MaxLength (get/set property)
   - ProcessedCount (computed property)
   - LastMessage (get with private set)

## Acceptance Criteria
- [ ] Property documentation is correctly extracted from markdown files
- [ ] XML documentation comments are generated for properties
- [ ] Property signatures are preserved in the generated code
- [ ] All test cases pass and generate expected documentation
- [ ] Generated documentation maintains existing class and method documentation

## Implementation Steps
1. Update ConvertMarkdownToXmlDocs method to handle property sections
2. Add property signature collection similar to method signatures
3. Create ProcessPropertySection method for property documentation
4. Update property documentation processing logic
5. Test with existing TestClass implementation

## Notes
- Follow existing patterns used for class and method documentation
- Maintain consistent XML documentation format
- Ensure backward compatibility with existing documentation
