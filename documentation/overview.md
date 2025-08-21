# TimeWarp Source Generators Documentation

## Quick Start

1. **Add the package**: Reference TimeWarp.SourceGenerators
2. **Configure analyzers**: Edit `.editorconfig`
3. **Build**: See diagnostics and generated code

## Documentation

### [Developer Documentation](./developer/overview.md)
Technical documentation for developers using or contributing to the project.

## What's Included

### Source Generators
- **MarkdownDocsGenerator** - Generates markdown from XML docs

### Analyzers
- **TWA001** - File name kebab-case enforcement

## Configuration

All configuration through `.editorconfig`:

```ini
# Example: Enable kebab-case file names
dotnet_diagnostic.TW0003.severity = warning
```

## Philosophy

- Everything uses kebab-case
- Opt-in by default
- Performance first
- Highly configurable