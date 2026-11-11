# Disposition — task 036

**Date:** 2026-11-11
**Outcome:** clean
**Rounds:** 2
**Final open count:** 0

## Summary

The .NET 11 GA move was confirmed in round 1. Round 1 raised one bug (M1): the `ci-code-coverage` audit check was silenced instead of satisfied. The fix pass added real Cobertura collection with pinned `dotnet-coverage` for both dev-cli console suites, wired it into the CI workflow command, uploaded `artifacts/coverage`, and removed the opt-out. Round 2 re-verified M1 as fixed (suite passes under coverage, audit 42/0 with `ci-code-coverage` PASS) and found one kitchen-only nit (M2, a misquoted line-rate in Results), which the reviewer corrected. Every raised finding is fixed.

## Exception log (if accepted-exceptions)

None.

## Escalations

- None.
