# Review framework — task 043

**Date:** 2026-11-16
**Host task:** kanban/in-progress/043-adopt-the-shared-pull-request-build-classifier-and-the-one-workflow-ci-standard/
**Diff scope:** branch `task/043-adopt-the-shared-pull-request-build-classifier-and` vs `master` (three-dot). Product paths: `.githooks/prepare-commit-msg`, `.githooks/prepare-commit-msg.cs`, `.github/workflows/workflow.yml`, `Directory.Packages.props`, `tools/dev-cli/endpoints/workflow-command.cs`. Kitchen move of `task.md` is in the same range. The branch is one commit behind `master` (`9d2ed0e`, a kanban land with no line changes).
**Plan / brief:** task 043 — drop the `pull_request` paths filter, call `PullRequestBuildClassifier.DecideAsync` from TimeWarp.Nuru.DevCli 4.0.0-beta.15, keep the `push` filter with a why comment, and declare any build inputs outside `source`, `tests`, `tools`, and `msbuild` through `pullRequestBuild`.
**Effort:** 2 (general only)
**Reviewer roster:** general
**Session IDs:** grok `01a1e4d6-a1d2-70a2-b465-9d6499d4324b`

## Budget (by-diff)

- Lines changed: 290
- Effort: 2
- TCB hits: none
- Roster axes: general
- Turn cap: 120 (--max-turns; cursor uncapped)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims by reading the diff and the kitchen `## Results` and `### How to validate` record
- Do not re-run a full build, a full test run, an audit, or the hooks; those belong to the implementer's recorded results, the host audit node, and CI
- Exception: one targeted command may check one suspected bug; the finding names the command and the reason. A full build, full test run, or audit is never that command
- A missing or thin recorded result is a finding, not a reason to run the gates
- Prior rounds are immutable; new work goes in `round-(N+1)/`
