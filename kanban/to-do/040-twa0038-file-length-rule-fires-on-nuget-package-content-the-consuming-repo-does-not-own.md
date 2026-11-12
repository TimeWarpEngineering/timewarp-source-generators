# TWA0038 file-length rule fires on NuGet package content the consuming repo does not own

## Description

Since 1.0.0-beta.14, the TWA0038 file-length analyzer (`source/timewarp-source-generators/file-length-analyzer.cs`) reports every syntax tree in the compilation that exceeds the line limit. That includes `.cs` files a NuGet package contributes as `contentFiles` or via a `Compile` item in its build props. In timewarp-jaribu, two files shipped inside TimeWarp.Nuru.DevCli exceed 500 lines and are compiled into `tools/dev-cli`; the repo cannot shorten them, so it had to hold SourceGenerators at beta.11 (jaribu tasks 061 and 062). Every consumer of Nuru.DevCli will hit the same wall.

Fix: TWA0038 should only report files the consuming repository owns.

- Skip a tree when its path is under the NuGet packages folder (`NuGetPackageRoot`, `~/.nuget/packages`, any `packages/` restore root) or under `obj/`, or when the file is a package `contentFiles` item. Use the compilation's analyzer-config / global options (`build_property.NuGetPackageRoot`, `build_property.ProjectDir`) rather than hard-coded paths; a tree outside `ProjectDir` and its repository root is not owned.
- Keep generated-file skipping as is.
- Add an `.editorconfig` escape, `dotnet_diagnostic.TWA0038.exclude_paths` (glob list), for the cases the heuristic misses.
- Tests: a file under the package root is not reported; a long file under `ProjectDir` still is; the exclude glob works; the existing `max_lines` override still works.
- Document in the analyzer's readme section and release notes.

## Checklist

- [ ] Ownership check: skip trees under NuGetPackageRoot, `obj/`, or outside ProjectDir/repo root
- [ ] `dotnet_diagnostic.TWA0038.exclude_paths` glob option
- [ ] Analyzer tests for package-root skip, owned-file report, exclude glob, max_lines
- [ ] Readme / release notes updated
- [ ] Verify against timewarp-jaribu `tools/dev-cli` with TimeWarp.Nuru.DevCli 4.0.0-beta.12: zero TWA0038 after the fix
- [ ] Ship as 1.0.0-beta.15 (release task separate if needed)

## Notes

- Origin: timewarp-jaribu task 061 review (2026-11-12) rejected `NoWarn` and obj-compile workarounds; jaribu 062 holds the pin pending this fix.
- No time estimates.

## Session

- Created: ce5c4952-6364-4ae8-824b-a0243ec9ccbe (2026-11-12)
