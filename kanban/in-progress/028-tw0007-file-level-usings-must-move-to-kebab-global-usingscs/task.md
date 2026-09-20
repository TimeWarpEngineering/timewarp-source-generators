# TW0007: file-level usings must move to kebab `global-usings.cs`

## Description

Replace third-party **GlobalUsingsAnalyzer 1.4.0** (BDSoftware, last release 2023-02-21). That analyzer only walks `CompilationUnitSyntax.Usings` — usings **before** any namespace. TimeWarp puts **file-scoped `namespace X;` first**, then usings (Allman / `tw-csharp`). Those usings live on `FileScopedNamespaceDeclarationSyntax.Usings` and **never fire 0003**.

Ship **TW0007** in **TimeWarp.SourceGenerators** (same pack as TW0001 kebab files). Next free `TW*` analyzer id (TW0003–0006 are generators).

## Requirements

- New analyzer (not an incremental generator): visit **every** `UsingDirectiveSyntax` in the tree (compilation unit **and** file-scoped / block namespace).
- Skip: `global using`, `using static`, aliases, generated files, and the project global-usings file.
- Default global-usings filename: kebab **`global-usings.cs`** (not `GlobalUsings.cs`).
- EditorConfig (under `[*.cs]`, same pattern as TW0001):
  - `dotnet_diagnostic.TW0007.severity` (default **warning**; enable per repo)
  - `dotnet_diagnostic.TW0007.filename = global-usings.cs`
  - `dotnet_diagnostic.TW0007.excluded_files` if needed
- Diagnostic: move `X` to `{filename}`.
- Tests: namespace-first file-scoped **does** warn; usings before namespace **do** warn; `global using` / `using static` / alias / `global-usings.cs` itself **do not**; PascalCase filename configurable.
- AnalyzerReleases unshipped entry.
- `isEnabledByDefault: false` like TW0001 until repos opt in (or true if you can prove SourceGenerators’ own tree is clean). Prefer **false** so existing consumers do not go red on package bump.
- Code-fix (move using + append to `global-usings.cs`) is **out of scope** unless cheap; diagnostic-only is enough.

Copy shape: `file-name-rule-analyzer.cs` (config options, kebab default). Do not depend on `GlobalUsingsAnalyzer` NuGet.

## Checklist

- [x] TW0007 + tests for namespace-first
- [x] EditorConfig filename default `global-usings.cs`
- [x] AnalyzerReleases.Unshipped.md
- [x] README / skill pointer if SourceGenerators docs list TW ids
- [x] Did not add BDSoftware package

## Out of scope

- Ganda audit check rewrite (Ganda **284** / PR #162 — retarget after this NuGet ships)
- Sweeping TimeWarp.State file usings (State **089**)
- Code-fix provider (follow-up OK)

## Notes

Cockpit 2026-09-20: decompiled 1.4.0 `AnalyzeUsings` → `compilationUnitRoot.Usings` only. User chose write-our-own over keeping theirs.

Ganda 284 currently audits the NuGet package. After TW0007 ships, 284 must check TimeWarp.SourceGenerators + TW0007 config, not GlobalUsingsAnalyzer.

## Session

- Created: cockpit grok 2026-09-20 (plan 1 after 284 PR #162)
- Implementer: grok 2026-09-20 (ganda task-work implement)
- Review oracle: grok 2026-09-20 (ganda task-work review, effort 1 general)

## Results

Shipped **TW0007** (`GlobalUsingsRuleAnalyzer`) in TimeWarp.SourceGenerators **1.0.0-beta.11**. DiagnosticAnalyzer visits every `UsingDirectiveSyntax` (compilation unit, file-scoped namespace-first, and block namespace). Skips `global using`, `using static`, aliases, generated/build-output paths, the configured global-usings file, and `excluded_files`. Default filename is kebab `global-usings.cs`. `isEnabledByDefault: false`; default severity warning when enabled. No BDSoftware / GlobalUsingsAnalyzer package. Code-fix left out of scope.

### Files changed

| Path | Change |
|------|--------|
| `source/timewarp-source-generators/global-usings-rule-analyzer.cs` | New TW0007 analyzer |
| `source/timewarp-source-generators/AnalyzerReleases.Unshipped.md` | TW0007 unshipped row |
| `source/Directory.Build.props` | Version `1.0.0-beta.11` |
| `.editorconfig` | TW0007 keys; removed leftover `globalusingsanalyzer000*` |
| `tests/timewarp-source-generators-analyzer-tests/**` | 11 CompilationWithAnalyzers cases |
| `tests/timewarp-source-generators-test-console/**` | `global-usings.cs`; TW0007=error smoke |
| `tools/dev-cli/endpoints/test-command.cs` | Runs analyzer-tests after test-console |
| `readme.md`, `documentation/**` | TW0007 docs + how-to |
| `source/timewarp-source-generators/interface-delegation-generator.cs` | Dropped redundant file-level using |

### Key decisions

- DiagnosticAnalyzer, not incremental generator, so `FileScopedNamespaceDeclarationSyntax.Usings` are not missed.
- Stay disabled-by-default so existing consumers do not go red on bump.
- Test-console enables TW0007 as **error** after moving its file-level using into `global-usings.cs`.
- No code-fix (brief: out of scope unless cheap).

### Test outcomes

`dotnet run --file tools/dev-cli/dev.cs -- test` — **PASS** (test-console + 11/11 TW0007 cases). No BDSoftware package refs.

### How to validate

**Automated**

```bash
dotnet run --file tools/dev-cli/dev.cs -- test
```

**Expect:** test-console prints kebab/multi-dot/Pascal messages; analyzer-tests prints `Passed: 11  Failed: 0  Total: 11`; `Tests completed successfully!`

**Smoke**

```bash
dotnet run --project tests/timewarp-source-generators-analyzer-tests/timewarp-source-generators-analyzer-tests.csproj -c Release
```

**Expect:** PASS lines for namespace-first warn, usings-before-namespace warn, and no-warn for `global using` / `using static` / alias / `global-usings.cs`; PascalCase filename configurable.

```bash
dotnet pack source/timewarp-source-generators/timewarp-source-generators.csproj -c Release
ls artifacts/packages/TimeWarp.SourceGenerators.1.0.0-beta.11.nupkg
```

**Expect:** pack id `TimeWarp.SourceGenerators` version `1.0.0-beta.11`. `rg -i 'BDSoftware|GlobalUsingsAnalyzer' Directory.Packages.props` is empty.

**Not in scope:** Ganda 284 audit rewrite; TimeWarp.State using sweep; code-fix.

### Review disposition

**Outcome:** clean (0 open findings)
**Effort / roster:** 1 — general only
**Rounds:** 1
**Final counts:** bug 0 / suggestion 0 / nit 0 (all open=0, fixed=0, wontfix=0)

Round 1 general review of `b7a6221` vs `origin/master` raised no issues. Analyzer visits every `UsingDirectiveSyntax` including namespace-first; skips, kebab default, opt-in warning, Unshipped row, docs, and 11/11 tests match the brief. No wontfix; no escalations.

**Review paths**

- `kanban/in-progress/028-tw0007-file-level-usings-must-move-to-kebab-global-usingscs/review/review-framework.md`
- `kanban/in-progress/028-tw0007-file-level-usings-must-move-to-kebab-global-usingscs/review/round-1/general.md`
- `kanban/in-progress/028-tw0007-file-level-usings-must-move-to-kebab-global-usingscs/review/round-1/merged.md`
- `kanban/in-progress/028-tw0007-file-level-usings-must-move-to-kebab-global-usingscs/review/disposition.md`
