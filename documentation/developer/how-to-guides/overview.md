# How-to Guides

## Overview

Step-by-step guides for common tasks with TimeWarp Source Generators.

## Analyzer Configuration

### [Configure File Name Analyzer](./configure-file-name-analyzer.md)
Learn how to enable and configure the kebab-case file name analyzer for your project.

### [Configure Global Usings Analyzer](./configure-global-usings-analyzer.md)
Learn how to enable TW0007 so file-level usings move to kebab-case `global-usings.cs` (covers namespace-first file-scoped usings).

### Generic TWA rules

TWA0004, TWA0023, and TWA0038 moved here from `TimeWarp.Architecture.Analyzers` with the same ids. See the [analyzer reference](../reference/analyzers/overview.md) and the [1.0.0-beta.13 notes](../../releases.md). Keep the existing `dotnet_diagnostic.TWA0004.severity`, `dotnet_diagnostic.TWA0023.severity`, and `dotnet_diagnostic.TWA0038.severity` keys. Set `dotnet_diagnostic.TWA0038.max_lines` when 500 is the wrong limit. Drop `TimeWarp.Architecture.Analyzers` only after this package version is published and the consumer no longer needs an Architecture-specific `TWA*` rule. Two analyzers with the same id in one compilation report each hit twice.

## Coming Soon

- Create custom analyzer rules
- Integrate with CI/CD pipelines
- Migrate from PascalCase to kebab-case