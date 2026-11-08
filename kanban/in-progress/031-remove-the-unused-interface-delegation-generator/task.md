# Remove the unused interface delegation generator

## Goal (implementer restatement)

Take the unused Delphi-style interface delegation generator out of this package. Delete the generator, the handwritten attribute (the generator's emitted copy goes with the generator), the three diagnostic rows, and the test-console sample that exists only for that feature. Leave the markdown-docs sample (`i-data-processor.cs` and `test-class.cs`) in place. Docs should name the diagnostics that remain — TW0001, TW0002, TW0003, and TW0007 — and should no longer offer delegation. Those three retired ids stay unused. Done cards that record the old assignment stay untouched. Archive task 026, the unfinished create card, instead of building the generator. `./bin/dev build` and `./bin/dev test` pass. Stop after Results: no `ganda kanban done`, no pull request.

## Description

`InterfaceDelegationGenerator` (`[Implements]`, diagnostics **TW0004**–**TW0006**)
is a Delphi-style delegation generator that shipped in this repo on
2025-10-17 (`479d5d3`) and is not called anywhere.

GitHub code search on 2026-11-08:

- `ImplementsAttribute` and `[Implements]` inside `lang:csharp` match only
  this repo: the generator, the handwritten attribute, and
  `tests/timewarp-source-generators-test-console/data-service.cs`.
- `InterfaceDelegationGenerator` in the TimeWarpEngineering org matches only
  this repo (source plus historical kanban).
- A separate `CloakBrowser.Generators.InterfaceDelegationGenerator` exists
  under CloakHQ and a few forks. It is not a reference to
  `TimeWarp.SourceGenerators`.

Other TimeWarp mentions are documentation, not callers: this repo's
`readme.md`, `timewarp-software` `public/packages/timewarp.sourcegenerators/index.md`
(a copy of that readme section), and one bullet in `hub` `profile/README.md`.
State and architecture docs name the `TW0001`–`TW0006` id range so their own
analyzers do not collide with it. They do not use `[Implements]`.

Task **026** is the unfinished "create this generator" card. The generator is
already on master. Close 026 by removing the feature, not by finishing it.

## Requirements

- Delete `source/timewarp-source-generators/interface-delegation-generator.cs`.
- Delete `source/timewarp-source-generators/interface-delegation-attribute.cs`.
  The generator also emits an `internal ImplementsAttribute`. Both copies go.
- Remove the **TW0004**, **TW0005**, and **TW0006** rows from
  `AnalyzerReleases.Unshipped.md`. Do not reuse those ids.
- Delete the test-console files that exist only for this sample:
  `data-service.cs`, `i-logger.cs`, `console-logger.cs`.
  Keep `i-data-processor.cs` and `test-class.cs`. `TestClass` implements
  `IDataProcessor<string>` for the markdown-docs sample.
- Remove the "Interface Delegation Generator" section from `readme.md`.
- In `documentation/overview.md`, stop saying the package owns
  `TW0001`–`TW0006`. The live ids after this removal are **TW0001**,
  **TW0002**, **TW0003**, and **TW0007**.
- Do not edit `kanban/done/**`. Those cards record the old id assignment.
- Archive task **026** so the create card is not left in to-do.
- `./bin/dev build` succeeds. `./bin/dev test` still passes.
- Do not implement on `master`.

Out of this repo (do not edit them here):

- `hub` `profile/README.md` still names interface delegation in the package blurb.
- `timewarp-software` `public/packages/timewarp.sourcegenerators/index.md`
  still copies the readme section.

## Checklist

- [x] Delete the generator and the handwritten attribute
- [x] Drop TW0004–TW0006 from `AnalyzerReleases.Unshipped.md` and do not reuse them
- [x] Delete `data-service.cs`, `i-logger.cs`, and `console-logger.cs`
- [x] Remove the readme section and fix the id range in `documentation/overview.md`
- [x] Archive task 026
- [x] `./bin/dev build` and `./bin/dev test` pass
- [x] `## Results` and `### How to validate`
- [x] Stop after product work and Results. Do not `kanban done`. Do not `gh pr create`

## Notes

- Search commands (2026-11-08):

```bash
gh search code --owner TimeWarpEngineering --language C# "ImplementsAttribute"
gh search code --owner TimeWarpEngineering --language C# "[Implements]"
gh search code --owner TimeWarpEngineering "InterfaceDelegationGenerator"
```

- Claim on-create self-install failed again (`MSB3030`, missing `dev.xml`).
  The claim still stands. `bin/dev` may be missing until self-install works.

### How to validate

```bash
rg -n "InterfaceDelegationGenerator|ImplementsAttribute|\\[Implements\\]|TW0004|TW0005|TW0006" \
  source tests readme.md documentation
# Expect: no hits

./bin/dev build
./bin/dev test
```

## Results

Removed the unused interface delegation generator, its handwritten attribute, and the test-console sample that existed only for that feature (`data-service.cs`, `i-logger.cs`, `console-logger.cs`). `i-data-processor.cs` and `test-class.cs` stay for the markdown-docs sample. The three diagnostic rows are gone from `AnalyzerReleases.Unshipped.md` and were not reassigned. The readme no longer documents the generator. `documentation/overview.md`, the readme prefix table, and the analyzer reference overview name the live ids: TW0001, TW0002, TW0003, and TW0007. `kanban/done/**` was not edited.

Task 026 is archived. `ganda kanban archive 026` ran from origin-home, committed on `task/026-create-interface-delegation-generator`, and landed on master as `0e4b3f5`. This branch does not contain that land. The claim on 026 remains.

`./bin/dev build` succeeded with 0 warnings and 0 errors. `./bin/dev test` passed: the test console printed the kebab-case, multi-dot, and PascalCase messages, and the analyzer suite reported Passed 11, Failed 0. `ganda repo audit` passes all checks. `ganda repo audit --fix` wrote a gitignored `.vscode/settings.json` window title so the window-icon check passes. The post-fix self-install still fails with the known MSB3030 missing `dev.xml`; `./bin/dev build` is the build that passed.

Out of this repo, still describing the removed feature: `hub` `profile/README.md` and `timewarp-software` `public/packages/timewarp.sourcegenerators/index.md`.

### How to validate

**Smoke:**

```bash
rg -n "InterfaceDelegationGenerator|ImplementsAttribute|\[Implements\]|TW0004|TW0005|TW0006" \
  source tests readme.md documentation
./bin/dev build
./bin/dev test
```

**Expect:**

- The search prints nothing.
- Build succeeds with 0 warnings and 0 errors.
- Test console prints the kebab-case, multi-dot, and PascalCase messages. Analyzer tests report Passed 11, Failed 0.

## Session

- Created: grok `01a1bbd2-4185-7452-8535-389eb11a4f8f` (2026-11-08)
- Trigger: Steve asked whether any repo uses the interface delegation generator, and to create a removal task if not
- Implementation: grok `01a1bc11-9829-7471-9676-451eab200c95` (2026-11-08)
