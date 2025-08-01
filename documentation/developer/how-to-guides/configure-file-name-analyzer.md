# How to Configure the File Name Analyzer

This guide walks you through configuring the TW0003 file name analyzer to enforce kebab-case naming in your project.

## Prerequisites

- TimeWarp.SourceGenerators package referenced in your project
- An `.editorconfig` file in your project root

## Step 1: Enable the Analyzer

Add the following to your `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.TW0003.severity = warning
```

Severity options:
- `none` - Disabled (default)
- `suggestion` - Shows as info in IDE
- `warning` - Shows as warning
- `error` - Fails the build

## Step 2: Configure Exceptions

Add patterns for files that should be excluded:

```ini
dotnet_diagnostic.TW0003.excluded_files = *.g.cs;*.Generated.cs;Program.cs
```

## Step 3: Test the Configuration

1. Build your project
2. Check for TW0003 warnings on non-kebab-case files
3. Verify exceptions are working

## Step 4: Gradual Adoption

For existing projects, start with specific folders:

```ini
# Global setting - suggestion only
[*.cs]
dotnet_diagnostic.TW0003.severity = suggestion

# Strict for new features
[Features/**.cs]
dotnet_diagnostic.TW0003.severity = error
```

## Step 5: Fix Violations

Rename files to kebab-case:
- `UserService.cs` → `user-service.cs`
- `IDataProcessor.cs` → `i-data-processor.cs`

## Troubleshooting

### Too Many Violations
Add more exception patterns or reduce severity to `suggestion`.

### Exceptions Not Working
- Check pattern syntax (use `*` for wildcards)
- Verify `.editorconfig` is in the correct location

### Analyzer Not Running
- Clean and rebuild the project
- Check that the package is properly referenced