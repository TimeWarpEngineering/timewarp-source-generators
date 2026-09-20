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

- [ ] TW0007 + tests for namespace-first
- [ ] EditorConfig filename default `global-usings.cs`
- [ ] AnalyzerReleases.Unshipped.md
- [ ] README / skill pointer if SourceGenerators docs list TW ids
- [ ] Did not add BDSoftware package

## Out of scope

- Ganda audit check rewrite (Ganda **284** / PR #162 — retarget after this NuGet ships)
- Sweeping TimeWarp.State file usings (State **089**)
- Code-fix provider (follow-up OK)

## Notes

Cockpit 2026-09-20: decompiled 1.4.0 `AnalyzeUsings` → `compilationUnitRoot.Usings` only. User chose write-our-own over keeping theirs.

Ganda 284 currently audits the NuGet package. After TW0007 ships, 284 must check TimeWarp.SourceGenerators + TW0007 config, not GlobalUsingsAnalyzer.

## Session

- Created: cockpit grok 2026-09-20 (plan 1 after 284 PR #162)
