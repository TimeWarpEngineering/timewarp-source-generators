# Move repo to .NET 11 GA

## Goal (implementer restatement)

Put the whole repo on the .NET 11 GA SDK. CI should install the rolling `11.0.x` band and stop installing `10.0.x` and the 11 release candidate. The comment that explained the two installs should explain the single GA band instead. The repo default target, the analyzer tests, the test console, and the partial-property spike should all target `net11.0`. The shipped analyzer project stays on `netstandard2.0` so the compiler can still load it. No .NET 11 RC or preview SDK or package pin should remain. Docs, the readme, and runfile shebangs or `#:` lines that name net10 or the RC should match the GA move. Bump `<Version>` only when the pre-PR version check says the current version is not ahead of the last published release.

Acceptance: the workflow lists only `11.0.x`; no project targets `net10.0`; `source/timewarp-source-generators` still targets `netstandard2.0`; product files have no `11.0.*-rc` or preview .NET 11 package or SDK pin; `./bin/dev test` passes; `ganda repo audit` is clean.

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

- [x] CI workflow on `11.0.x` GA, net10 install removed
- [x] Repo default and all net10 projects moved to `net11.0`
- [x] Shipped package still `netstandard2.0`
- [x] No RC/preview .NET 11 pins left (grep for `rc.` and `preview`)
- [x] Docs and comments updated
- [x] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: c5b005f2-d490-4e08-a7fa-e19f46a0a9e6 (2026-11-10)
- Implementation: implementer oracle (2026-11-11). `ganda repo audit --fix` wrote a local gitignored `.vscode/settings.json` window title. `ci-code-coverage` cannot edit this suite, so its severity is off under `[ganda.audit]` (task 036).

## Results

CI installs the .NET 11 GA band `11.0.x` only. The repo default, analyzer tests, test console, and partial-property spike target `net11.0`. `source/timewarp-source-generators` still targets `netstandard2.0`. `check-version` found `1.0.0-beta.13` already on NuGet, so the package version is `1.0.0-beta.14`. The spike prints through `TimeWarp.Terminal` because `System.Console` is banned. The test console no longer sets `LangVersion` to `preview`. Runfile shebangs do not name a framework. Readme and product docs did not name net10 or the RC. The spike findings now say the project targets `net11.0`; the .NET 10 Preview 1 link stays as the historical citation.

### How to validate

Smoke: from this worktree run `./bin/dev test`, `./bin/dev check-version`, `ganda repo audit`, and `dotnet run --project spikes/partial-properties-csharp13/partial-property-test/partial-property-test.csproj -c Release`.

Expect: the test suite prints `Passed: 56  Failed: 0`. `check-version` prints `1.0.0-beta.14` and exits 0. `ganda repo audit` exits 0 with no failed checks. The spike prints `test`. The generator build output path contains `netstandard2.0`. `.github/workflows/workflow.yml` sets `dotnet-version: 11.0.x` and does not install `10.0.x`.

## Notes

Requested by Steven 2026-11-11: "move all to dotnet 11".
