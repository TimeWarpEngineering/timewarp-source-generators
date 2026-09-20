# GlobalUsingsRuleAnalyzer (TW0007)

## Overview

The GlobalUsingsRuleAnalyzer reports file-level `using` directives that should be moved to the project global-usings file as `global using` directives. Unlike third-party GlobalUsingsAnalyzer 1.4.0, this rule visits **every** `UsingDirectiveSyntax` in the tree — compilation-unit usings **and** usings on file-scoped or block namespaces — so TimeWarp's namespace-first file-scoped layout is covered.

## Rule Details

- **Rule ID**: TW0007
- **Category**: Usage
- **Default Severity**: Warning (disabled by default)
- **Message**: Move '{0}' to '{1}'

> **Diagnostic ID (SSOT):** This rule is **TW0007** from TimeWarp.SourceGenerators. It is **not** a TimeWarp Architecture analyzer (`TWA*`). Configure with `dotnet_diagnostic.TW0007.*`.

## What is reported

Reported:

- `using System.Text;` before a namespace
- `namespace Sample;` then `using System.Text;` (file-scoped, namespace-first)
- `using` directives inside a block namespace

Not reported:

- `global using …`
- `using static …`
- aliases (`using X = Y;`)
- generated files and `obj/` / `bin/` / `artifacts/generated/` paths
- the configured global-usings file itself (default `global-usings.cs`)
- files listed in `dotnet_diagnostic.TW0007.excluded_files`

## Configuration

The analyzer is disabled by default so existing consumers do not go red on a package bump. Enable it in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.TW0007.severity = warning
dotnet_diagnostic.TW0007.filename = global-usings.cs
```

Severity options: `none` (default), `suggestion`, `warning`, `error`.

### Filename

Default is kebab-case **`global-usings.cs`**. Repos that still use PascalCase can set:

```ini
dotnet_diagnostic.TW0007.filename = GlobalUsings.cs
```

### Exceptions

```ini
dotnet_diagnostic.TW0007.excluded_files = legacy-usings.cs;*.KeepUsings.cs
```

Default exceptions include generated and designer patterns (`*.g.cs`, `*.Generated.cs`, `*.designer.cs`, `*.razor.cs`, assembly-info / GlobalUsings.g.cs).

## Implementation Details

Implemented as a `DiagnosticAnalyzer` (not an incremental generator) so namespace-owned usings are not missed. Code-fix (move + append to the global-usings file) is out of scope.

## Suppressing Violations

```csharp
#pragma warning disable TW0007 // Move using to global-usings.cs
using Rare.Namespace;
#pragma warning restore TW0007
```

Or in `.editorconfig`:

```ini
dotnet_diagnostic.TW0007.severity = none
```
