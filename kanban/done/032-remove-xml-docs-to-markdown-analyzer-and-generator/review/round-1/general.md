# Round 1 — general
**Date:** 2026-11-09
**Scope reviewed:** branch vs merge-base `5e02ad4` (product commit `b1d71df`): 34 files, +122 / −1633

## Summary

The change deletes the TW0002 analyzer, both TW0003 generator files, the code-fix setup doc, the XMLDoc template pages, and the markdown-only test fixtures. Release tracking, live docs, and package tags now list only TW0001 and TW0007. The package id is unchanged. Every requirement in task.md is met, and the implementer restatement matches the task (no misread).

Verified on this worktree: `./bin/dev build` succeeded with 0 warnings and 0 errors and packed `TimeWarp.SourceGenerators.1.0.0-beta.11.nupkg`. `./bin/dev test` printed the four console smoke lines and reported Passed 11, Failed 0. `rg "XmlDocsToMarkdown|MarkdownDocsGenerator|TW0002|TW0003|xmldoc-template|enabling-code-fix"` over everything outside `kanban/` has no hits. Only the `spikes/` folder still describes the old generator, and it is not live docs. Task 016 is archived at the merge-base. Task 013 is archived on master (`6d2a467`), and this branch does not touch that file, so the merge is clean. The archived 016 draft `.cs` is not in any project.

Risk is low. The remaining issues are wording in the rewritten docs, plus a missing in-repo record that TW0002 and TW0003 are retired.

## Issues

### Issue 1 — Severity: suggestion
- File: documentation/overview.md:20 (also documentation/developer/reference/overview.md:16, documentation/developer/reference/source-generators/overview.md:3, documentation/developer/conceptual/overview.md:14-15)
- Description: The new text says "This package does not ship a source generator." That is not accurate. TW0001 `FileNameRuleAnalyzer` is still a `[Generator]` `IIncrementalGenerator` (source/timewarp-source-generators/file-name-rule-analyzer.cs:3-4). It uses `RegisterSourceOutput` only to report diagnostics. An IDE lists it under the package's source generators, so this sentence contradicts what users see. The conceptual overview also dropped the accurate "use `IIncrementalGenerator`" note and now says only "ships Roslyn diagnostic analyzers".
- Suggestion: Say that the package generates no source code. For example: "This package generates no source. TW0001 runs as an incremental generator that only reports diagnostics, and TW0007 is a diagnostic analyzer. XML documentation stays in the C# file." Use the same sentence in all four places.
- Status: open

### Issue 2 — Severity: suggestion
- File: documentation/developer/conceptual/analyzer-release-tracking.md:72-76 (and source/timewarp-source-generators/AnalyzerReleases.Unshipped.md)
- Description: The task says TW0002 and TW0003 "stay retired. Do not reuse them." The rows were removed, as required. Now nothing in the product tree records that those ids are retired. A later contributor who sees TW0001 followed by TW0007 can take TW0002 for a new rule. The only record will be under `kanban/done/`.
- Suggestion: Add a short "Retired ids" note to analyzer-release-tracking.md: "TW0002 (XmlDocsToMarkdownAnalyzer) and TW0003 (MarkdownDocsGenerator) were removed unshipped in task 032. Do not reuse these ids." Leave `AnalyzerReleases.Unshipped.md` as a pure table (RS2008 tooling), or add a comment line only if the release-tracking analyzer accepts it.
- Status: open

### Issue 3 — Severity: nit
- File: documentation/developer/conceptual/partial-properties-field-keyword.md:35
- Description: The doc says "For a non-nullable reference type, `field ?? \"\"` satisfies definite assignment." That is the wrong concept. The backing field is always definitely assigned, because it starts at its default value. The `?? ""` getter is about nullable analysis. It makes the getter null-resilient, so the compiler does not warn that a non-nullable property is left null (CS8618).
- Suggestion: Reword it to "For a non-nullable reference type, a getter of `field ?? \"\"` never returns null, so nullable analysis does not warn about an uninitialized property."
- Status: open
