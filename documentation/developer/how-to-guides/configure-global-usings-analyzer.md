# How to Configure the Global Usings Analyzer

This guide walks through enabling **TW0007**, which requires file-level `using` directives to live in kebab-case `global-usings.cs`.

> **ID reminder:** The rule ID is **TW0007** (`dotnet_diagnostic.TW0007.*`). Do **not** use Architecture IDs such as `TWA*` or the old BDSoftware `globalusingsanalyzer0003` keys.

## Prerequisites

- TimeWarp.SourceGenerators **≥ 1.0.0-beta.11**
- An `.editorconfig` file in your project or repo root

## Step 1: Enable the Analyzer

```ini
[*.cs]
dotnet_diagnostic.TW0007.severity = warning
dotnet_diagnostic.TW0007.filename = global-usings.cs
```

Severity options:

- `none` — disabled (default)
- `suggestion` — IDE info
- `warning` — warning
- `error` — fails the build

## Step 2: Choose the global-usings filename

TimeWarp default is kebab **`global-usings.cs`**. If a repo still uses PascalCase:

```ini
dotnet_diagnostic.TW0007.filename = GlobalUsings.cs
```

The diagnostic message names this file: `Move 'System.Text' to 'global-usings.cs'`.

## Step 3: Exclude files if needed

```ini
dotnet_diagnostic.TW0007.excluded_files = Program.cs;legacy-usings.cs
```

`global using`, `using static`, aliases, generated files, and the configured global-usings file are skipped without extra config.

## Step 4: Move reported usings

For each TW0007:

1. Add `global using The.Namespace;` to `global-usings.cs`
2. Delete the file-level `using The.Namespace;`

Namespace-first file-scoped files are in scope (this is the gap in GlobalUsingsAnalyzer 1.4.0):

```csharp
namespace Sample;
using System.Text; // TW0007
```

## Troubleshooting

### Analyzer not running

- Pin TimeWarp.SourceGenerators **≥ 1.0.0-beta.11**
- Confirm `dotnet_diagnostic.TW0007.severity` is not `none`
- Clean and rebuild

### Still seeing BDSoftware 0003 only

TW0007 replaces GlobalUsingsAnalyzer. Remove the BDSoftware package and its `dotnet_diagnostic.globalusingsanalyzer000*` keys. Do not add `GlobalUsingsAnalyzer` from BDSoftware.
