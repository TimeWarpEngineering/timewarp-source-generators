# Analyzers Reference

## Available Analyzers

### TWA001 - File Name Rule Analyzer

Enforces kebab-case naming convention for C# source files.

- **Rule ID**: TWA001
- **Category**: Naming  
- **Default Severity**: Info (disabled)
- **Configuration**: `.editorconfig`

[Full Reference](./file-name-rule-analyzer.md)

## Common Configuration

All analyzers are configured through `.editorconfig`:

```ini
# Enable an analyzer
dotnet_diagnostic.TWA001.severity = warning

# Configure exceptions
dotnet_diagnostic.TWA001.excluded_files = *.g.cs;*.Generated.cs
```

## Analyzer Categories

- **Naming**: File and identifier naming conventions
- **SourceGenerator**: Diagnostics from source generators

## Suppression Methods

1. **File-level pragma**:
   ```csharp
   #pragma warning disable TWA001
   ```

2. **Project-wide**:
   ```xml
   <NoWarn>TWA001</NoWarn>
   ```

3. **EditorConfig**:
   ```ini
   dotnet_diagnostic.TWA001.severity = none
   ```