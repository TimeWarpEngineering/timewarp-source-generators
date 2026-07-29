# Release Notes

## 1.0.0-beta.9

### TW0001 (FileNameRuleAnalyzer)

- Accept **multi-dot** kebab-case `.cs` basenames: every segment between dots must be kebab-case
  (e.g. `application-state.close-modal.cs`).
- Single-stem kebab and default exceptions (`*.razor.cs`, `*.g.cs`, …) unchanged.

### Consumers

- **timewarp-architecture** / TimeWarp.State template consumers can enable
  `dotnet_diagnostic.TW0001.severity = warning|error` after pinning to **≥ 1.0.0-beta.9**
  without false positives on multi-dot state/action partials.
