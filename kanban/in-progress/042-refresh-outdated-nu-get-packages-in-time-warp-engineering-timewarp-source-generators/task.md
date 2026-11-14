# Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators

## Description

Refresh outdated NuGet packages in TimeWarpEngineering/timewarp-source-generators.
Filter: (all)
Outdated:
  Microsoft.CodeAnalysis.Analyzers 5.9.0 -> 5.12.0 (minor)
  Microsoft.CodeAnalysis.CSharp 5.9.0 -> 5.12.0 (minor)
  Microsoft.CodeAnalysis.CSharp.Workspaces 5.9.0 -> 5.12.0 (minor)
  TimeWarp.Nuru 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
  TimeWarp.Nuru.DevCli 4.0.0-beta.12 -> 4.0.0-beta.13 (patch)
Edit Directory.Packages.props in this repo only.

Version: set <Version> in source/Directory.Build.props from 1.0.0-beta.15 to 1.0.0-beta.16 in this pull request. The prerelease suffix is -beta.N, never +beta.N. The wave releases it after merge.

## Session

- Minted by ganda task wave run 20261114T054552Z-9a0f39
