# Update TimeWarp.Build.Tasks reference to 1.0.1

## Description

`TimeWarp.Build.Tasks` 1.0.1 is available. This repo pins 1.0.0 in `Directory.Packages.props`
(line 18); the reference itself is in `Directory.Build.props` (line 88, `PrivateAssets="all"`).

## Requirements

1. Set `<PackageVersion Include="TimeWarp.Build.Tasks" Version="1.0.1" />` in
   `Directory.Packages.props`.
2. Restore and build. TimeWarp packages may come from the org GitHub Packages feed
   (https://nuget.pkg.github.com/TimeWarpEngineering/index.json) as well as nuget.org. If 1.0.1
   does not restore from the configured sources, stop and report rather than adding a feed.
3. Fix anything 1.0.1 breaks or newly reports in the build.
4. Bump `<Version>` in `source/Directory.Build.props` only if the pre-PR version check requires it.

## Checklist

- [ ] Pin is 1.0.1 and restore succeeds
- [ ] `./bin/dev test` passes; `ganda repo audit` clean

## Session

- Created: c5b005f2-d490-4e08-a7fa-e19f46a0a9e6 (2026-11-11)

## Notes

Requested by Steven 2026-11-11.
