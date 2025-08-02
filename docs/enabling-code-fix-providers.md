# Enabling Code Fix Providers in the TimeWarp Source Generators Project

## Current State
The project currently supports source generators and diagnostic analyzers but not code fix providers. This is because code fix providers require additional dependencies and configuration.

## What's Missing

### 1. Required NuGet Packages
The project needs additional packages for code fix providers:
- `Microsoft.CodeAnalysis.CSharp.Workspaces` - Provides workspace APIs for code fixes
- `Microsoft.CodeAnalysis.Workspaces.Common` - Common workspace functionality
- Possibly `System.Composition` - For MEF composition attributes

### 2. Package References
Add to `Directory.Packages.props`:
```xml
<PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.11.0" />
<PackageVersion Include="Microsoft.CodeAnalysis.Workspaces.Common" Version="4.11.0" />
```

Add to the project file or `Directory.Build.props`:
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" />
  <PackageReference Include="Microsoft.CodeAnalysis.Workspaces.Common" />
</ItemGroup>
```

### 3. Global Usings
Add these namespaces to `global-usings.cs`:
```csharp
global using System.Composition;
global using Microsoft.CodeAnalysis.CodeActions;
global using Microsoft.CodeAnalysis.CodeFixes;
```

### 4. Project Configuration
The project might need additional configuration:
- Ensure the output includes code fix providers in the analyzer package
- May need to adjust the `IncludeBuildOutput` property

## Implementation Considerations

### 1. File System Access
Code fix providers that create new files (like creating markdown files) face challenges:
- They run in the IDE/compiler context, not the file system directly
- Creating files outside the solution requires special handling
- Consider using `AdditionalFiles` or solution-level operations

### 2. Alternative Approach
Instead of directly creating files, consider:
- Generate the markdown content and show it to the user
- Create a "paste markdown" action that the user can apply
- Use the workspace API to add the file to the project

### 3. Simplified Implementation
For a first version, the code fix could:
1. Extract XML documentation from the source
2. Convert it to markdown format
3. Remove the XML docs from the source file
4. Display the markdown content for the user to save manually

## Example Code Fix Structure
```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XmlDocsToMarkdownCodeFixProvider)), Shared]
public class XmlDocsToMarkdownCodeFixProvider : CodeFixProvider
{
    // Implementation that:
    // 1. Extracts XML docs
    // 2. Converts to markdown
    // 3. Removes from source
    // 4. Provides the markdown as a code action result
}
```

## Testing Considerations
- Code fix providers need different testing infrastructure
- Consider using the Microsoft.CodeAnalysis.Testing packages
- Test both the analyzer and fix provider together

## Next Steps
1. Decide if code fix providers are needed for this project
2. If yes, add the required dependencies
3. Implement a simplified version first
4. Consider file creation strategies
5. Add appropriate tests