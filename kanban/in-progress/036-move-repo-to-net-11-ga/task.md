# Move repo to .NET 11 GA

## Description

.NET 11 GA (SDK 11.0.100) is released and installed locally. This repo is only partly on it:
CI still installs the release candidate and most projects still target net10. Move everything
to .NET 11 GA.

Current state (2026-11-11):

- `.github/workflows/workflow.yml` installs `10.0.x` and `11.0.100-rc.3.26522.148`.
- `Directory.Build.props` sets `<TargetFramework>net10.0</TargetFramework>` (repo default).
- `tests/timewarp-source-generators-analyzer-tests`, `tests/timewarp-source-generators-test-console`,
  and `spikes/partial-properties-csharp13/partial-property-test` target `net10.0`.
- `tools/dev-cli` and `.githooks` already target `net11.0`.
- No `global.json`.

## Requirements

1. CI installs the GA .NET 11 SDK (`11.0.x`), not the RC. Drop the `10.0.x` install once nothing
   targets net10, and update the comment that explains why both were installed.
2. Every project that targets `net10.0` targets `net11.0` (repo default, tests, test console, spike).
3. The shipped package `source/timewarp-source-generators` stays on `netstandard2.0`. Analyzers and
   generators must load in the compiler, so do not retarget it.
4. Replace any RC or preview .NET 11 package or SDK versions with GA versions.
5. Update docs, readme, and runfile shebang/`#:` directives that name net10 or the RC.
6. Bump `<Version>` in `source/Directory.Build.props` only if the pre-PR version check requires it.

## Checklist

- [ ] CI workflow on `11.0.x` GA, net10 install removed
- [ ] Repo default and all net10 projects moved to `net11.0`
- [ ] Shipped package still `netstandard2.0`
- [ ] No RC/preview .NET 11 pins left (grep for `rc.` and `preview`)
- [ ] Docs and comments updated
- [ ] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: c5b005f2-d490-4e08-a7fa-e19f46a0a9e6 (2026-11-10)

## Notes

Requested by Steven 2026-11-11: "move all to dotnet 11".
