# Analyzers Reference

**TW0001** and **TW0007** are this package's own ids. **TWA0004**, **TWA0023**, and **TWA0038** are generic style rules moved from `TimeWarp.Architecture.Analyzers` with those ids kept, so an existing `.editorconfig` key still applies. Every other `TWA*` id stays in TimeWarp.Architecture.Analyzers. Do not configure TW0001 or TW0007 with a `TWA*` id.

## Available Analyzers

### TW0001 - File Name Rule Analyzer

Enforces kebab-case naming convention for C# source files (including multi-segment kebab basenames such as `application-state.close-modal.cs`).

- **Rule ID**: TW0001
- **Category**: Naming  
- **Default Severity**: Info (disabled)
- **Configuration**: `.editorconfig`

[Full Reference](./file-name-rule-analyzer.md)

### TW0007 - Global Usings Rule Analyzer

Reports file-level `using` directives (including namespace-first file-scoped usings) that should move to kebab-case `global-usings.cs`.

- **Rule ID**: TW0007
- **Category**: Usage
- **Default Severity**: Warning (disabled)
- **Configuration**: `.editorconfig` (`filename`, `excluded_files`)

[Full Reference](./global-usings-rule-analyzer.md)

### TWA0004 - Purpose Region Analyzer

Reports a non-generated C# file that has no `#region Purpose` block. Moved from TimeWarp.Architecture.Analyzers. The id is still TWA0004.

- **Rule ID**: TWA0004
- **Category**: Documentation
- **Default Severity**: Warning (enabled)

[Full Reference](./purpose-region-analyzer.md)

### TWA0023 - Type Stem Identifier Analyzer

Reports an identifier that does not end with its type stem. Interfaces drop a leading `I`. Moved from TimeWarp.Architecture.Analyzers. The id is still TWA0023. Off by default.

- **Rule ID**: TWA0023
- **Category**: Naming
- **Default Severity**: Warning (disabled)

[Full Reference](./type-stem-identifier-analyzer.md)

### TWA0038 - File Length Analyzer

Reports a C# file longer than 500 lines. `dotnet_diagnostic.TWA0038.max_lines` overrides the limit. Moved from TimeWarp.Architecture.Analyzers. The id is still TWA0038.

- **Rule ID**: TWA0038
- **Category**: Maintainability
- **Default Severity**: Warning (enabled)

[Full Reference](./file-length-analyzer.md)

## Common Configuration

All analyzers are configured through `.editorconfig`:

```ini
# Enable an analyzer
dotnet_diagnostic.TW0001.severity = warning

# Configure exceptions
dotnet_diagnostic.TW0001.excluded_files = *.g.cs;*.Generated.cs
```

## Analyzer Categories

- **Naming**: File names (TW0001) and type-stem identifiers (TWA0023)
- **Usage**: Using-directive placement (TW0007)
- **Documentation**: Purpose region (TWA0004)
- **Maintainability**: File length (TWA0038)

## Suppression Methods

1. **File-level pragma**:
   ```csharp
   #pragma warning disable TW0001
   ```

2. **Project-wide**:
   ```xml
   <NoWarn>TW0001</NoWarn>
   ```

3. **EditorConfig**:
   ```ini
   dotnet_diagnostic.TW0001.severity = none
   ```
