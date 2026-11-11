# Move file-level usings in file-length analyzer tests to global usings

## Goal (implementer restatement)

The file-length analyzer tests import `System.Globalization`, `System.Text`, and `Microsoft.CodeAnalysis.Text` as file-level usings. Those three belong in the analyzer-tests `global-usings.cs`, in alphabetical order, and the test file should not repeat them. If those globals make a name ambiguous in another file in that project, qualify or otherwise resolve that name so the project still compiles. The partial-property spike's `using TimeWarp.Terminal;` is a one-line move into a `global-usings.cs` in that project; do it because it is the same convention and does not touch hook baselines or analyzer config.

Acceptance: the three directives live only as `global using` lines in the analyzer-tests global usings file; `file-length-analyzer-tests.cs` has no file-level usings; the analyzer-tests project builds with no new ambiguity; `./bin/dev test` passes; `ganda repo audit` is clean. Hook files, `using static`, usings inside test snippet strings, and enabling TW0007 stay unchanged.

## Description

`tests/timewarp-source-generators-analyzer-tests/file-length-analyzer-tests.cs` has three
file-level using directives (lines 12-14): `System.Globalization`, `System.Text`, and
`Microsoft.CodeAnalysis.Text`. Repo convention (TW0007) is that these live in the project's
`global-usings.cs` as `global using` directives.

TW0007 cannot catch this itself: the analyzer-tests project references the generator project as a
plain ProjectReference, not as an analyzer, and this repo cannot reference its own package.
So this is a manual cleanup.

## Requirements

1. Move the three usings into the analyzer-tests project's `global-usings.cs` (keep it
   alphabetical) and delete them from `file-length-analyzer-tests.cs`.
2. Fix any ambiguity the new global usings cause in other test files in that project.
3. Optional, only if it is trivial: move `using TimeWarp.Terminal;` from
   `spikes/partial-properties-csharp13/partial-property-test/Program.cs` to a global using.

## Out of scope

- `.githooks/*.cs` (`using TimeWarp.Amuru;`): hooks are checked against the audit attest baseline.
- `using static` lines and usings inside test snippet strings: TW0007 skips or ignores these.
- Enabling TW0007 in `.editorconfig`.

## Checklist

- [x] Three usings moved to `global-usings.cs`
- [x] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: c5b005f2-d490-4e08-a7fa-e19f46a0a9e6 (2026-11-11)
- Implementation: implementer oracle (2026-11-11). Moved the three analyzer-test usings into `global-usings.cs`. No other file in that project needed a qualification. Moved the spike's `TimeWarp.Terminal` import into a new global usings file. `./bin/dev test` passed 56 of 56. `ganda repo audit` passed 42, failed 0.
- Implement oracle: implemented by implementer-cursor (cursor, vendor xai, model grok-4.7-high), session not reported, max-turns uncapped (no --max-turns flag; bound 500), Done — 2026-11-11T06:40:05Z
- Review oracle round 1: cursor (gemini-4-carbon), headless ganda task work, effort 1, roster general, max-turns uncapped (no --max-turns flag; bound 80). Disposition clean, 0 findings (2026-11-11)
- Review oracle: review by reviewer-cursor (cursor, vendor google, model gemini-4-carbon-high), session not reported, max-turns uncapped (no --max-turns flag; bound 80), artifacts general.md, merged.md, disposition.md — 2026-11-11T06:49:01Z
- Installed ganda changed during this walk: was 52427784 bytes 10036feefb89c261f3cb88b300b00f8b3163d1c0606d5b9489db5c4ecee5feda; now 52431880 bytes c7bbb61cd011fc4249bc48c66cafc9f933344941eca4d4e2bd3c7da7b9a4cf32

## Results

`System.Globalization`, `System.Text`, and `Microsoft.CodeAnalysis.Text` are `global using` directives in `tests/timewarp-source-generators-analyzer-tests/global-usings.cs`, in alphabetical order. `file-length-analyzer-tests.cs` no longer has file-level usings. The other analyzer-test files did not gain an ambiguous name from those globals, so none of them changed. `using static` lines stayed. The partial-property spike imports `TimeWarp.Terminal` from `spikes/partial-properties-csharp13/partial-property-test/global-usings.cs`, and `Program.cs` no longer has a file-level using.

### How to validate

Smoke: from this worktree run `./bin/dev test`, `ganda repo audit`, and `dotnet run --project spikes/partial-properties-csharp13/partial-property-test/partial-property-test.csproj -c Release`.

Expect: the test suite prints `Passed: 56  Failed: 0`. `ganda repo audit` exits 0 with 42 passed and 0 failed. The spike prints `test`. `file-length-analyzer-tests.cs` has no `using` directive. The three namespaces appear only as `global using` lines in the analyzer-tests `global-usings.cs`.

### Review disposition

- **Outcome:** clean. 1 round, effort 1, roster `general`.
- **Final counts:** bug 0, suggestion 0, nit 0. Open 0, fixed 0, wontfix 0.
- The reviewer verified the restatement against the task (no misread) and re-ran build (0 warnings, 0 errors), test (`56/56`), the partial-property spike (`test`), and `ganda repo audit` (all 42 local checks passed). No exceptions or escalations.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.

## Review

- Round 1: Done

## Notes

Requested by Steven 2026-11-11.
