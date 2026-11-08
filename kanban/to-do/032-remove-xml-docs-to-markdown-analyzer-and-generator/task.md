# Remove XML docs to markdown analyzer and generator

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

- [ ] Delete the analyzer and both generator files
- [ ] Drop TW0002 and TW0003 from the release-tracking files and do not reuse them
- [ ] Archive tasks 016 and 013
- [ ] Delete the code-fix setup doc and the markdown-docs-only test files
- [ ] Update live docs and package tags
- [ ] `./bin/dev build` and `./bin/dev test` pass
- [ ] `## Results` and `### How to validate`
- [ ] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

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

## Session

- Created: grok `01a1bbd2-4185-7452-8535-389eb11a4f8f` (2026-11-09)
- Trigger: Steve, after merging PR #40, said task 016's code fix and the analyzer are unwanted because XML docs belong in the source file
