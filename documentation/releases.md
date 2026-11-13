# Release Notes

## 1.0.0-beta.15

**Breaking:** the package now targets `net11.0`. The analyzer DLL under `analyzers/dotnet/cs` is built for `net11.0`, not `netstandard2.0`, and there is no `netstandard2.0` fallback.

- The analyzers need the .NET 11 SDK compiler (`dotnet build` on the .NET 11 SDK, or an editor that hosts that compiler).
- The package does not load in Visual Studio, whose compiler runs on .NET Framework.
- The repository build uses `TimeWarp.Build.Tasks` 2.0.0.

## 1.0.0-beta.14

The repository builds and tests on the .NET 11 GA SDK (`net11.0`). The shipped package still targets `netstandard2.0`.

## 1.0.0-beta.13

Generic style rules moved from `TimeWarp.Architecture.Analyzers`, with the same diagnostic ids. `.editorconfig` keys do not change. Architecture-specific `TWA*` rules stay in that package.

- **TWA0004** `PurposeRegionAnalyzer`. Warning, enabled. Every non-generated C# file needs a `#region Purpose` block.
- **TWA0023** `TypeStemIdentifierAnalyzer`. Warning, disabled. An identifier ends with its type stem (interfaces drop a leading `I`). Opt-out is `[TypeStemIdentifier("reason")]`, matched by simple name. The attribute type stays in `TimeWarp.Architecture.Attributes`.
- **TWA0038** `FileLengthAnalyzer`. Warning, enabled. Default limit is 500 lines. Override with `dotnet_diagnostic.TWA0038.max_lines`.

TWA0004 and TWA0038 are on by default, as they were in Architecture.Analyzers. A repo that treats warnings as errors and does not want the file-length rule sets `dotnet_diagnostic.TWA0038.severity` to `none`. `timewarp-state` task 153 drops `TimeWarp.Architecture.Analyzers` after this package is published and keeps `dotnet_diagnostic.TWA0004.severity = error`.

```ini
[*.cs]
dotnet_diagnostic.TWA0004.severity = warning
dotnet_diagnostic.TWA0023.severity = warning
dotnet_diagnostic.TWA0038.severity = warning
dotnet_diagnostic.TWA0038.max_lines = 500
```

NuGet publish of `1.0.0-beta.13` is `dev release` from a clean master after this change merges. Until that publish, Architecture.Analyzers still reports these three ids.

## 1.0.0-beta.12

The package ships analyzers only. It generates no source.

- Removed the interface delegation generator, `[Implements]`, and diagnostics **TW0004**, **TW0005**, and **TW0006**. Those ids stay retired.
- Removed **TW0002** `XmlDocsToMarkdownAnalyzer` and **TW0003** `MarkdownDocsGenerator`. XML documentation stays in the C# file. Those ids stay retired.
- Live diagnostics are **TW0001** and **TW0007**.

## 1.0.0-beta.11

### TW0007 (GlobalUsingsRuleAnalyzer)

- New analyzer: file-level `using` directives must move to kebab **`global-usings.cs`**.
- Visits every `UsingDirectiveSyntax` (compilation unit **and** file-scoped / block namespace), so namespace-first TimeWarp files are covered.
- Skips `global using`, `using static`, aliases, generated/build-output paths, the configured global-usings file, and `excluded_files`.
- Disabled by default (`isEnabledByDefault: false`). Enable with `dotnet_diagnostic.TW0007.severity = warning|error`.
- Replaces third-party GlobalUsingsAnalyzer (BDSoftware); do not add that package.

### Consumers

- Pin **≥ 1.0.0-beta.11** and set `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.TW0007.severity = warning
dotnet_diagnostic.TW0007.filename = global-usings.cs
```

- Ganda audit check rewrite (Ganda 284) should retarget to this package + TW0007 after this NuGet ships.

## 1.0.0-beta.10

### TW0001 (FileNameRuleAnalyzer)

- **Skip build output and generated paths** under `obj/`, `bin/`, and `artifacts/generated/`
  (gRPC `Greet.cs` stubs, SDK `EmbeddedAttribute.cs`, etc.). These are not authored product
  basenames; without this skip, enabling TW0001 fails every project that compiles intermediates.

### Consumers

- Prefer **≥ 1.0.0-beta.10** before turning `dotnet_diagnostic.TW0001.severity` to warning/error
  in a solution with protobuf, Razor, or SDK-generated trees.

## 1.0.0-beta.9

### TW0001 (FileNameRuleAnalyzer)

- Accept **multi-dot** kebab-case `.cs` basenames: every segment between dots must be kebab-case
  (e.g. `application-state.close-modal.cs`).
- Single-stem kebab and default exceptions (`*.razor.cs`, `*.g.cs`, …) unchanged.

### Consumers

- **timewarp-architecture** / TimeWarp.State template consumers can enable
  `dotnet_diagnostic.TW0001.severity = warning|error` after pinning to **≥ 1.0.0-beta.9**
  without false positives on multi-dot state/action partials (still need **beta.10** for obj/bin skip).
