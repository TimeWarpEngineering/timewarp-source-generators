# Round 1 — merged findings
**Date:** 2026-11-17
**Sources:** general

## Counts

| Severity | open | fixed | wontfix |
|----------|------|-------|---------|
| bug | 1 | 0 | 0 |
| suggestion | 1 | 0 | 0 |
| nit | 0 | 0 | 0 |

## Issues

### M1 — Severity: bug — Status: open
- File: source/timewarp-source-generators/source-file-ownership.cs:146
- Description: `IsOutsideRoots` treats a file as unowned when it sits outside every non-null root. `ProjectDir` alone is enough. The new props make `ProjectDir` compiler-visible, and the SDK always defines it. `RepositoryRoot` is only set when the consumer's `Directory.Build.props` defines it. A rooted file outside the project directory, such as a linked `..\..\source\shared\Long.cs`, then produces no TWA0038 even though the repository owns it. The task says a tree is not owned when it is outside `ProjectDir` and the repository root. `documentation/releases.md:7-8` says "outside both" and that the outside-roots rule is off when a key is absent. `readme.md:70` says "outside both". The design comments in `source-file-ownership.cs:12` and `file-length-analyzer.cs:13` and the test at `tests/timewarp-source-generators-analyzer-tests/file-length-analyzer-tests.cs:188` follow the restatement's "and/or" wording instead. `OutsideRootsFile_Should_BeClean` expects `/opt/vendor/widget.cs` to be clean when only `ProjectDir` is set.
- Suggestion: Apply the outside-roots skip only when both `ProjectDir` and `RepositoryRoot` are known, and only when the rooted path is outside both. If either key is absent, leave the rule off so package content still depends on the NuGet package root (and the `/.nuget/packages/` fallback). Change the ProjectDir-only assertion so that file still warns. Update the two design comments so they match the readme and release notes.
- Source: general
- Disposition notes:

### M2 — Severity: suggestion — Status: open
- File: documentation/developer/reference/analyzers/file-length-analyzer.md:24
- Description: The reference "Not reported" list still names only generated code and paths under `obj/` or `goldens/`. It does not mention the NuGet package root, files outside the project and repository, or `dotnet_diagnostic.TWA0038.exclude_paths`. The TWA0038 blurb in `documentation/developer/reference/analyzers/overview.md:51` only mentions `max_lines`. The package readme and `documentation/releases.md` were updated; this page is the rule's full write-up and is now incomplete.
- Suggestion: Extend "Not reported" and the overview blurb to match the readme: package root, outside both project directory and `$(RepositoryRoot)`, and the `exclude_paths` glob key.
- Source: general
- Disposition notes:

## Duplicates / conflicts

- Single reviewer. No overlaps.
