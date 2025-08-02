# Partial Properties with the field Keyword (C# 14 Preview)

> **⚠️ Preview Feature Warning**
> 
> This documentation describes features available in C# 14 preview with `LangVersion=preview`. 
> These features are subject to change before the final release in November 2025.
> The implementation may break or change significantly.

## Overview

The XMLDocs source generator now supports generating documentation for partial properties using the new `field` keyword introduced in .NET 10 Preview 1. This feature allows developers to write simple property declarations while the source generator provides both the implementation and XML documentation.

## How It Works

### 1. Developer Writes the Defining Declaration

```csharp
public partial class MyClass
{
    public partial string Name { get; set; }
    public partial int Count { get; }
    public partial string? Description { get; set; }
}
```

### 2. Source Generator Creates the Implementing Declaration

```csharp
public partial class MyClass
{
    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    /// <value>
    /// The name of the item.
    /// </value>
    public partial string Name { get => field ??= ""; set => field = value; }
    
    /// <summary>
    /// Gets the count.
    /// </summary>
    /// <value>
    /// The total count.
    /// </value>
    public partial int Count { get => field; }
    
    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    /// <value>
    /// The optional description.
    /// </value>
    public partial string? Description { get => field; set => field = value; }
}
```

## The `field` Keyword

The `field` keyword is a contextual keyword that provides access to a compiler-generated backing field within property accessors. Key points:

- Only available with `LangVersion=preview` in current .NET versions
- Creates a backing field automatically - no need to declare one
- Works with get, set, and init accessors
- For non-nullable reference types, use `field ??= ""` to ensure initialization

## Requirements

1. **Language Version**: Must set `<LangVersion>preview</LangVersion>` in your project file
2. **Partial Properties**: Properties must be marked with the `partial` modifier
3. **Markdown Documentation**: Create a corresponding `.md` file with property documentation

## Example Markdown Format

```markdown
## Properties

### Name
Gets or sets the name.

#### Value
The name of the item.

### Count
Gets the count.

#### Value
The total count.
```

## Benefits

1. **Clean Code**: Developers write simple property declarations without backing fields
2. **Generated Documentation**: XML documentation is automatically generated from markdown
3. **Type Safety**: Proper handling of nullable reference types
4. **No Boilerplate**: The `field` keyword eliminates manual backing field management

## Limitations

- Properties must be self-contained (can't reference other fields)
- Requires preview language features
- Subject to breaking changes before C# 14 release

## Migration Path

When C# 14 is officially released:
1. Remove `LangVersion=preview` if using the default `latest`
2. Test for any breaking changes in the `field` keyword behavior
3. Update this documentation to reflect the final specification

## References

- [C# field keyword proposal](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/field-keyword)
- [.NET 10 Preview 1 C# features](https://github.com/dotnet/core/blob/main/release-notes/10.0/preview/preview1/csharp.md)