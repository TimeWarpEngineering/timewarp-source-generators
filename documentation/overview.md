# TimeWarp Source Generators Documentation

## Quick Start

1. **Add the package**: Reference TimeWarp.SourceGenerators
2. **Configure analyzers**: Edit `.editorconfig`
3. **Build**: See diagnostics and generated code

## Documentation

### [Developer Documentation](./developer/overview.md)
Technical documentation for developers using or contributing to the project.

## What's Included

### Analyzers
- **TW0001** - File name kebab-case enforcement (single- and multi-dot basenames)
- **TW0007** - File-level usings belong in kebab-case `global-usings.cs`

This package does not ship a source generator. XML documentation stays in the C# file.

Diagnostic IDs from this package use the **TW** prefix (**TW0001** and **TW0007**). The **TWA** prefix is reserved for TimeWarp Architecture and is not used here.

## Configuration

All configuration through `.editorconfig`:

```ini
# Example: Enable kebab-case file names
dotnet_diagnostic.TW0001.severity = warning
```

## Philosophy

- Everything uses kebab-case
- Opt-in by default
- Performance first
- Highly configurable
