# TWA0038 file-length rule fires on NuGet package content the consuming repo does not own

## Description

Since 1.0.0-beta.14, the TWA0038 file-length analyzer (`source/timewarp-source-generators/file-length-analyzer.cs`) reports every syntax tree in the compilation that exceeds the line limit. That includes `.cs` files a NuGet package contributes as `contentFiles` or via a `Compile` item in its build props. In timewarp-jaribu, two files shipped inside TimeWarp.Nuru.DevCli exceed 500 lines and are compiled into `tools/dev-cli`; the repo cannot shorten them, so it had to hold SourceGenerators at beta.11 (jaribu tasks 061 and 062). Every consumer of Nuru.DevCli will hit the same wall.

Fix: TWA0038 should only report files the consuming repository owns.

- Skip a tree when its path is under the NuGet packages folder (`NuGetPackageRoot`, `~/.nuget/packages`, any `packages/` restore root) or under `obj/`, or when the file is a package `contentFiles` item. Use the compilation's analyzer-config / global options (`build_property.NuGetPackageRoot`, `build_property.ProjectDir`) rather than hard-coded paths; a tree outside `ProjectDir` and its repository root is not owned.
- Keep generated-file skipping as is.
- Add an `.editorconfig` escape, `dotnet_diagnostic.TWA0038.exclude_paths` (glob list), for the cases the heuristic misses.
- Tests: a file under the package root is not reported; a long file under `ProjectDir` still is; the exclude glob works; the existing `max_lines` override still works.
- Document in the analyzer's readme section and release notes.

## Goal (implementer restatement)

TWA0038 must stop warning about `.cs` files that a NuGet package compiles into the consumer (for example the TimeWarp.Nuru.DevCli `build/*.props` `Compile` items under `~/.nuget/packages/...`). The consumer cannot edit those files, so a warning there is noise that forced jaribu to pin an old analyzer.

Acceptance:

- A tree under the NuGet package root is not reported. The root comes from `build_property.NuGetPackageRoot`, which the package now makes visible through `build/` and `buildTransitive/` props. When that key is absent, a `/.nuget/packages/` path segment is the fallback.
- `obj/` and `goldens/` trees stay exempt. Generated-code skipping is unchanged.
- When `build_property.ProjectDir` and/or `build_property.RepositoryRoot` are known, a rooted tree under neither of them is not owned and is not reported.
- `.editorconfig` key `dotnet_diagnostic.TWA0038.exclude_paths` takes a `;`/`,` glob list (`*`, `**`, `?`). A matching tree is not reported.
- A long file under `ProjectDir` still warns. `max_lines` still overrides the limit.
- Tests cover each of those cases. The readme TWA0038 section, `documentation/releases.md` (beta.16), and the analyzer release notes describe the change.
- Verified against timewarp-jaribu `tools/dev-cli`: zero TWA0038 with the locally built package.

## Checklist

- [x] Ownership check: skip trees under NuGetPackageRoot, `obj/`, or outside ProjectDir/repo root
- [x] `dotnet_diagnostic.TWA0038.exclude_paths` glob option
- [x] Analyzer tests for package-root skip, owned-file report, exclude glob, max_lines
- [x] Readme / release notes updated
- [x] Verify against timewarp-jaribu `tools/dev-cli` with the TimeWarp.Nuru.DevCli version jaribu pins at the time (this repo pins 4.0.0-beta.15): zero TWA0038 after the fix
- [ ] Ship as 1.0.0-beta.16 (release task separate if needed)

## Notes

- Origin: timewarp-jaribu task 061 review (2026-11-12) rejected `NoWarn` and obj-compile workarounds; jaribu 062 holds the pin pending this fix.
- No time estimates.
- Groomed 2026-11-17: 1.0.0-beta.15 was released 2026-11-13 (tasks 039 and 041), and master is on unreleased `1.0.0-beta.16`, so this ships as beta.16. Bump `<Version>` only if `./bin/dev check-version` requires it.
- `build_property.NuGetPackageRoot` is not visible to analyzers by default. A package makes it visible by shipping a build props or targets file with `<CompilerVisibleProperty Include="NuGetPackageRoot" />`. This package ships no build file today (the csproj packs only the DLL and the readme). Either add one under `build/` (and `buildTransitive/` if consumers reference it transitively) or do not depend on that property. Check which `build_property.*` keys the analyzer actually receives in a consumer before relying on any of them, `ProjectDir` included, and keep a path-based fallback for when a key is absent.
- The package targets `net11.0` since beta.15 (task 041), so current BCL APIs are available to the analyzer; `EnforceExtendedAnalyzerRules` still bans file IO.

## Session

