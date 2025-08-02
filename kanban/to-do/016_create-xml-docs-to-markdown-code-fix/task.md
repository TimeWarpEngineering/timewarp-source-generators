# Create XML Docs to Markdown Code Fix Provider

## Description
Create a code fix provider for the XmlDocsToMarkdownAnalyzer (TW0004) that extracts XML documentation from C# code and converts it to markdown files compatible with the MarkdownDocsGenerator.

## Acceptance Criteria
- [ ] Code fix provider responds to TW0004 diagnostics
- [ ] Extracts all XML documentation elements from type and members
- [ ] Converts XML to markdown following MarkdownDocsGenerator format
- [ ] Removes XML documentation from source code
- [ ] Handles file creation strategy (see implementation notes)
- [ ] Supports all XML doc elements (summary, param, returns, remarks, etc.)

## Prerequisites
The project currently lacks code fix provider support. Required changes:

### 1. Add NuGet Packages
Add to `Directory.Packages.props`:
```xml
<PackageVersion Include="Microsoft.CodeAnalysis.CSharp.Workspaces" Version="4.11.0" />
<PackageVersion Include="Microsoft.CodeAnalysis.Workspaces.Common" Version="4.11.0" />
```

Add to project references:
```xml
<ItemGroup>
  <PackageReference Include="Microsoft.CodeAnalysis.CSharp.Workspaces" />
  <PackageReference Include="Microsoft.CodeAnalysis.Workspaces.Common" />
</ItemGroup>
```

### 2. Update Global Usings
Add to `global-usings.cs`:
```csharp
global using System.Composition;
global using Microsoft.CodeAnalysis.CodeActions;
global using Microsoft.CodeAnalysis.CodeFixes;
```

## Implementation Notes

### File Creation Strategy
Since code fix providers run in IDE/compiler context:
- Option 1: Generate markdown content for user to save manually
- Option 2: Use workspace APIs to add file to project
- Option 3: Create temporary file and prompt user

### Simplified First Version
1. Extract XML documentation from source
2. Convert to markdown format
3. Remove XML docs from source file
4. Display markdown content for manual save

### Code Structure
- Use existing temporary file: `xml-docs-to-markdown-code-fix-provider.cs.temp`
- Implement `CodeFixProvider` base class
- Register for `TW0004` diagnostic ID
- Handle type and member documentation

## Technical Details
- Must handle partial classes appropriately
- Should preserve code structure when removing XML docs
- Follow existing kebab-case and PascalCase file naming patterns
- Coordinate with MarkdownDocsGenerator expected format

## Dependencies
- Requires project configuration for code fix providers
- XmlDocsToMarkdownAnalyzer (TW0004) must be working
- Understanding of MarkdownDocsGenerator format

## References
- See `docs/enabling-code-fix-providers.md` for setup details
- Existing code in `xml-docs-to-markdown-code-fix-provider.cs.temp`