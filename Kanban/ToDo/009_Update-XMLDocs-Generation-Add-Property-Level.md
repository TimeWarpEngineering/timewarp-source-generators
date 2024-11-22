# Task 009: Update XMLDocs Generation - Add Property Level Documentation

## Description

Update the MarkdownDocsGenerator to support property-level documentation from markdown files, similar to how method documentation is currently handled. This will provide complete XML documentation coverage for class properties.

## Requirements

- Parse property sections from markdown files following the template format
- Generate appropriate XML documentation for properties including:
  - Summary
  - Value description
  - Remarks (if any)
  - Example usage (if provided)
- Support both auto-implemented and full properties
- Maintain existing class and method documentation functionality

## Checklist

### Design
- [ ] Update property parsing logic in ConvertMarkdownToXmlDocs method
- [ ] Add PropertySection processing similar to MethodSection processing
- [ ] Add support for property-specific XML tags (<value>, etc.)
- [ ] Add tests for property documentation generation

### Implementation
- [ ] Create ProcessPropertySection method
- [ ] Update property signature extraction from ClassDeclarationSyntax
- [ ] Implement property documentation generation
- [ ] Handle auto-implemented properties
- [ ] Verify integration with existing documentation generation

### Documentation
- [ ] Update example markdown templates with property documentation
- [ ] Add property documentation examples to test cases
- [ ] Update README with property documentation features

### Review
- [ ] Consider Accessibility Implications
- [ ] Consider Monitoring and Alerting Implications
- [ ] Consider Performance Implications
- [ ] Consider Security Implications
- [ ] Code Review

## Notes

Current MarkdownDocsGenerator.cs focuses on class and method documentation. Properties are defined in the XMLDoc-Template.md but not currently implemented in the generator.

Key XML documentation tags for properties:
- `<summary>` - Brief description
- `<value>` - Description of the property value
- `<remarks>` - Additional information
- `<example>` - Usage examples

## Implementation Notes

Will need to extract property declarations similar to how method signatures are currently extracted:
```csharp
var propertySignatures = classDeclaration.Members
    .OfType<PropertyDeclarationSyntax>()
    .ToDictionary(
        p => p.Identifier.Text,
        p => $"public {p.Type} {p.Identifier} {{ {(p.AccessorList?.ToString() ?? "get;")} }}"
    );
```
