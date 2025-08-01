# Analyzers Reference

## Available Analyzers

### TW0003 - File Name Rule Analyzer

Enforces kebab-case naming convention for C# source files.

- **Rule ID**: TW0003
- **Category**: Naming  
- **Default Severity**: Info (disabled)
- **Configuration**: `.editorconfig`

[Full Reference](./file-name-rule-analyzer.md)

## Common Configuration

All analyzers are configured through `.editorconfig`:

```ini
# Enable an analyzer
dotnet_diagnostic.TW0003.severity = warning

# Configure exceptions
dotnet_diagnostic.TW0003.excluded_files = *.g.cs;*.Generated.cs
```

## Analyzer Categories

- **Naming**: File and identifier naming conventions
- **SourceGenerator**: Diagnostics from source generators

## Suppression Methods

1. **File-level pragma**:
   ```csharp
   #pragma warning disable TW0003
   ```

2. **Project-wide**:
   ```xml
   <NoWarn>TW0003</NoWarn>
   ```

3. **EditorConfig**:
   ```ini
   dotnet_diagnostic.TW0003.severity = none
   ```