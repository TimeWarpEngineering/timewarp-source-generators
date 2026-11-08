# Partial properties and the `field` keyword

XML documentation stays in the C# file. This package does not generate property implementations or documentation from a markdown file.

## Partial properties

A partial property has a defining declaration and an implementing declaration. Both are written in source.

```csharp
public partial class MyClass
{
  public partial string Name { get; set; }
}
```

```csharp
public partial class MyClass
{
  /// <summary>
  /// Gets or sets the name.
  /// </summary>
  public partial string Name
  {
    get => field ?? "";
    set => field = value;
  }
}
```

## The `field` keyword

The `field` keyword is a contextual keyword. Inside a property accessor it names the compiler-generated backing field.

- It is available when the language version includes the feature
- The backing field does not need a separate declaration
- It works with get, set, and init accessors
- For a non-nullable reference type, `field ?? ""` satisfies definite assignment

## References

- [C# field keyword](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/proposals/field-keyword)
