# Partial Properties in C# 13 - Findings

## Test Results

Successfully compiled and ran partial property test with:
- .NET 9.0 with `LangVersion=preview`
- Source generator generating implementing declarations

## How Partial Properties Work

1. **Defining Declaration** (what the developer writes):
   ```csharp
   public partial string Prop { get; set; }
   ```
   This declares the property shape but generates no backing field.

2. **Implementing Declaration** (what the source generator creates):
   ```csharp
   public partial string Prop { get => field; set => field = value; }
   ```
   This provides the actual implementation using the `field` keyword, which creates a compiler-generated backing field.

## Key Learnings

- The `field` keyword requires `LangVersion=preview` in current .NET versions
- The `field` keyword is part of .NET 10 Preview 1 as "field-backed properties"
- Both declarations must be in the same partial class
- The defining declaration is NOT an auto-property - it just declares the property contract
- The implementing declaration uses `field` to access a compiler-generated backing field
- For non-nullable reference types, you can use `field ??= ""` to ensure initialization

## Source Generator Implementation

Our XMLDocs generator now:
1. Looks for partial properties in the user's code (the defining declarations)
2. Generates implementing declarations with the `field` keyword
3. Adds XML documentation to the generated implementation
4. Handles nullable reference types appropriately

Example generated code:
```csharp
/// <summary>
/// A test property demonstrating partial property support.
/// </summary>
/// <value>
/// A string value that can be get and set.
/// </value>
public partial string TestProperty { get => field ??= ""; set => field = value; }
```

## Developer Experience

This approach provides excellent developer experience:
- Developers write simple, familiar property syntax
- No need to write backing fields or property implementations
- XML documentation is automatically generated from markdown files
- The `field` keyword eliminates boilerplate

## Important Clarification

Initially, I was confused about the relationship between field names. For example, `ProcessedCount` property and `ProcessCount` field are completely unrelated - just because they have similar names doesn't mean one backs the other. The `field` keyword creates its own backing field for each property.

## References

- [C# field keyword proposal](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/field-keyword)
- [.NET 10 Preview 1 C# features](https://github.com/dotnet/core/blob/main/release-notes/10.0/preview/preview1/csharp.md)