# Update TimeWarp.Build.Tasks reference to 2.0.0

## Description

`ganda nuget outdated` (2026-11-12) reports `TimeWarp.Build.Tasks` 1.0.2 -> 2.0.0 on nuget.org.
It is the only outdated package. This repo pins 1.0.2 in `Directory.Packages.props`; the
reference itself is in `Directory.Build.props` (`PrivateAssets="all"`).

This is a major version bump, so expect breaking changes: removed, renamed, or re-defaulted
MSBuild tasks, targets, or properties.

## Requirements

1. Before changing the pin, find out what 2.0.0 changed against 1.0.2 (release notes, or diff
   the two packages with `tw-dotnet-inspect`) and record the breaking changes that touch this
   repo in `## Notes`.
2. Set `<PackageVersion Include="TimeWarp.Build.Tasks" Version="2.0.0" />` in
   `Directory.Packages.props`. Leave the `PackageReference` in `Directory.Build.props` as it is
   unless 2.0.0 requires a change.
3. Restore and build with the configured sources (nuget.org). If 2.0.0 does not restore, stop
   and report; do not add a NuGet feed.
4. Fix any build break or new diagnostic that 2.0.0 introduces. Do not suppress a diagnostic to
   get a green build.
5. TimeWarp.SourceGenerators and TimeWarp.Build.Tasks had a circular dependency that Build.Tasks
   1.0.2 removed (task 038). Confirm 2.0.0 does not bring back a dependency on
   TimeWarp.SourceGenerators. If it does, stop and report.
6. Bump `<Version>` in `source/Directory.Build.props` only when `./bin/dev check-version` says
   the current package version is already published.

## Checklist

- [ ] Breaking changes in 2.0.0 that affect this repo are recorded in Notes
- [ ] 2.0.0 has no dependency on TimeWarp.SourceGenerators
- [ ] Pin is 2.0.0 and restore succeeds
- [ ] `./bin/dev build` reports 0 warnings and 0 errors
- [ ] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: 83d187ba-085b-471b-8964-44a61c5cebc6 (2026-11-12)

## Notes

Requested by Steven 2026-11-12. Previous bump of this package: task 038 (1.0.0 -> 1.0.2).
