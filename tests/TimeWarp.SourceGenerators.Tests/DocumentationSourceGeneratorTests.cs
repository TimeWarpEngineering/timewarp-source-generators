using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using TimeWarp.SourceGenerators.Tests.Infrastructure;
using Xunit;

namespace TimeWarp.SourceGenerators.Tests;

public class DocumentationSourceGeneratorTests
{
    [Fact]
    public void Should_Generate_Documentation_From_Markdown()
    {
        // Arrange
        var source = @"
namespace TestNamespace;

public class TestClass 
{
    public void TestMethod() { }
}";

        var markdownContent = @"# TestClass

This is a test class that demonstrates documentation generation.

## TestMethod

This method is used for testing the documentation generator.";

        var compilation = CSharpCompilation.Create(
            assemblyName: "TestAssembly",
            syntaxTrees: new[] { CSharpSyntaxTree.ParseText(source) },
            references: SourceGeneratorTestHelper.DefaultReferences
        );

        // Create AdditionalText for markdown file
        var markdownFile = new CustomAdditionalText("TestClass.cs.md", markdownContent);

        var generator = new DocumentationSourceGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(
            generators: new[] { generator },
            additionalTexts: new[] { markdownFile },
            parseOptions: compilation.SyntaxTrees.First().Options as CSharpParseOptions
        );

        // Act
        driver = driver.RunGenerators(compilation);

        // Assert
        var runResult = driver.GetRunResult();
        var generatedFiles = runResult.GeneratedTrees;

        Assert.Single(generatedFiles);
        var generatedSource = generatedFiles[0].ToString();
        
        Assert.Contains("/// <summary>", generatedSource);
        Assert.Contains("This is a test class that demonstrates documentation generation.", generatedSource);
        Assert.Contains("This method is used for testing the documentation generator.", generatedSource);
    }
}

// Helper class to create AdditionalText for testing
public class CustomAdditionalText : AdditionalText
{
    private readonly string _text;
    public override string Path { get; }

    public CustomAdditionalText(string path, string text)
    {
        Path = path;
        _text = text;
    }

    public override SourceText GetText(CancellationToken cancellationToken = default)
    {
        return SourceText.From(_text);
    }
}
