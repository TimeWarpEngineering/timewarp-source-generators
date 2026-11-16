# Adopt the shared pull request build classifier and the one-workflow CI standard

## Description

Bring timewarp-source-generators to the org CI standard (ganda 476 / 479 / 565 / 587, audited by ganda 655): `workflow.yml` has
no `paths` / `paths-ignore` filter on `pull_request`, so every pull request reports `CI`, and `dev workflow`
calls the shared classifier `PullRequestBuildClassifier` from `TimeWarp.Nuru.DevCli` 4.0.0-beta.15
(timewarp-nuru 595) to skip build and test on a pull request that touches no build path (kanban, docs, ...).

Tracked by timewarp-ganda 656 (architecture reposet rollout).

Survey 2026-11-16 for this repo: has a `pull_request` paths filter; `dev workflow` does not classify. Work: remove the filter, add the classifier call.

## Requirements

1. Run `ganda repo audit --fix` first and commit what it changes.
2. Bump the `TimeWarp.Nuru.DevCli` pin to `4.0.0-beta.15` (and `TimeWarp.Nuru` to the same version if pinned).
3. In the `workflow` command's pull-request path, call `PullRequestBuildClassifier.DecideAsync` first. On skip,
   print its message and exit 0. Push and release paths are unchanged.
4. Remove `paths` / `paths-ignore` from the `pull_request` trigger of `.github/workflows/workflow.yml`. Keep any
   `push` filter and add a comment saying why (a kanban-only merge must not rebuild and upload packages).
5. If this repo builds from roots outside the classifier defaults (`source`, `tests`, `tools`, `msbuild`), declare
   them through the classifier's config (`PullRequestBuildConfig`), not by copying or editing the rule.
6. No local copy of the classification rule remains in `tools/dev-cli/`.
7. `ganda repo audit` passes, including both CI-standard checks from ganda 655.

## Goal (implementer restatement)

Every pull request in this repo must report a `CI` check, so `workflow.yml` loses its `pull_request` `paths`
filter. To keep docs/kanban-only pull requests cheap, `dev workflow` asks the shared
`PullRequestBuildClassifier` (TimeWarp.Nuru.DevCli 4.0.0-beta.15 content) whether the pull request touches a
build path; when it does not, it prints the classifier's message and exits 0 without clean/build/test.
Push and release paths behave as before (the classifier always answers "build" for non-`pull_request`
events). The `push` filter stays, with a comment saying a kanban-only merge must not rebuild and upload packages.

Acceptance:
- `ganda repo audit --fix` output committed first.
- `TimeWarp.Nuru` and `TimeWarp.Nuru.DevCli` pinned at 4.0.0-beta.15.
- `dev workflow` calls `PullRequestBuildClassifier.DecideAsync(repoRoot, ct)` first on the PR path; skip exits 0.
- No `paths` on `pull_request`; `push` filter kept with a why comment.
- Build inputs outside the default roots are declared in `.timewarp/dev.jsonc` under
  `pullRequestBuild.buildPaths`: `readme.md` and `assets/**`, which are packed into the nupkg
  (`PackageReadmeFile` / `PackageIcon`). The default rule treats both as non-build (review round 1, M1).
- No local copy of the rule in `tools/dev-cli/`.
- `./bin/dev build`, `./bin/dev test`, and `ganda repo audit` pass (both ganda 655 CI-standard checks).

## Checklist

- [x] `ganda repo audit --fix` run and committed
- [x] DevCli pin at 4.0.0-beta.15
- [x] `dev workflow` calls the shared classifier on pull requests; skip exits 0 with its message
- [x] No `paths` filter on `pull_request`; `push` filter kept with a why comment
- [x] Extra build roots (if any) declared through config
- [x] `ganda repo audit` clean
- [ ] This task's pull request shows a `CI` run (record the URL in Results)

## Results

- `ganda repo audit --fix` added `.githooks/prepare-commit-msg` (+ `.cs`); committed first.
- Pins bumped in `Directory.Packages.props`: TimeWarp.Nuru and TimeWarp.Nuru.DevCli 4.0.0-beta.13 -> 4.0.0-beta.15;
  boy-scout for the `package-pin-currency` audit check: TimeWarp.Amuru / Amuru.Tools 3.0.0-beta.2 -> 3.0.0-beta.5,
  TimeWarp.Terminal 2.0.0-beta.1 -> 2.0.0-beta.2.
- `tools/dev-cli/endpoints/workflow-command.cs`: `RunPrWorkflowAsync` first calls
  `PullRequestBuildClassifier.DecideAsync(RepoRoot, Ct)`, prints `decision.Message`, and returns (exit 0) when
  `!decision.RequiresBuild`. Release path unchanged; push (merge mode) shares the PR pipeline but the classifier
  always builds for non-`pull_request` events.
- `.github/workflows/workflow.yml`: `paths` removed from `pull_request`; `push` filter kept with a why comment.
  Checkout already uses `fetch-depth: 0`, which the classifier's three-dot diff needs.
- Extra build inputs (fix for review round 1, M1): `.timewarp/dev.jsonc` declares
  `pullRequestBuild.buildPaths` = `readme.md`, `assets/**`. Both are packed into TimeWarp.SourceGenerators
  (`readme.md` from the csproj and `PackageReadmeFile`; `assets/logo.png` as `PackageIcon` in
  `source/Directory.Build.props`), so a pull request that changes them builds and packs. The Design comment in
  `workflow-command.cs` names these additions. The rest of the repo builds from `source`, `tests`, `tools`,
  `msbuild` plus root files (`Directory.*.props`, `nuget.config`, `.slnx`), which the classifier already treats
  as build paths. No local classification rule exists in `tools/dev-cli/`.
