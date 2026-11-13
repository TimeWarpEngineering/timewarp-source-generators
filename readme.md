[![Dotnet](https://img.shields.io/badge/dotnet-11.0-blue)](https://dotnet.microsoft.com)
[![Stars](https://img.shields.io/github/stars/TimeWarpEngineering/timewarp-source-generators?logo=github)](https://github.com/TimeWarpEngineering/timewarp-source-generators)
[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)
[![workflow](https://github.com/TimeWarpEngineering/timewarp-source-generators/actions/workflows/release-build.yml/badge.svg)](https://github.com/TimeWarpEngineering/timewarp-source-generators/actions)
[![nuget](https://img.shields.io/nuget/v/TimeWarp.SourceGenerators?logo=nuget)](https://www.nuget.org/packages/TimeWarp.SourceGenerators/)
[![nuget](https://img.shields.io/nuget/dt/TimeWarp.SourceGenerators?logo=nuget)](https://www.nuget.org/packages/TimeWarp.SourceGenerators/)
[![Issues Open](https://img.shields.io/github/issues/TimeWarpEngineering/timewarp-source-generators.svg?logo=github)](https://github.com/TimeWarpEngineering/timewarp-source-generators/issues)
[![Forks](https://img.shields.io/github/forks/TimeWarpEngineering/timewarp-source-generators)](https://github.com/TimeWarpEngineering/timewarp-source-generators)
[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-source-generators.svg?style=flat-square&logo=github)](https://github.com/TimeWarpEngineering/timewarp-source-generators/issues)
[![Twitter](https://img.shields.io/twitter/url?style=social&url=https%3A%2F%2Fgithub.com%2FTimeWarpEngineering%2Ftimewarp-source-generators)](https://twitter.com/intent/tweet?url=https://github.com/TimeWarpEngineering/timewarp-source-generators)

[![Twitter](https://img.shields.io/twitter/follow/StevenTCramer.svg)](https://twitter.com/intent/follow?screen_name=StevenTCramer)
[![Twitter](https://img.shields.io/twitter/follow/TheFreezeTeam1.svg)](https://twitter.com/intent/follow?screen_name=TheFreezeTeam1)

# TimeWarp.SourceGenerators

![TimeWarp Logo](assets/logo.png)

TimeWarp.SourceGenerators ships Roslyn analyzers for C# file names, global usings, and the generic style rules TWA0004, TWA0023, and TWA0038.

## Give a Star! :star:

If you like or are using this project please give it a star. Thank you!

## Features

### File Name Rule Analyzer

Enforces kebab-case naming for C# files, including multi-dot partials (e.g. `application-state.close-modal.cs`) when every segment is kebab-case.

#### Configuration

Configure exceptions in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.TW0001.excluded_files = Program.cs;Startup.cs;*.Designer.cs
```

### Global Usings Rule Analyzer (TW0007)

Reports file-level `using` directives that should move to kebab-case `global-usings.cs`, including usings **after** a file-scoped `namespace X;` (the gap in BDSoftware GlobalUsingsAnalyzer 1.4.0). Disabled by default.

```ini
[*.cs]
dotnet_diagnostic.TW0007.severity = warning
dotnet_diagnostic.TW0007.filename = global-usings.cs
```

See [configure-global-usings-analyzer.md](./documentation/developer/how-to-guides/configure-global-usings-analyzer.md).

### Generic rules moved from TimeWarp.Architecture.Analyzers

These ids are unchanged, so an existing `.editorconfig` entry still applies. Other `TWA*` ids stay in `TimeWarp.Architecture.Analyzers`.

| ID | Rule | Default |
|----|------|---------|
| **TWA0004** | `#region Purpose` in every non-generated C# file | Warning, enabled |
| **TWA0023** | Identifier ends with its type stem | Warning, disabled |
| **TWA0038** | File longer than 500 lines (`max_lines` overrides) | Warning, enabled |

```ini
[*.cs]
dotnet_diagnostic.TWA0004.severity = warning
dotnet_diagnostic.TWA0023.severity = none
dotnet_diagnostic.TWA0038.severity = warning
dotnet_diagnostic.TWA0038.max_lines = 500
```

TWA0023 opt-out is `[TypeStemIdentifier("reason")]`. The attribute type stays in `TimeWarp.Architecture.Attributes` and is matched by simple name.

### Diagnostic ID prefixes (SSOT)

| Prefix | Where it lives |
|--------|----------------|
| **TW** | **TimeWarp.SourceGenerators** — **TW0001** and **TW0007** |
| **TWA0004, TWA0023, TWA0038** | **TimeWarp.SourceGenerators** — generic style rules moved from Architecture.Analyzers. Same ids. |
| **other TWA\*** | **TimeWarp.Architecture.Analyzers** — contracts, endpoints, slices, and the rest |

File-name kebab-case is **TW0001** (`dotnet_diagnostic.TW0001.*`, `#pragma warning disable TW0001`). Do not configure that rule with a `TWA*` id.

## Getting started

To quickly get started I recommend reviewing the samples in this repo.

## Installation

```console
dotnet add package TimeWarp.SourceGenerators
```

You can see the latest NuGet packages from the official [TimeWarp NuGet page](https://www.nuget.org/profiles/TimeWarp.Enterprises).

* [TimeWarp.SourceGenerators](https://www.nuget.org/packages/TimeWarp.SourceGenerators/) [![nuget](https://img.shields.io/nuget/v/TimeWarp.SourceGenerators?logo=nuget)](https://www.nuget.org/packages/TimeWarp.SourceGenerators/)

## Requirements

The analyzer assembly targets `net11.0`, not `netstandard2.0`. It loads in a compiler that runs on .NET 11: `dotnet build` with the .NET 11 SDK, and editors that host that compiler. It does not load in Visual Studio, whose compiler runs on .NET Framework.

## Releases

See the [Release Notes](./documentation/releases.md)
## Unlicense

[![License](https://img.shields.io/github/license/TimeWarpEngineering/timewarp-source-generators.svg?style=flat-square&logo=github)](https://unlicense.org)

## Contributing

Time is of the essence.  Before developing a Pull Request I recommend opening a [discussion](https://github.com/TimeWarpEngineering/timewarp-source-generators/discussions).

Please feel free to make suggestions and help out with the [documentation](https://timewarpengineering.github.io/timewarp-source-generators/).
Please refer to [Markdown](http://daringfireball.net/projects/markdown/) for how to write markdown files.

## Contact

Sometimes the github notifications get lost in the shuffle.  If you file an [issue](https://github.com/TimeWarpEngineering/timewarp-source-generators/issues) and don't get a response in a timely manner feel free to ping on our [Discord server](https://discord.gg/A55JARGKKP).

[![Discord](https://img.shields.io/discord/715274085940199487?logo=discord)](https://discord.gg/7F4bS2T)