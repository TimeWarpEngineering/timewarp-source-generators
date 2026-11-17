# Bump version to 1.0.0-beta.17 for the TWA0038 ownership fix

## Description

`1.0.0-beta.16` was tagged and published to nuget.org on 2026-11-14 from commit `addddba`
(task 042, the NuGet package refresh). Task 040 (TWA0038 skips files the consuming repo does
not own, PR #53) merged after that, but it did not bump `<Version>`, and it wrote its release
notes under the `## 1.0.0-beta.16` heading. `./bin/dev check-version` now fails:
"Version 1.0.0-beta.16 was already released." The fix cannot be released until the version moves.

## Requirements

1. Set `<Version>1.0.0-beta.17</Version>` in `source/Directory.Build.props`.
2. In `documentation/releases.md`, rename the current `## 1.0.0-beta.16` heading (the TWA0038
   ownership notes from task 040) to `## 1.0.0-beta.17`. Do not change that entry's text.
3. Below it, add a `## 1.0.0-beta.16` entry that says what beta.16 actually shipped. Take it
   from `git log v1.0.0-beta.15..v1.0.0-beta.16` (the package refresh in task 042). Keep it to
   the facts in that range.
4. Fix any other doc that says the TWA0038 ownership fix or `exclude_paths` shipped in beta.16
   (`readme.md`, `AnalyzerReleases.*.md` if they name a package version).
5. No product code changes.

## Checklist

- [ ] `<Version>` is `1.0.0-beta.17`; `./bin/dev check-version` passes
- [ ] `documentation/releases.md` has the TWA0038 notes under beta.17 and a truthful beta.16 entry
- [ ] No other doc attributes the TWA0038 fix to beta.16
- [ ] `./bin/dev build` and `./bin/dev test` pass; `ganda repo audit` clean

## Session

- Created: 83d187ba-085b-471b-8964-44a61c5cebc6 (2026-11-17)

## Notes

Requested by Steven 2026-11-17 ("release beta.16"; the tag already exists, so the fix ships as
beta.17). The release itself (`dev release`) is cut from origin-home master after this merges
and is not part of this task.
