# Update XMLDocs Generation - Add Property Level Documentation

## Status: NOW FEASIBLE WITH .NET 9
This task is now feasible! .NET 9 and C# 13 have added support for partial properties.

### Update (2025-08-01)
Partial properties ARE now supported in .NET 9! Testing confirms that we can have:
- A declaration part: `public partial string TestProperty { get; set; }`
- An implementation part with actual getter/setter logic

This means we can now generate property documentation in the same way we generate method documentation.

See `tests/timewarp-source-generators-test-console/partial-property-test.cs` for a working example of partial properties.

## Original Requirements (For Reference)

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

### Alternative Approaches to Consider
1. Document properties in the same file where they are declared
2. Generate documentation in a separate format (e.g., separate documentation files)
3. Wait for future C# versions that might support partial properties
4. Use a different approach for property documentation that doesn't rely on partial classes

## Conclusion
This task needs to be reconsidered with a different technical approach that works within C#'s current limitations regarding property declarations.
