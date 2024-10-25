using FluentAssertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using TimeWarp.SourceGenerators;
using static TimeWarp.SourceCodeGenerators.Tests.Infrastructure.SourceGeneratorTestHelper;

namespace DocumentationSourceGenerator_;

public class Should_
{
    public void Generate_Documentation_From_Markdown()
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

### TestMethod

This method is used for testing the documentation generator.

Parameters:
- name: The name of the person to greet

Returns: A string containing the greeting message";

        var additionalTexts = new[]
        {
            new CustomAdditionalText("TestClass.cs.md", markdownContent)
        };

        // Act
        string output = GetGeneratedOutput<DocumentationSourceGenerator>(source, additionalTexts);

        // Assert
        output.Should().Contain("/// <summary>");
        output.Should().Contain("/// This is a test class that demonstrates documentation generation.");
        output.Should().Contain("/// This method is used for testing the documentation generator.");
        output.Should().Contain("/// <param name=\"name\">The name of the person to greet</param>");
        output.Should().Contain("/// <returns>A string containing the greeting message</returns>");
    }
}

// Helper class to create AdditionalText for testing
file class CustomAdditionalText : AdditionalText
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