- Created: ce5c4952-6364-4ae8-824b-a0243ec9ccbe (2026-11-12)
- 2026-11-17 implement (ganda task work, Claude Opus 5.5): ownership rules, exclude_paths, build props, tests, docs, jaribu verification. Audit boy-scout: `ganda repo audit --fix` created the gitignored `.vscode/settings.json` for the advisory `vscode-window-icon` warning; nothing tracked changed.
- Ship item: `<Version>` is already `1.0.0-beta.16` (unreleased), so no bump. The NuGet publish is a separate `dev release` after merge.

## Results

### Changes

- `source/timewarp-source-generators/source-file-ownership.cs` (new): `SourceFileOwnership` decides whether the consumer owns a tree. It skips files under `build_property.NuGetPackageRoot` (fallback when that key is absent: a `/.nuget/packages/` segment), files under `obj/`/`goldens/`, and rooted files outside both `build_property.ProjectDir` and `build_property.RepositoryRoot` (only when at least one of those keys is known). It collapses `.`/`..` lexically, because DevCli's Compile items arrive as `.../build/../content/...`. It also does the `exclude_paths` glob matching (`**`, `*`, `?`; a glob matches the path suffix at a segment boundary, and a leading `/` anchors it).
- `file-length-analyzer.cs`: uses the ownership check and the new `ExcludePathsOptionKey` (`dotnet_diagnostic.TWA0038.exclude_paths`). Generated-code skipping and `max_lines` are unchanged. Removed the unused `IsExemptPath`; its obj/goldens rule moved to `SourceFileOwnership.IsBuildOutput`.
- `build/TimeWarp.SourceGenerators.props` (new, packed to `build/` and `buildTransitive/`): `CompilerVisibleProperty` for `NuGetPackageRoot`, `ProjectDir`, and `RepositoryRoot`.
- Tests: 8 new TWA0038 cases (package root with `..`, `.nuget/packages` fallback, owned ProjectDir file warns, RepositoryRoot-linked file warns, outside-roots skip, exclude glob, max_lines with roots, glob/normalize helpers).
- Docs: readme TWA0038 paragraph, `documentation/releases.md` 1.0.0-beta.16, `AnalyzerReleases.Unshipped.md`.

### Gates

| Gate | Command | Result |
|------|---------|--------|
| Build | `./bin/dev build` | Pass. 0 warnings, 0 errors. Package has `build/` + `buildTransitive/` props. |
| Test | `./bin/dev test` | Pass. 64/64. |
| Audit | `ganda repo audit` | First run passed with 1 advisory (`vscode-window-icon`). After `ganda repo audit --fix`, the rerun passed all checks. |

### Jaribu verification

Scratch copy of timewarp-jaribu master at `/tmp/verify-040/jaribu`, with an isolated `NUGET_PACKAGES=/tmp/verify-040/pkgs` (no `.nuget/packages` segment, so the `build_property.NuGetPackageRoot` path was the one exercised). The local feed held this branch packed as `1.0.0-beta.16-verify040`. Build command: `dotnet build tools/dev-cli/dev.cs --no-incremental`.

- DevCli 4.0.0-beta.15 (jaribu's current pin) ships no file over 500 lines, so it is clean with both analyzer versions. To exercise the fix, the copy used DevCli 4.0.0-beta.12, which ships `nuget-version-service.cs` (704 lines) and `release-command.cs` (609 lines).
- Analyzer beta.15: 2 TWA0038 errors on those two package files (the bug).
- Analyzer from this branch: 0 TWA0038. The generated editorconfig shows `build_property.NuGetPackageRoot`, `ProjectDir`, and `RepositoryRoot`.
- A 607-line probe file added under jaribu `tools/dev-cli/endpoints/` still raised TWA0038 with the fixed analyzer.
- The unrelated CS0103/CS0246 errors come from jaribu's endpoints, which target the newer DevCli API and do not compile against beta.12.

### How to validate

Smoke:

```bash
./bin/dev build && ./bin/dev test
unzip -l artifacts/packages/TimeWarp.SourceGenerators.1.0.0-beta.16.nupkg | grep -E "build(Transitive)?/TimeWarp.SourceGenerators.props"
```

Expect: build passes with 0 warnings. Tests are 64/64 and include `PASS file under NuGetPackageRoot is clean`, `PASS long file under ProjectDir still warns`, and `PASS exclude_paths glob skips matching files`. The nupkg lists both `build/` and `buildTransitive/TimeWarp.SourceGenerators.props`. In a consumer that compiles a package's long `.cs` (for example DevCli 4.0.0-beta.12), TWA0038 reports nothing for the package files and still reports owned files over the limit.
