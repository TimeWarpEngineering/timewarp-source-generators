using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Text.RegularExpressions;

namespace DocumentationGenerator;

[Generator]
public class MarkdownDocumentationGenerator : ISourceGenerator
{
  public void Initialize(GeneratorInitializationContext context)
  {
    // No initialization required
  }

  public void Execute(GeneratorExecutionContext context)
  {
    Dictionary<string, string> mdContents = GetMarkdownContents(context.AdditionalFiles);
    HashSet<string> processedMdFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

    foreach (SyntaxTree syntaxTree in context.Compilation.SyntaxTrees)
    {
      string csFileName = Path.GetFileName(syntaxTree.FilePath);
      string mdFileName = Path.ChangeExtension(csFileName, ".md");

      if (mdContents.TryGetValue(mdFileName, out string mdContent))
      {
        processedMdFiles.Add(mdFileName);

        SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
        SyntaxNode root = syntaxTree.GetRoot();

        SyntaxNode newRoot = ProcessMembers(root, semanticModel, mdContent);

        context.AddSource(
          $"{Path.GetFileNameWithoutExtension(csFileName)}_Generated.cs",
          SourceText.From(newRoot.ToFullString(), Encoding.UTF8)
        );
      }
    }

    // Log warnings for Markdown files without corresponding .cs files
    foreach (var mdFile in mdContents.Keys)
    {
      if (!processedMdFiles.Contains(mdFile))
      {
        context.ReportDiagnostic(Diagnostic.Create(
          new DiagnosticDescriptor(
            "MW001",
            "Unprocessed Markdown file",
            "Markdown file '{0}' does not have a corresponding .cs file and was not processed.",
            "MarkdownWarning",
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true),
          Location.None,
          mdFile));
      }
    }
  }

  private static Dictionary<string, string> GetMarkdownContents(IEnumerable<AdditionalText> additionalFiles)
  {
    return additionalFiles
      .Where(file => Path.GetExtension(file.Path).Equals(".md", StringComparison.OrdinalIgnoreCase))
      .ToDictionary(
        file => Path.GetFileName(file.Path),
        file => file.GetText()?.ToString() ?? string.Empty,
        StringComparer.OrdinalIgnoreCase  // Use case-insensitive comparison for file names
      );
  }

  private static SyntaxNode ProcessMembers(SyntaxNode root, SemanticModel semanticModel, string mdContent)
  {
    return root.ReplaceNodes(
      root.DescendantNodes().OfType<MemberDeclarationSyntax>(),
      (node, _) => ProcessMember(node, semanticModel, mdContent)
    );
  }

  private static SyntaxNode ProcessMember(MemberDeclarationSyntax member, SemanticModel semanticModel, string mdContent)
  {
    ISymbol symbol = semanticModel.GetDeclaredSymbol(member);
    if (symbol == null)
    {
      return member;
    }

    MarkdownDocumentation documentation = FindDocumentationForSymbol(symbol, mdContent);
    if (documentation == null)
    {
      return member;
    }

    SyntaxTriviaList newLeadingTrivia = member.GetLeadingTrivia()
      .Add(SyntaxFactory.Trivia(
        SyntaxFactory.DocumentationCommentTrivia(
          SyntaxKind.SingleLineDocumentationCommentTrivia,
          SyntaxFactory.List(GenerateDocumentationNodes(documentation))
        )
      ));

    return member.WithLeadingTrivia(newLeadingTrivia);
  }

  private static MarkdownDocumentation FindDocumentationForSymbol(ISymbol symbol, string mdContent)
  {
    string fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    string[] parts = fullName.Split('.');

    for (int i = parts.Length; i > 0; i--)
    {
      string sectionName = string.Join(".", parts.Skip(parts.Length - i));
      MarkdownDocumentation documentation = ParseDocumentation(mdContent, sectionName);
      if (documentation != null)
      {
        return documentation;
      }
    }

    return null;
  }

  private static MarkdownDocumentation ParseDocumentation(string content, string targetSection)
  {
    string[] lines = content.Split('\n');
    StringBuilder summary = new StringBuilder();
    List<(string name, string description)> parameters = new List<(string name, string description)>();
    string returns = null;

    bool inTargetSection = false;
    foreach (string line in lines)
    {
      if (line.StartsWith("## "))
      {
        if (inTargetSection)
        {
          break;
        }
        string currentSection = line.Substring(3).Trim();
        inTargetSection = currentSection == targetSection;
      }
      else if (inTargetSection)
      {
        if (line.StartsWith("- Parameter "))
        {
          Match match = Regex.Match(line, @"- Parameter (\w+): (.+)");
          if (match.Success)
          {
            parameters.Add((match.Groups[1].Value, match.Groups[2].Value));
          }
        }
        else if (line.StartsWith("- Returns: "))
        {
          returns = line.Substring("- Returns: ".Length).Trim();
        }
        else if (!string.IsNullOrWhiteSpace(line))
        {
          summary.AppendLine(line);
        }
      }
    }

    return inTargetSection
      ? new MarkdownDocumentation(summary.ToString().Trim(), parameters, returns)
      : null;
  }

  private static IEnumerable<XmlNodeSyntax> GenerateDocumentationNodes(MarkdownDocumentation documentation)
  {
    yield return SyntaxFactory.XmlText("/// ");
    yield return SyntaxFactory.XmlElement(
      "summary",
      SyntaxFactory.SingletonList<XmlNodeSyntax>(
        SyntaxFactory.XmlText(ConvertMarkdownToXml(documentation.Summary))
      )
    );

    foreach ((string name, string description) in documentation.Parameters)
    {
      yield return SyntaxFactory.XmlNewLine("\n");
      yield return SyntaxFactory.XmlText("/// ");

      XmlNameSyntax xmlName = SyntaxFactory.XmlName("param");
      XmlNameAttributeSyntax nameAttribute = SyntaxFactory.XmlNameAttribute(name);

      XmlElementStartTagSyntax startTag = SyntaxFactory.XmlElementStartTag(xmlName)
        .WithAttributes(SyntaxFactory.SingletonList<XmlAttributeSyntax>(nameAttribute));

      XmlElementEndTagSyntax endTag = SyntaxFactory.XmlElementEndTag(xmlName);

      yield return SyntaxFactory.XmlElement(
        startTag,
        SyntaxFactory.SingletonList<XmlNodeSyntax>(
          SyntaxFactory.XmlText(ConvertMarkdownToXml(description))
        ),
        endTag
      );
    }

    if (!string.IsNullOrEmpty(documentation.Returns))
    {
      yield return SyntaxFactory.XmlNewLine("\n");
      yield return SyntaxFactory.XmlText("/// ");
      yield return SyntaxFactory.XmlElement(
        "returns",
        SyntaxFactory.SingletonList<XmlNodeSyntax>(
          SyntaxFactory.XmlText(ConvertMarkdownToXml(documentation.Returns))
        )
      );
    }
  }

  private static string ConvertMarkdownToXml(string markdown)
  {
    markdown = Regex.Replace(markdown, @"`(.+?)`", "<c>$1</c>");
    markdown = Regex.Replace(markdown, @"\*\*(.+?)\*\*", "<b>$1</b>");
    markdown = Regex.Replace(markdown, @"\*(.+?)\*", "<i>$1</i>");
    return markdown;
  }
}

public class MarkdownDocumentation
{
  public string Summary { get; }
  public List<(string name, string description)> Parameters { get; }
  public string Returns { get; }

  public MarkdownDocumentation(string summary, List<(string name, string description)> parameters, string returns)
  {
    Summary = summary;
    Parameters = parameters;
    Returns = returns;
  }
}
