# Release Notes

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
