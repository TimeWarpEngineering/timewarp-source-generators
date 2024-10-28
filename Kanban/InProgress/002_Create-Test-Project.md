# Create Test Project for Source Generator

## Description
Create a test console application that references the TimeWarp.SourceGenerators to verify the source generator is working correctly.

## Steps
1. Create Test Project
   - [x] Create new console project: `dotnet new console -n TimeWarp.SourceGenerators.TestConsole`
   - [x] Move project to Tests directory:
     ```powershell
     New-Item -ItemType Directory -Path Tests -ErrorAction SilentlyContinue
     Move-Item Source/TimeWarp.SourceGenerators.TestConsole Tests/
     ```
   - [x] Update solution reference:
     ```powershell
     dotnet sln remove Source/TimeWarp.SourceGenerators.TestConsole
     dotnet sln add Tests/TimeWarp.SourceGenerators.TestConsole
     ```
   - [x] Add project reference to TimeWarp.SourceGenerators

2. Verify Source Generator
   - [x] Add code that uses the generated HelloWorld class
   - [x] Build and run the test project
   
## Acceptance Criteria
- [x] Project builds successfully
- [x] Can access the generated HelloWorld.Message constant
- [x] Running the program displays the generated message

## Notes
- This will help verify both the source generation and diagnostic output are working correctly
- The test project provides a practical example of using the generator
