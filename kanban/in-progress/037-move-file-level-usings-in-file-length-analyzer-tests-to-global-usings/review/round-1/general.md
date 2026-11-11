# Round 1 — general
**Date:** 2026-11-11
**Scope reviewed:** branch `task/037-move-file-level-usings-in-file-length-analyzer-tes` (`4e63dc7`) vs `master`, product paths: `tests/timewarp-source-generators-analyzer-tests/file-length-analyzer-tests.cs`, `tests/timewarp-source-generators-analyzer-tests/global-usings.cs`, `spikes/partial-properties-csharp13/partial-property-test/Program.cs`, `spikes/partial-properties-csharp13/partial-property-test/global-usings.cs`

## Summary

Commit `4e63dc7` removes the three file-level `using` directives (`System.Globalization`, `System.Text`, `Microsoft.CodeAnalysis.Text`) from `tests/timewarp-source-generators-analyzer-tests/file-length-analyzer-tests.cs` and inserts them as `global using` directives in alphabetical order within `tests/timewarp-source-generators-analyzer-tests/global-usings.cs`. It also takes the trivial optional requirement in `spikes/partial-properties-csharp13/partial-property-test/`, moving `using TimeWarp.Terminal;` from `Program.cs` into a new kebab-case `global-usings.cs` with a `#region Purpose` header. The implementer restatement matches the original task with no misread, out-of-scope boundaries (`.githooks/*.cs`, `using static`, snippet strings, `.editorconfig` TW0007) are untouched, and risk is negligible.

## Verification (re-run by reviewer)

| Gate | Result |
|------|--------|
| `RestoreSources=/home/steve/.nuget/packages ./bin/dev build` | Exit 0, 0 warnings, 0 errors; generator targets `netstandard2.0`; both test consoles build for `net11.0` |
| `RestoreSources=/home/steve/.nuget/packages ./bin/dev test` | Exit 0; `test-console` runs cleanly; `analyzer-tests` prints `Passed: 56  Failed: 0  Total: 56` |
| `RestoreSources=/home/steve/.nuget/packages dotnet run --project spikes/partial-properties-csharp13/partial-property-test/partial-property-test.csproj -c Release` | Exit 0; prints `test` |
| `ganda repo audit` | All 42 local checks PASS (`analyzer-suppressions`, `kebab-path-names`, `memsearch-git-hooks`, `source-file-length`, `editorconfig`, `editorconfig-charset`, etc.); the sole failure in the sandboxed review shell is `self-hosted-runner` (`gh repo view --json isPrivate` blocked by offline sandbox proxy) |
| Repo-wide `using` scan (`*.cs`) | Zero non-static file-level `using` directives remain outside `.githooks/*.cs` and test snippet string literals |

## Checks against the task

- `tests/timewarp-source-generators-analyzer-tests/global-usings.cs` keeps its `System`-first alphabetical ordering (`System.Globalization` between `System.Collections.Immutable` and `System.Linq`; `System.Text` between `System.Net.Http` and `System.Threading.Tasks`; `Microsoft.CodeAnalysis.Text` between `Microsoft.CodeAnalysis.Diagnostics` and `TimeWarp.SourceGenerators`).
- `tests/timewarp-source-generators-analyzer-tests/file-length-analyzer-tests.cs` has no file-level `using` directives, and its `#region Design` line-span notes refer to the synthetic `SourceOfLines` fixture rather than physical lines of the test file itself.
- None of the other nine `.cs` files in `tests/timewarp-source-generators-analyzer-tests/` suffer a symbol ambiguity under `TreatWarningsAsErrors=true`.
- `spikes/partial-properties-csharp13/partial-property-test/global-usings.cs` uses kebab-case naming and carries a one-line `#region Purpose` block per `tw-agent-context-regions`.
- Out-of-scope files (`.githooks/*.cs`, `using static` imports in the two type-stem test files, snippet strings, and `.editorconfig`) are unchanged.

## Issues

None.
