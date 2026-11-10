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
- **TWA0004** - `#region Purpose` in every non-generated C# file (moved from Architecture.Analyzers, same id)
- **TWA0023** - Identifier ends with its type stem (moved, same id, off by default)
- **TWA0038** - File length, 500 lines by default (moved, same id)

This package generates no source. TW0001 runs as an incremental generator that only reports diagnostics. TW0007, TWA0004, TWA0023, and TWA0038 are diagnostic analyzers. XML documentation stays in the C# file.

**TW0001** and **TW0007** use the **TW** prefix. **TWA0004**, **TWA0023**, and **TWA0038** keep the Architecture ids so existing `.editorconfig` keys still apply. Every other **TWA\*** id stays in TimeWarp.Architecture.Analyzers.

## Configuration

All configuration through `.editorconfig`:

```ini
# Example: Enable kebab-case file names
dotnet_diagnostic.TW0001.severity = warning
```

## Philosophy

- Everything uses kebab-case
- TW0001, TW0007, and TWA0023 are off until `.editorconfig` enables them
- TWA0004 and TWA0038 warn by default, matching TimeWarp.Architecture.Analyzers
- Performance first
- Highly configurable
