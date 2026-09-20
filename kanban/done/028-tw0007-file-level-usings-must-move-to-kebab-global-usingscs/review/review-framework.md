# Review framework — task 028

**Date:** 2026-09-20
**Host task:** kanban/in-progress/028-tw0007-file-level-usings-must-move-to-kebab-global-usingscs/
**Diff scope:** branch `task/028-tw0007-file-level-usings-must-move-to-kebab-global` vs `origin/master` (implement commit `b7a6221` feat(TW0007): add analyzer for namespace-first file-level usings). Uncommitted `.gitignore` is out of scope (unrelated local dirt).
**Plan / brief:** Ship TW0007 (`GlobalUsingsRuleAnalyzer`) as a DiagnosticAnalyzer that visits every `UsingDirectiveSyntax` (compilation unit, file-scoped namespace-first, block namespace). Skip `global using`, `using static`, aliases, generated files, the configured kebab `global-usings.cs`, and `excluded_files`. Disabled by default; no BDSoftware package; no code-fix.
**Effort:** 1 (general only)
**Reviewer roster:** general
**Session IDs:** Grok Build review oracle (ganda task-work review, 2026-09-20)

## Ground rules

- Reviewers are read-only on product code; they write only under `review/round-N/`
- Severity: bug | suggestion | nit — Status starts as open
- Do not invent issues to fill space; zero issues is a valid outcome
- Address the diff and surrounding call sites; re-verify falsifiable claims against the repo
- Prior rounds are immutable; new work goes in `round-(N+1)/`
