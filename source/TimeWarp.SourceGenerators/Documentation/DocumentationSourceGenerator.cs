#nullable enable

namespace TimeWarp.SourceGenerators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

[Generator(LanguageNames.CSharp)]
public sealed class MarkdownDocsGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Create a provider for finding markdown files
    IncrementalValuesProvider<AdditionalText> markdownFiles = context.AdditionalTextsProvider
      .Where(file => file.Path.EndsWith(".cs.md", StringComparison.OrdinalIgnoreCase));

    // Add diagnostics for markdown files found
    context.RegisterSourceOutput(
      markdownFiles,
      (sourceProductionContext, file) =>
        sourceProductionContext.ReportDiagnostic(
          Diagnostic.Create(
            new DiagnosticDescriptor(
              id: "MD2DOC002",
              title: "Found markdown file",
              messageFormat: "Found markdown file: {0}",
              category: "Documentation",
              defaultSeverity: DiagnosticSeverity.Info,
              isEnabledByDefault: true
            ),
            location: Location.None,
            messageArgs: new[] { file.Path }
          )
        )
    );

    // Get all C# source files in the compilation
    IncrementalValuesProvider<(AdditionalText Left, Compilation Right)> csFiles = markdownFiles
      .Combine(context.CompilationProvider);

    // Register the source output
    context.RegisterSourceOutput(
      csFiles,
      (sourceProductionContext, pair) => Execute(sourceProductionContext, pair.Left, pair.Right));
  }

  private static void Execute(SourceProductionContext context, AdditionalText markdownFile, Compilation compilation)
  {
    // Log that we're starting to process a file
    context.ReportDiagnostic(
      Diagnostic.Create(
        new DiagnosticDescriptor(
          id: "MD2DOC003",
          title: "Processing markdown file",
          messageFormat: "Starting to process: {0}",
          category: "Documentation",
          defaultSeverity: DiagnosticSeverity.Info,
          isEnabledByDefault: true
        ),
        location: Location.None,
        messageArgs: new[] { markdownFile.Path }
      )
    );

    // Get the content of the markdown file
    SourceText? sourceText = markdownFile.GetText(context.CancellationToken);
    if (sourceText is null)
    {
      context.ReportDiagnostic(
        Diagnostic.Create(
          new DiagnosticDescriptor(
            id: "MD2DOC004",
            title: "Failed to read file",
            messageFormat: "Could not read content from: {0}",
            category: "Documentation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
          ),
          location: Location.None,
          messageArgs: new[] { markdownFile.Path }
        )
      );
      return;
    }

    string markdownContent = sourceText.ToString();
    if (string.IsNullOrEmpty(markdownContent))
    {
      context.ReportDiagnostic(
        Diagnostic.Create(
          new DiagnosticDescriptor(
            id: "MD2DOC005",
            title: "Empty file",
            messageFormat: "File is empty: {0}",
            category: "Documentation",
            defaultSeverity: DiagnosticSeverity.Warning,
            isEnabledByDefault: true
          ),
          location: Location.None,
          messageArgs: new[] { markdownFile.Path }
        )
      );
      return;
    }

    string csFilePath = markdownFile.Path.Substring(0, markdownFile.Path.Length - 3); // Remove .md from .cs.md
    string className = Path.GetFileNameWithoutExtension(csFilePath);

    try
    {
      string documentation = GenerateDocumentation(markdownContent);
      context.AddSource(
        $"{className}.Documentation.g.cs",
        SourceText.From(documentation, Encoding.UTF8)
      );

      // Log successful generation
      context.ReportDiagnostic(
        Diagnostic.Create(
          new DiagnosticDescriptor(
            id: "MD2DOC006",
            title: "Documentation generated",
            messageFormat: "Successfully generated documentation for: {0}",
            category: "Documentation",
            defaultSeverity: DiagnosticSeverity.Info,
            isEnabledByDefault: true
          ),
          location: Location.None,
          messageArgs: new[] { className }
        )
      );
    }
    catch (Exception ex)
    {
      context.ReportDiagnostic(
        Diagnostic.Create(
          new DiagnosticDescriptor(
            id: "MD2DOC001",
            title: "Documentation Generation Error",
            messageFormat: "Failed to generate documentation for {0}: {1}",
            category: "Documentation",
            defaultSeverity: DiagnosticSeverity.Error,
            isEnabledByDefault: true
          ),
          location: Location.None,
          messageArgs: new[] { className, ex.Message }
        )
      );
    }
  }

  private static string GenerateDocumentation(string markdown)
  {
    string[] lines = markdown.Split(new[] { '\n' }, StringSplitOptions.None);
    var docs = new StringBuilder();
    string currentSection = string.Empty;
    bool inCodeBlock = false;
    var codeBlockContent = new StringBuilder();

    docs.AppendLine("// <auto-generated/>");
    docs.AppendLine("// Generated by TimeWarp.SourceGenerators.MarkdownDocsGenerator");
    docs.AppendLine("#nullable enable");
    docs.AppendLine();
    docs.AppendLine("namespace TimeWarp.SourceGenerators;");
    docs.AppendLine();

    foreach (string line in lines)
    {
      string trimmedLine = line.Trim();

      // Handle code blocks
      if (trimmedLine.StartsWith("```", StringComparison.Ordinal))
      {
        if (inCodeBlock)
        {
          // End of code block - add the content
          docs.AppendLine("/// <code>");
          foreach (string codeLine in codeBlockContent.ToString().Split(new[] { '\n' }, StringSplitOptions.None))
          {
            docs.AppendLine($"/// {codeLine.TrimEnd()}");
          }
          docs.AppendLine("/// </code>");
          codeBlockContent.Clear();
        }
        inCodeBlock = !inCodeBlock;
        continue;
      }

      if (inCodeBlock)
      {
        codeBlockContent.AppendLine(trimmedLine);
        continue;
      }

      // Skip empty lines outside code blocks
      if (string.IsNullOrWhiteSpace(trimmedLine))
        continue;

      // Handle headers as section markers
      if (trimmedLine.StartsWith("#", StringComparison.Ordinal))
      {
        if (currentSection != string.Empty)
        {
          // Close previous section if exists
          docs.AppendLine($"/// </{currentSection}>");
        }
        currentSection = ParseHeaderToXmlTag(trimmedLine);
        docs.AppendLine($"/// <{currentSection}>");
        continue;
      }

      // Convert markdown line to XML doc comment
      string xmlLine = ConvertMarkdownToXml(trimmedLine);
      docs.AppendLine($"/// {xmlLine}");
    }

    // Close the last section if needed
    if (currentSection != string.Empty)
    {
      docs.AppendLine($"/// </{currentSection}>");
    }

    return docs.ToString();
  }

  private static string ParseHeaderToXmlTag(string header)
  {
    int level = header.TakeWhile(c => c == '#').Count();
    string text = header.Substring(level).Trim().ToLowerInvariant();

    return text switch
    {
      "summary" => "summary",
      "parameters" => "param",
      "returns" => "returns",
      "remarks" => "remarks",
      "examples" => "example",
      "exceptions" => "exception",
      "see also" => "seealso",
      _ => "remarks"
    };
  }

  private static string ConvertMarkdownToXml(string markdown)
  {
    string xml = markdown;

    // Handle markdown links
    xml = System.Text.RegularExpressions.Regex.Replace(
      xml,
      @"\[([^\]]+)\]\(([^\)]+)\)",
      "<see href=\"$2\">$1</see>"
    );

    // Handle inline code
    xml = System.Text.RegularExpressions.Regex.Replace(
      xml,
      @"`([^`]+)`",
      "<c>$1</c>"
    );

    // Handle parameters
    if (xml.IndexOf(':') != -1)
    {
      string[] parts = xml.Split(new[] { ':' }, 2, StringSplitOptions.None);
      if (parts.Length == 2)
      {
        return $"<param name=\"{parts[0].Trim()}\">{parts[1].Trim()}</param>";
      }
    }

    return xml;
  }
}
