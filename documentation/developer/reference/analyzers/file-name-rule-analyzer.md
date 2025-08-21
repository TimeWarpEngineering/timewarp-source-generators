# FileNameRuleAnalyzer (TWA001)

## Overview

The FileNameRuleAnalyzer enforces kebab-case naming conventions for C# source files. This analyzer helps maintain consistent file naming across projects that prefer kebab-case (e.g., `my-component.cs`) over traditional PascalCase.

## Rule Details

- **Rule ID**: TWA001
- **Category**: Naming
- **Default Severity**: Info (disabled by default)
- **Message**: File '{0}' should use kebab-case naming convention (e.g., 'my-file.cs')

## Kebab-Case Pattern

Valid kebab-case file names must:
- Start with a lowercase letter
- Use only lowercase letters and numbers
- Separate words with hyphens (-)
- Not contain consecutive hyphens
- End with `.cs` extension

### Valid Examples
- `user-service.cs`
- `data-processor.cs`
- `my-component.cs`
- `api-client-v2.cs`

### Invalid Examples
- `UserService.cs` (PascalCase)
- `userService.cs` (camelCase)
- `user_service.cs` (snake_case)
- `user--service.cs` (consecutive hyphens)
- `-user-service.cs` (starts with hyphen)

## Configuration

### Enabling the Analyzer

The analyzer is disabled by default to avoid breaking existing projects. To enable it, add to your `.editorconfig`:

```ini
# Enable with warning severity
dotnet_diagnostic.TWA001.severity = warning

# Or as an error to fail builds
dotnet_diagnostic.TWA001.severity = error

# Or as a suggestion for gentle nudging
dotnet_diagnostic.TWA001.severity = suggestion
```

### Exception Patterns

Configure files that should be excluded from the rule:

```ini
dotnet_diagnostic.TWA001.excluded_files = *.g.cs;*.Generated.cs;GlobalUsings.cs;Program.cs;Startup.cs
```

Default exceptions include:
- Generated files (`*.g.cs`, `*.Generated.cs`, `*.designer.cs`)
- Razor component code-behind files (`*.razor.cs`) - must match their `.razor` file names
- Build files (`Directory.Build.props`, `Directory.Build.targets`, `Directory.Packages.props`)
- Assembly metadata (`AssemblyInfo.cs`)
- Analyzer tracking files (`AnalyzerReleases.Shipped.md`, `AnalyzerReleases.Unshipped.md`)

## Implementation Details

The analyzer is implemented as an `IIncrementalGenerator` for optimal performance:
- Only re-analyzes changed files
- Efficient caching of analysis results
- Minimal impact on build times

## Use Cases

This analyzer is useful for:
- Teams transitioning from other languages (e.g., JavaScript/TypeScript) that use kebab-case
- Projects that want to align file naming with URL patterns
- Maintaining consistency with kebab-case component naming in frameworks

## Examples

### Project-Wide Configuration

In your root `.editorconfig`:

```ini
[*.cs]
# Enable kebab-case file naming for all C# files
dotnet_diagnostic.TWA001.severity = warning

# But allow exceptions for specific patterns
dotnet_diagnostic.TWA001.excluded_files = *.g.cs;*.Generated.cs;*.designer.cs;Program.cs;Startup.cs;GlobalUsings.cs;AssemblyInfo.cs
```

### Gradual Adoption

For existing projects, start with suggestion severity:

```ini
[*.cs]
dotnet_diagnostic.TWA001.severity = suggestion

# Only enforce in new feature folders
[Features/**.cs]
dotnet_diagnostic.TWA001.severity = warning
```

## Suppressing Violations

For specific files that need exceptions beyond the configured patterns:

```csharp
#pragma warning disable TWA001 // File name should use kebab-case
// File: LegacyAPIClient.cs
#pragma warning restore TWA001
```

Or in the project file:

```xml
<ItemGroup>
  <Compile Update="LegacyAPIClient.cs">
    <AnalyzerSuppression>TWA001</AnalyzerSuppression>
  </Compile>
</ItemGroup>
```