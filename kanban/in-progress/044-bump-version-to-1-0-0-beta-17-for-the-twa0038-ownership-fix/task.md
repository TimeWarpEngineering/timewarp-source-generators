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

## Goal (implementer restatement)

beta.16 is already on nuget.org and holds only the task 042 dependency refresh. The TWA0038
ownership fix (task 040) is on master under the beta.16 version and release heading, so it cannot
ship. Move `<Version>` to `1.0.0-beta.17`, put the task 040 notes under a `## 1.0.0-beta.17`
heading unchanged, and add a `## 1.0.0-beta.16` entry that lists only what
`v1.0.0-beta.15..v1.0.0-beta.16` changed (Roslyn 5.9.0 → 5.12.0, Nuru/Nuru.DevCli
4.0.0-beta.12 → 4.0.0-beta.13, Build.Tasks 2.0.0 → 2.0.1-beta.1). Correct any other doc that
ties the fix or `exclude_paths` to beta.16. No product code.

Done when: `./bin/dev check-version` passes, releases.md is truthful for both versions, no other
doc names beta.16 for the fix, and build, test, and audit pass.

## Checklist

- [x] `<Version>` is `1.0.0-beta.17`; `./bin/dev check-version` passes
- [x] `documentation/releases.md` has the TWA0038 notes under beta.17 and a truthful beta.16 entry
- [x] No other doc attributes the TWA0038 fix to beta.16
- [x] `./bin/dev build` and `./bin/dev test` pass; `ganda repo audit` clean

## Session

- Created: 83d187ba-085b-471b-8964-44a61c5cebc6 (2026-11-17)
- 2026-11-17 implement (ganda task work, Claude Opus 5.5): version bump and release notes. Searched readme, `documentation/`, and `AnalyzerReleases.*.md`: none names a package version for the TWA0038 fix or `exclude_paths`, so only `releases.md` changed. Audit boy-scout: the advisory `vscode-window-icon` warning was fixed by `ganda repo audit --fix`, which writes the gitignored `.vscode/settings.json`; nothing tracked changed.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5, response model claude-opus-5-5), session 56431ef2-3cd7-4bf8-bffe-a75e8b3146ac, max-turns 500, Done — 2026-11-17T13:10:42Z
- Reviewer: grok session 01a1e9a1-b181-70a3-9fe0-5b25965c6740 (2026-11-17), effort 1, roster general. Round 1 disposition clean. No misread between the implementer restatement and the original task.
- Review oracle: review by reviewer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1e9a1-b181-70a3-9fe0-5b25965c6740, max-turns 80, artifacts general.md, merged.md, disposition.md — 2026-11-17T13:14:49Z

## Review

- Round 1: Done

## Notes

Requested by Steven 2026-11-17 ("release beta.16"; the tag already exists, so the fix ships as
beta.17). The release itself (`dev release`) is cut from origin-home master after this merges
and is not part of this task.

## Results

- `source/Directory.Build.props`: `<Version>` 1.0.0-beta.16 → 1.0.0-beta.17.
- `documentation/releases.md`: the task 040 TWA0038 notes now sit under `## 1.0.0-beta.17`, text unchanged. New `## 1.0.0-beta.16` entry lists only the `v1.0.0-beta.15..v1.0.0-beta.16` changes: Roslyn 5.9.0 → 5.12.0, Nuru and Nuru.DevCli 4.0.0-beta.12 → 4.0.0-beta.13, TimeWarp.Build.Tasks 2.0.0 → 2.0.1-beta.1 (build-time only).
- No other doc ties the fix to beta.16 (`readme.md`, `documentation/developer/reference/analyzers/*.md`, and `AnalyzerReleases.Unshipped.md` name no package version). No product code changed.

### Gates

| Gate | Command | Result |
|------|---------|--------|
| Version | `./bin/dev check-version` | Pass: source is 1 prerelease increment ahead of v1.0.0-beta.16, "safe to release" |
| Build | `./bin/dev build` | Pass: 0 warnings, 0 errors; packs `TimeWarp.SourceGenerators.1.0.0-beta.17.nupkg` |
| Test | `./bin/dev test` | Pass: 64/64 |
| Audit | `ganda repo audit` | Pass with 1 advisory warning (`vscode-window-icon`); `ganda repo audit --fix` then reported 43 passed, 0 failed |

### How to validate

Smoke:

```bash
./bin/dev check-version
grep -n '<Version>' source/Directory.Build.props
grep -n '^## 1.0.0-beta.1[5-7]' documentation/releases.md
git diff v1.0.0-beta.15 v1.0.0-beta.16 -- Directory.Packages.props
```

Expect: `check-version` says the version is new and safe to release. `<Version>1.0.0-beta.17</Version>`. `releases.md` headings appear in order beta.17, beta.16, beta.15; the beta.17 body is the TWA0038 ownership text and the beta.16 body lists exactly the pin changes the `git diff` shows.

### Review

- Round 1, effort 1, reviewer roster: general (grok session 01a1e9a1-b181-70a3-9fe0-5b25965c6740, 2026-11-17).
- Counts: bug 0 open / 0 fixed / 0 wontfix; suggestion 0; nit 0. Final open count 0.
- Disposition: clean. The implementer restatement matches the original task. No wontfix and no escalation.
- Paths: `review/review-framework.md`, `review/round-1/general.md`, `review/round-1/merged.md`, `review/disposition.md`.
