# Retarget the source generator project from netstandard2.0 to net11.0

## Description

`source/timewarp-source-generators/timewarp-source-generators.csproj` is the only project in
the repo still on `netstandard2.0`. Everything else is `net11.0`. Move it to `net11.0`.

Decision (Steven, 2026-11-12): this is deliberate. The package will no longer load in a
compiler host that runs on .NET Framework, which means Visual Studio. That is accepted:
"Visual Studio is the one that is behind not us." Do not multi-target and do not keep a
`netstandard2.0` asset as a fallback.

## Requirements

1. Set `<TargetFramework>net11.0</TargetFramework>` in the generator csproj. If the repo-level
   `Directory.Build.props` already supplies `net11.0`, removing the line is fine too; pick
   whichever matches the other projects.
2. Roslyn's analyzer rules warn when a compiler extension targets something other than
   `netstandard2.0` (RS1041, and possibly others under `EnforceExtendedAnalyzerRules`). Because
   the retarget is the decision, suppress exactly the diagnostic(s) that say "wrong target
   framework" and nothing else, with a comment that names this task. Report every diagnostic id
   you suppress in Results. Any other new diagnostic gets fixed, not suppressed.
3. Remove what only existed for `netstandard2.0`: polyfills (`IsExternalInit`, nullable
   attributes, index/range and similar), polyfill packages, and `#if` branches. Leave the
   `NoWarn` entries (`RS2001`, `NU5128`) alone unless the retarget makes one unnecessary; say
   which in Results.
4. Packaging: the analyzer DLL is packed by hand to `analyzers/dotnet/cs` from `$(OutputPath)`.
   Confirm `dotnet pack` still puts the `net11.0` DLL there and that the test console and the
   analyzer tests still load the generator and analyzers from the built output.
5. This breaks consumers on older hosts, so:
   - bump `<Version>` in `source/Directory.Build.props` to the next beta if
     `./bin/dev check-version` requires it;
   - add a `documentation/releases.md` entry for that version that says the package now targets
     `net11.0`, needs the .NET 11 SDK compiler, and does not load in Visual Studio. The beta.14
     entry says "The shipped package still targets `netstandard2.0`"; leave history as it is.
   - update `readme.md` and any other doc that states a `netstandard2.0` target or Visual
     Studio support.
6. Commit as a breaking change (`feat!:` or a `BREAKING CHANGE:` footer).

## Checklist

- [ ] Generator project targets `net11.0`; no `netstandard2.0` target left in the repo
- [ ] Only the target-framework analyzer diagnostic(s) are suppressed, each with a comment
- [ ] netstandard-only polyfills and conditionals removed
- [ ] `./bin/dev build` reports 0 warnings and 0 errors
- [ ] `./bin/dev test` passes (generators and analyzers load from the `net11.0` build)
- [ ] Packed nupkg has the `net11.0` DLL under `analyzers/dotnet/cs`
- [ ] `documentation/releases.md` and `readme.md` state the new target and the Visual Studio limit
- [ ] `ganda repo audit` clean

## Session

- Created: 83d187ba-085b-471b-8964-44a61c5cebc6 (2026-11-12)

## Notes

Requested by Steven 2026-11-12. Unreleased `1.0.0-beta.15` (task 039, Build.Tasks 2.0.0) is
already on master; `check-version` decides whether this task needs its own bump.
