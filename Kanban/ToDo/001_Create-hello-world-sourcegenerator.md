# Create Hello World Source Generator

## Description
Create a minimal source generator that outputs diagnostic information to verify the source generation pipeline is working correctly.

## Steps
1. Create Project
   - [ ] Create new source generator project: `dotnet new classlib -n TimeWarp.SourceGenerators`
   - [ ] Add required source generator package references to .csproj
   - [ ] Add to solution: `dotnet sln add TimeWarp.SourceGenerators`

2. Implement Basic Generator
   - [ ] Create HelloWorldGenerator class implementing ISourceGenerator
   - [ ] Add minimal diagnostic output in Initialize method
   - [ ] Add simple source generation in Execute method

## Acceptance Criteria
- [ ] Project builds successfully
- [ ] Diagnostic information appears in build output
- [ ] Generated source code is visible in IDE

## Notes
This is a minimal proof-of-concept. Additional capabilities will be added in subsequent tasks.
