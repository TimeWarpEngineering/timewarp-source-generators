# Remove XML docs to markdown analyzer and generator

## Goal (implementer restatement)

XML documentation stays in the C# file. A sibling markdown file is the wrong
place for it, so the whole sidecar goes: the TW0002 analyzer that tells people
to move comments out, and the TW0003 generator that turns a sibling `.md` into
generated XML docs. The unfinished code fix (task 016) and the constructor and
event extension (task 013) are not built. Those tasks are archived. The draft
code fix is not compiled.

After this change the package id is still `TimeWarp.SourceGenerators`. It ships
analyzers TW0001 and TW0007. TW0002 and TW0003 are retired and are not reused.
Live docs and package tags stop advertising the sidecar. `kanban/done/**` stays
as history. Architecture and state keep their old TW0002 notes. `./bin/dev build`
and `./bin/dev test` pass. This walk stops after product work and Results.

## Description

Keep XML documentation in the C# file. A separate markdown file is the wrong
place for it once an assistant is reading the source. Steve, 2026-11-09.

That decision retires the whole sidecar pipeline, not only the unfinished
code fix:

- **TW0002** `XmlDocsToMarkdownAnalyzer` is shipped and enabled by default.
  Its message is "XML documentation for '{0}' should be moved to a markdown
  file". Architecture already sets `dotnet_diagnostic.TW0002.severity = none`
  (task 171) because the nag is unwanted.
- Task **016** is the unshipped code fix that would extract those comments
  into markdown and delete them from the source. The draft lives only under
  the kitchen: `kanban/to-do/016-create-xml-docs-to-markdown-code-fix/`.
- **TW0003** `MarkdownDocsGenerator` is the other direction: a `.md` file
  beside the class becomes generated XML docs. That is the separate file
  this decision rejects. Task **013** would extend that generator to
  constructors and events. Do not build it.

No other TimeWarp repo calls `XmlDocsToMarkdownAnalyzer` or
`MarkdownDocsGenerator` except to name the diagnostic or to silence TW0002.
GitHub code search on 2026-11-09 (`--owner TimeWarpEngineering`).

After this removal the package ships analyzers **TW0001** and **TW0007**.
**TW0002** and **TW0003** stay retired. Do not reuse them. The package id
stays `TimeWarp.SourceGenerators`.

## Requirements

- Delete `source/timewarp-source-generators/xml-docs-to-markdown-analyzer.cs`.
- Delete `source/timewarp-source-generators/markdown-docs-generator.cs` and
  `markdown-docs-generator.markdown-to-xml.cs`.
- Remove the **TW0002** and **TW0003** rows from
  `AnalyzerReleases.Unshipped.md` and from
  `documentation/developer/conceptual/analyzer-release-tracking.md`.
- Archive task **016**. Do not compile its draft
  `xml-docs-to-markdown-code-fix-provider.cs`.
- Archive task **013** (`013-update-xmldocs-generation-add-constructor-event-level.md`).
- Delete `docs/enabling-code-fix-providers.md`. It exists only to set up this
  code fix. Task **015** (kebab-case code fix) stays open and is out of scope.
- Drop the markdown sidecars and the types that exist only to feed the
  generator, under `tests/timewarp-source-generators-test-console/`:
  `i-data-processor.cs`, `i-data-processor.md`, `test-class.cs`,
  `test-class.md`, `partial-property-test.cs`, `partial-property-test.md`,
  `kebab-case-test.md`, `pascal-case-test.md`.
  Keep the TW0001 fixtures and the console smoke that uses them:
  `kebab-case-test.cs`, `PascalCaseTest.cs`,
  `application-state.close-modal.cs`,
  `weather-forecasts-state.fetch-weather-forecasts.cs`, `program.cs`,
  `global-usings.cs`.
