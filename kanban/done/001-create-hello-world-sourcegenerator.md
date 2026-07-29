# Create Hello World Source Generator

## Description
Create a minimal source generator that outputs diagnostic information to verify the source generation pipeline is working correctly.

## Steps
1. Create Project
   - [x] Create new source generator project: `dotnet new classlib -n TimeWarp.SourceGenerators`
   - [x] Add required source generator package references to .csproj
   - [x] Add to solution: `dotnet sln add TimeWarp.SourceGenerators`

2. Implement Basic Generator
   - [x] Create HelloWorldGenerator class implementing IIncrementalGenerator
   - [x] Add minimal diagnostic output in Initialize method
   - [x] Add simple source generation in Execute method

## Acceptance Criteria
- [x] Project builds successfully
    ```shell
    dotnet build Source/TimeWarp.SourceGenerators
    ```
- [x] Diagnostic information appears in build output
    - Info diagnostic TW0001: "The HelloWorld generator has been loaded and initialized"
    - Note: Info level diagnostics are only visible in Visual Studio's Output Window or Error List
    - Only Warning level and above appear in command-line build output
- [x] Generated source code is visible in IDE
    - Look for generated file: HelloWorld.g.cs
    - Should contain TimeWarp.Generated.HelloWorld class

## Notes
- To verify the generator:
  - In Visual Studio: Check Output Window or Error List for TW0001 diagnostic
  - From CLI: 
    1. Check for generated HelloWorld.g.cs file
    2. Run test console app to verify the generator works
- The generated HelloWorld.g.cs file should be visible in Solution Explorer under Dependencies > Analyzers
This is a minimal proof-of-concept. Additional capabilities will be added in subsequent tasks.
