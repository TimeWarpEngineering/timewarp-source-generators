# Source Generators Reference

## Available Source Generators

### MarkdownDocsGenerator (TWG001)

Generates XML documentation from markdown files for C# classes.

- **Generated Files**: `*.docs.g.cs` 
- **Purpose**: Converts markdown documentation to XML documentation comments
- **File Matching**: 
  - Supports both PascalCase and kebab-case naming conventions
  - For kebab-case source files (e.g., `test-class.cs`), matches kebab-case markdown files (e.g., `test-class.md`)
  - For PascalCase source files (e.g., `TestClass.cs`), matches PascalCase markdown files (e.g., `TestClass.md`)
  - Always falls back to class name matching for backward compatibility
- **Templates**: See [XMLDoc Templates](#xmldoc-templates)

## XMLDoc Templates

The MarkdownDocsGenerator uses templates to structure the output:

- [XMLDoc Template](./xmldoc-template.md) - Base template structure
- [XMLDoc Template README](./xmldoc-template-readme.md) - Template usage guide
- [XMLDoc Template Example](./xmldoc-template-example.md) - Example implementation

## Source Generator Basics

### Registration

Source generators are registered using the `[Generator]` attribute:

```csharp
[Generator]
public class MyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Generator logic
    }
}
```

### Output

Generated files are placed in:
```
Generated/
└── TimeWarp.SourceGenerators/
    └── TimeWarp.SourceGenerators.GeneratorName/
        └── generated-file.g.cs
```

## Best Practices

1. Use `IIncrementalGenerator` for performance
2. Include `.g.cs` suffix for generated files
3. Place in unique namespaces to avoid conflicts
4. Report diagnostics for visibility