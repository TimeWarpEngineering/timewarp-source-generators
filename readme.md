[![Dotnet](https://img.shields.io/badge/dotnet-6.0-blue)](https://dotnet.microsoft.com)
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

TimeWarp.SourceGenerators is our collection of source generators.

## Give a Star! :star:

If you like or are using this project please give it a star. Thank you!

## Features

### Interface Delegation Generator

Implements Delphi-style interface delegation for C#. Mark fields or properties with `[Implements]` to automatically generate forwarding methods for interface members.

#### Usage

```csharp
public partial class DataService : ILogger, IDataProcessor<string>
{
    [Implements]
    private readonly ILogger Logger;

    [Implements]
    private readonly IDataProcessor<string> Processor;

    public DataService(ILogger logger, IDataProcessor<string> processor)
    {
        Logger = logger;
        Processor = processor;
    }

    // Optionally override specific methods
    public string Process(string input)
    {
        // Custom implementation
        return Processor.Process(input.ToUpper());
    }
}
```

The generator will automatically create forwarding implementations for all interface methods and properties, except those you explicitly implement.

#### Requirements

- Class must be marked as `partial`
- Class must implement the interface being delegated
- Field/property type must be the interface or implement the interface

#### Diagnostics

- **TW1001**: Class must be partial for interface delegation
- **TW1002**: Class does not implement the delegated interface
- **TW1003**: Multiple fields delegate the same interface

### File Name Rule Analyzer

Enforces kebab-case naming convention for C# files.

#### Configuration

Configure exceptions in `.editorconfig`:

```ini
[*.cs]
dotnet_diagnostic.TWA001.excluded_files = Program.cs;Startup.cs;*.Designer.cs
```

## Getting started

To quickly get started I recommend reviewing the samples in this repo.

## Installation

```console
dotnet add package TimeWarp.SourceGenerators
```

You can see the latest NuGet packages from the official [TimeWarp NuGet page](https://www.nuget.org/profiles/TimeWarp.Enterprises).

* [TimeWarp.SourceGenerators](https://www.nuget.org/packages/TimeWarp.SourceGenerators/) [![nuget](https://img.shields.io/nuget/v/TimeWarp.SourceGenerators?logo=nuget)](https://www.nuget.org/packages/TimeWarp.SourceGenerators/)

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