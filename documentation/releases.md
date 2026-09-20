# Release Notes

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
