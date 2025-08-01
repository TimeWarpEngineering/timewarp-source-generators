# Source Generators Reference

## Available Source Generators

### HelloWorldGenerator (TW0001)

A simple demonstration source generator that creates a static class with a hello message.

- **Generated File**: `HelloWorld.g.cs`
- **Namespace**: `TimeWarp.Generated`
- **Diagnostic**: Info message when loaded

### MarkdownDocsGenerator (TW0002)

Generates markdown documentation from XML documentation comments.

- **Generated Files**: `*.docs.g.cs` 
- **Purpose**: Creates structured markdown from XMLDoc comments
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