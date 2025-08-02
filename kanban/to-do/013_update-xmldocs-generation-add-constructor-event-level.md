# Update XMLDocs Generation - Add Constructor and Event Level Documentation

## Status: TO DO - PREVIEW FEATURE
This task depends on C# preview features for partial constructors and partial events.

## Overview

Extend the XMLDocs source generator to support documentation for constructors and events using the new partial constructors and partial events features being introduced in C#.

## Requirements

### Constructor Documentation
- Support constructor documentation in markdown files
- Generate partial constructor implementing declarations
- Handle multiple constructors with different parameter lists
- Example markdown format:
```markdown
## Constructors

### Constructor
Default constructor that initializes the class.

### Constructor(string name)
Creates a new instance with the specified name.

#### Parameters
- `name` - The name to use for initialization
```

### Event Documentation  
- Support event documentation in markdown files
- Generate partial event implementing declarations
- Example markdown format:
```markdown
## Events

### DataChanged
Occurs when the data has been modified.

#### EventArgs
Uses standard EventArgs.
```

## Technical Considerations

### Partial Constructors
```csharp
// Developer writes (defining declaration):
public partial MyClass(string name);

// Source generator creates (implementing declaration):
/// <summary>
/// Creates a new instance with the specified name.
/// </summary>
/// <param name="name">The name to use for initialization</param>
public partial MyClass(string name) 
{ 
    // Implementation needed - constructors can't use field keyword
}
```

### Partial Events
```csharp
// Developer writes (defining declaration):
public partial event EventHandler DataChanged;

// Source generator creates (implementing declaration):
/// <summary>
/// Occurs when the data has been modified.
/// </summary>
public partial event EventHandler DataChanged
{
    add { field += value; }
    remove { field -= value; }
}
```

## Challenges

1. **Constructor Implementation**: Unlike properties, constructors can't use the `field` keyword. The source generator would need to:
   - Generate empty constructor bodies, OR
   - Allow developers to provide implementation snippets in markdown, OR
   - Skip implementation and only add documentation attributes

2. **Event Accessors**: Events support custom add/remove accessors which may need special handling

3. **Preview Feature**: These features may not be available yet or may change before release

## Implementation Steps

1. Research current status of partial constructors and events in C# preview
2. Create spike to test the syntax and capabilities
3. Update `markdown-docs-generator.cs` to:
   - Extract ConstructorDeclarationSyntax from class members
   - Extract EventDeclarationSyntax from class members
   - Parse "Constructors" and "Events" sections from markdown
   - Generate appropriate partial declarations

## Dependencies

- C# language version that supports partial constructors and events
- May require .NET 10+ or specific preview version

## Notes

⚠️ **Preview Feature Warning**: This functionality depends on C# preview features that may change or be removed before final release. Implementation should be done in a way that can be easily disabled or removed if needed.

## References

- [Partial events and constructors proposal](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/partial-events-and-constructors)