- CI run URL: recorded after the host opens the pull request.

### Gates

- `./bin/dev build` — passed (0 warnings, 0 errors). Dev CLI recompiled with `dotnet run --file tools/dev-cli/dev.cs -- self-install` (succeeded) after the dev-cli change.
- `./bin/dev test` — passed (56 passed, 0 failed).
- `ganda repo audit` — passed ("Repository passes all audit checks"), including `workflow-file` and `dev-workflow-classifier`.

### Gates (fix pass, review round 1 M1)

- `./bin/dev build` — passed (0 warnings, 0 errors).
- `./bin/dev test` — passed (56 passed, 0 failed).
- `ganda repo audit` — passed ("Repository passes all audit checks").
- Spot check: a synthetic `pull_request` event whose diff is `readme.md` only printed
  "Build path changed: readme.md. Full build." and started the pipeline.

### How to validate

Smoke:
```bash
./bin/dev self-install   # or: dotnet run --file tools/dev-cli/dev.cs -- self-install
B=$(git rev-parse dd9e260); H=$(git rev-parse 3d94b1a)   # kanban-only diff
echo "{\"pull_request\":{\"base\":{\"sha\":\"$B\"},\"head\":{\"sha\":\"$H\"}}}" > /tmp/ev.json
GITHUB_EVENT_NAME=pull_request GITHUB_EVENT_PATH=/tmp/ev.json ./bin/dev workflow; echo $?
grep -n -A3 'pull_request:' .github/workflows/workflow.yml
ganda repo audit
```

Expect:
- `dev workflow` prints "Pull request changes only declared non-build paths. Skipping build and test. ... (1 changed paths)"
  and exits 0 without running Clean/Build/Test. A diff touching a build path (e.g. head = this branch's HEAD)
  prints "Build path changed: ... Full build." and runs the pipeline.
- `pull_request` has only `branches`, no `paths`; `push` keeps its `paths` list with a comment.
- `ganda repo audit` passes, with `workflow-file` and `dev-workflow-classifier` PASS.
- A pull request event whose diff touches only `readme.md` or `assets/logo.png` prints "Build path changed: ..."
  and runs the full pipeline (declared in `.timewarp/dev.jsonc` `pullRequestBuild.buildPaths`).
- The task pull request shows a `CI` check.

### Review

Round 1 (effort 2, roster: general) raised M1 (bug): packed `readme.md` and `assets/logo.png` were not declared as build paths. Round 2 (effort 1, roster: general) re-reviewed the fix delta `0851645`..`1acceba`. M1 is fixed. No new findings. Final counts: bug 0 open / 1 fixed / 0 wontfix; suggestion 0; nit 0. Disposition: clean. Paths: `review/review-framework.md`, `review/round-1/merged.md`, `review/round-2/general.md`, `review/round-2/merged.md`, `review/disposition.md`.

## Review

- Round 1: M1 fixed in round 2
- Round 2: Done
## Notes


After merge, the ganda 656 tracker proves a kanban-only pull request here reports `CI` success and merges with
plain `ganda pr merge` (no `--allow-no-ci`).

## Session

- Created: 64677b22-d705-4b25-b537-4b53732bcb92 (2026-11-16)
- 2026-11-16 implement: classifier wired, pr paths filter removed, pins bumped (boy-scout: package-pin-currency); build/test/audit green.
- Implement oracle: implemented by implementer-claude (claude, vendor anthropic, model claude-opus-5-5), session not reported, max-turns 500, Done — 2026-11-16T14:50:25Z
- 2026-11-16 review round 1: general (grok, session `01a1e4d6-a1d2-70a2-b465-9d6499d4324b`), effort 2, turn cap 120. M1 open. Disposition not written.
- Review oracle: review by reviewer-grok (grok, vendor xai, model grok-4.7, response model grok-4.7-build), session 01a1e4d6-a1d2-70a2-b465-9d6499d4324b, max-turns 120, artifacts general.md, merged.md — 2026-11-16T14:59:02Z
- 2026-11-16 implement-fix-2: M1 fixed — `pullRequestBuild.buildPaths` (`readme.md`, `assets/**`) in `.timewarp/dev.jsonc`; Design comment updated; build/test/audit green.
- 2026-11-16 review round 2: general (grok, session `01a1e4e0-431b-7263-934a-d67a013a47d4`), effort 1, turn cap 80. M1 fixed. No new findings. Disposition: clean.

## Learnings

- Misread: the Goal restatement said this repo builds only from the classifier's default roots plus root files that already count as build paths, so no `pullRequestBuild` config is needed. Requirement 5 asks for build inputs outside `source`, `tests`, `tools`, and `msbuild` to be declared through `PullRequestBuildConfig`. `readme.md` and `assets/logo.png` are packed into the package, and the default rule treats both as non-build. That gap was M1.
- The fix pass corrected the Goal restatement: it now names `pullRequestBuild.buildPaths` for `readme.md` and `assets/**`. Round 2 compared that restatement with requirements 1–7 and found no remaining mismatch.
