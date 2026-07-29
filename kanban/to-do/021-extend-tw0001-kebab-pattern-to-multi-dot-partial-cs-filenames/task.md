# Extend TW0001 kebab pattern to multi-dot partial cs filenames

## Description

`FileNameRuleAnalyzer` (**TW0001**) currently matches only a **single** kebab stem:

```text
^[a-z][a-z0-9]*(?:-[a-z0-9]+)*\.cs$
```

That rejects legitimate **multi-dot partial** basenames used heavily by TimeWarp templates
(TimeWarp.State actions), e.g.:

```text
application-state.cs                 ✓ already
application-state.close-modal.cs     ✗ fails today
counter-state.increment-counter.cs   ✗ fails today
```

Each **segment** is already kebab-case; extra dots mark secondary partial files for one type.
Architecture’s partial-class analyzer (TWA0001) already accepts this shape. TW0001 should
**not** false-fail it.

Until this ships, consumer monorepos (e.g. timewarp-architecture task **133**) **cannot enable**
`dotnet_diagnostic.TW0001.severity = warning|error` without mass false positives (~40+ SPA
state partials alone).

### In scope

- Update `KebabCasePattern` (or equivalent validation) so multi-dot basenames pass when **every**
  segment is kebab:  
  `^[a-z][a-z0-9]*(?:-[a-z0-9]+)*(?:\.[a-z][a-z0-9]*(?:-[a-z0-9]+)*)*\.cs$`  
  (or equivalent; exact regex is implementer’s choice if tests lock behavior).
- Keep rejecting PascalCase / snake_case / mixed invalid stems (`UserService.cs`,
  `application_state.cs`, `Application-state.close-modal.cs`).
- Preserve existing default exceptions (`*.razor.cs`, `*.g.cs`, …).
- Tests: valid multi-dot kebab; invalid multi-dot (one Pascal segment); still valid single-stem.
- Docs: `file-name-rule-analyzer.md`, how-to-configure, overview — examples of multi-dot partials.
- Bump package version + release notes so architecture can pin and enable TW0001.

### Out of scope

- Renaming Architecture product files
- Folder / non-`.cs` checks (Ganda task **188** `kebab-path-names`)
- Changing diagnostic id (**TW0001** stays; task **020** locked `TW*`)

## Requirements

- Multi-dot kebab `.cs` basenames are TW0001-clean.
- No regression on single-stem kebab or default exceptions.
- Documented; package published or at least version bumped for consumers to reference.
- Call out in release notes: architecture/template consumers can enable TW0001 after upgrade.

## Checklist

- [ ] Implement multi-dot kebab validation in `file-name-rule-analyzer.cs`
- [ ] Unit / console tests for pass and fail cases
- [ ] Update analyzer docs + configure how-to examples
- [ ] AnalyzerReleases if required by Roslyn release tracking
- [ ] Version bump + release notes
- [ ] Note consumers: timewarp-architecture can enable `dotnet_diagnostic.TW0001` after pin bump

## Notes

### Origin

- timewarp-architecture task **133** (kebab gaps): remediations done; **TW0001 enable blocked**
  on this pattern gap.
- Pattern SSOT today: `source/timewarp-source-generators/file-name-rule-analyzer.cs`
- Related: task **020** (TW* prefix SSOT — done, no id rename)

### Consumer example (must pass after fix)

```
application-state.close-modal.cs
weather-forecasts-state.fetch-weather-forecasts.cs
```

### Consumer example (must still fail)

```
ApplicationState.CloseModal.cs
application-state.CloseModal.cs
application_state.close_modal.cs
```

## Session

- Created: 2026-07-29 — follow-up after architecture task 133 deferred TW0001 enable