- Update live docs so they no longer present this pipeline as a feature:
  `readme.md` prefix table, `documentation/overview.md`,
  `documentation/developer/reference/analyzers/overview.md`,
  `documentation/developer/reference/source-generators/overview.md`,
  `documentation/developer/conceptual/partial-properties-field-keyword.md`.
  Live ids are **TW0001** and **TW0007**.
- In `source/Directory.Build.props` `PackageTags`, drop `Source Generator`
  and `SourceGenerators`. Leave `TimeWarp; Analyzers`. Do not rename the
  package.
- Do not edit `kanban/done/**`.
- Do not edit architecture or state. Their TW0002 comments explain an old
  id collision and a severity override. A leftover `severity = none` is
  harmless.
- `./bin/dev build` succeeds. `./bin/dev test` still passes.
- Do not implement on `master`.

## Checklist

- [x] Delete the analyzer and both generator files
- [x] Drop TW0002 and TW0003 from the release-tracking files and do not reuse them
- [x] Archive tasks 016 and 013
- [x] Delete the code-fix setup doc and the markdown-docs-only test files
- [x] Update live docs and package tags
- [x] `./bin/dev build` and `./bin/dev test` pass
- [x] `## Results` and `### How to validate`
- [x] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

## Notes

- Claim on-create self-install failed again (`MSB3030`, missing `dev.xml`).
  The claim still stands.

### How to validate

```bash
rg -n "XmlDocsToMarkdown|MarkdownDocsGenerator|TW0002|TW0003" \
  source tests readme.md documentation docs
# Expect: no hits

./bin/dev build
./bin/dev test
```

## Results

XML documentation stays in the C# file. The TW0002 analyzer, both TW0003 generator files, the code-fix setup doc, the XMLDoc template pages, and the markdown-only test fixtures are deleted. Release tracking lists TW0001 and TW0007. Those retired ids are not reused. Package tags are `TimeWarp; Analyzers`. The packed nuspec tags are `TimeWarp Analyzers`. The package id is still `TimeWarp.SourceGenerators`.

Task 016 was already archived on origin-home (`5e02ad4`). Its draft stays under `kanban/archived/016-create-xml-docs-to-markdown-code-fix/` and is not part of a project. Task 013 was archived from origin-home with `ganda kanban archive 013` and landed as `6d2a467`. This branch does not edit that file. `kanban/done/**` was not edited.

`KebabCaseTest.GetTestMessage` and `PascalCaseTest.GetPascalMessage` were implementing partials whose other half came from the generator. They are ordinary methods so the TW0001 console smoke still compiles. The test console no longer passes markdown in as additional files.

`./bin/dev build` succeeded with 0 warnings and 0 errors and packed `artifacts/packages/TimeWarp.SourceGenerators.1.0.0-beta.11.nupkg`. `./bin/dev test` printed the four console lines and reported Passed 11, Failed 0. `ganda repo audit` then passed 36, failed 0, skipped 3. `--fix` wrote the local ignored VS Code window title. The post-fix dev-cli self-install failed with the known `MSB3030` missing `dev.xml`. `./bin/dev --version` still runs.

Spikes under `spikes/partial-properties-csharp13/` still describe the old generator. They are not live docs and were left in place.

### How to validate

Smoke:

```bash
rg -n "XmlDocsToMarkdown|MarkdownDocsGenerator|TW0002|TW0003" \
  source tests readme.md documentation
# docs/ is removed with the code-fix setup page

./bin/dev build
./bin/dev test
```

Expect:

- The search prints no hits.
- Build succeeds with 0 warnings and 0 errors.
- The console prints the kebab-case, close-modal, fetch-weather, and PascalCase lines.
- Analyzer tests report Passed 11, Failed 0.

## Session

- Created: grok `01a1bbd2-4185-7452-8535-389eb11a4f8f` (2026-11-09)
- Trigger: Steve, after merging PR #40, said task 016's code fix and the analyzer are unwanted because XML docs belong in the source file
- Implementation: grok `01a1bc51-a208-7a53-a024-65c8ebeb9160` (2026-11-09)
