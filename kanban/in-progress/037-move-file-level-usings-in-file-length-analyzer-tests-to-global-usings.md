# Move file-level usings in file-length analyzer tests to global usings

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

- [ ] Three usings moved to `global-usings.cs`
- [ ] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: c5b005f2-d490-4e08-a7fa-e19f46a0a9e6 (2026-11-11)

## Notes

Requested by Steven 2026-11-11.
