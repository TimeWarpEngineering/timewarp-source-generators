# Bump TimeWarp.SourceGenerators to 1.0.0-beta.12

## Description

`dev check-version` refuses to release because `1.0.0-beta.11` is already on
NuGet. Bump `<Version>` to `1.0.0-beta.12` and record what consumers get
since beta.11: the delegation generator and the XML-docs markdown pipeline
are gone. Live diagnostics are TW0001 and TW0007.

## Checklist

- [x] `source/Directory.Build.props` version is `1.0.0-beta.12`
- [x] `documentation/releases.md` has a beta.12 section
- [x] `dev check-version` exits 0 (source 1.0.0-beta.12, NuGet 1.0.0-beta.11)
- [x] `ganda repo audit` exits 0 (advisory: missing gitignored `.vscode/settings.json`)
- [ ] Pull request merged and master CI green
- [ ] `dev release` from a clean synced master

## Session

- Created: grok `01a1bbd2-4185-7452-8535-389eb11a4f8f` (2026-11-09)
- Trigger: Steve asked to bump and release after check-version reported beta.11 already published

## Notes

Release is `dev release` from origin-home master after the bump is on master
and the master push CI run is green. Do not hand-tag.
