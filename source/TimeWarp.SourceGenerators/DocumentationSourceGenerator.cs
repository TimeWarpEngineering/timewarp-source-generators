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
    var csFiles = new HashSet<string>(
      context.Compilation.SyntaxTrees
        .Select(st => Path.GetFileNameWithoutExtension(st.FilePath)),
      StringComparer.OrdinalIgnoreCase
    );

    var mdContents = GetRelevantMarkdownContents(context.AdditionalFiles, csFiles);

    foreach (var syntaxTree in context.Compilation.SyntaxTrees)
    {
      string csFileNameWithoutExtension = Path.GetFileNameWithoutExtension(syntaxTree.FilePath);

      if (mdContents.TryGetValue(csFileNameWithoutExtension, out string mdContent))
      {
        SemanticModel semanticModel = context.Compilation.GetSemanticModel(syntaxTree);
        SyntaxNode root = syntaxTree.GetRoot();

        string documentationSource = GenerateDocumentationSource(root, semanticModel, mdContent);

        if (!string.IsNullOrWhiteSpace(documentationSource))
        {
          context.AddSource(
            $"{csFileNameWithoutExtension}.Documentation.cs",
            SourceText.From(documentationSource, Encoding.UTF8)
          );
        }
      }
    }
  }

  private string GenerateDocumentationSource(SyntaxNode root, SemanticModel semanticModel, string mdContent)
  {
    var documentationBuilder = new StringBuilder();

    foreach (var member in root.DescendantNodes().OfType<MemberDeclarationSyntax>())
    {
      var symbol = semanticModel.GetDeclaredSymbol(member);
      if (symbol != null)
      {
        var documentation = FindDocumentationForSymbol(symbol, mdContent);
        if (documentation != null)
        {
          string xmlDoc = GenerateXmlDocumentation(documentation);
          documentationBuilder.AppendLine(xmlDoc);
          documentationBuilder.AppendLine(member.ToFullString());
        }
      }
    }

    return documentationBuilder.ToString();
  }

  private static Dictionary<string, string> GetRelevantMarkdownContents(
    IEnumerable<AdditionalText> additionalFiles,
    HashSet<string> csFiles)
  {
    return additionalFiles
      .Where(file =>
        Path.GetExtension(file.Path).Equals(".md", StringComparison.OrdinalIgnoreCase) &&
        csFiles.Contains(Path.GetFileNameWithoutExtension(file.Path))
      )
      .ToDictionary(
        file => Path.GetFileNameWithoutExtension(file.Path),
        file => file.GetText()?.ToString() ?? string.Empty,
        StringComparer.OrdinalIgnoreCase
      );
  }

  private static SyntaxNode ProcessMembers(SyntaxNode root, SemanticModel semanticModel, string mdContent)
  {
    bool changes = false;
    var newRoot = root.ReplaceNodes(
      root.DescendantNodes().OfType<MemberDeclarationSyntax>(),
      (node, _) =>
      {
        var newNode = ProcessMember(node, semanticModel, mdContent);
        if (newNode != node)
        {
          changes = true;
        }
        return newNode;
      }
    );

    return changes ? newRoot : root;
  }

  private static SyntaxNode ProcessMember(MemberDeclarationSyntax member, SemanticModel semanticModel, string mdContent)
  {
    ISymbol symbol = semanticModel.GetDeclaredSymbol(member);
    if (symbol == null)
    {
      Console.WriteLine($"Warning: Unable to get symbol for member {member}");
      return member;
    }

    MarkdownDocumentation documentation = FindDocumentationForSymbol(symbol, mdContent);
    if (documentation == null)
    {
      Console.WriteLine($"Warning: No documentation found for symbol {symbol.ToDisplayString()}");
      return member;
    }

    Console.WriteLine($"Info: Found documentation for symbol {symbol.ToDisplayString()}");

    string xmlDoc = GenerateXmlDocumentation(documentation);
    SyntaxTriviaList newLeadingTrivia = member.GetLeadingTrivia()
      .Add(SyntaxFactory.SyntaxTrivia(SyntaxKind.SingleLineCommentTrivia, xmlDoc));

    return member.WithLeadingTrivia(newLeadingTrivia);
  }

  private static string GenerateXmlDocumentation(MarkdownDocumentation documentation)
  {
    var xmlBuilder = new StringBuilder();
    xmlBuilder.AppendLine("/// <summary>");
    xmlBuilder.AppendLine($"/// {documentation.Summary}");
    xmlBuilder.AppendLine("/// </summary>");

    foreach (var param in documentation.Parameters)
    {
      xmlBuilder.AppendLine($"/// <param name=\"{param.name}\">{param.description}</param>");
    }

    if (!string.IsNullOrEmpty(documentation.Returns))
    {
      xmlBuilder.AppendLine($"/// <returns>{documentation.Returns}</returns>");
    }

    return xmlBuilder.ToString();
  }

  private static MarkdownDocumentation FindDocumentationForSymbol(ISymbol symbol, string mdContent)
  {
    string fullName = symbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat);
    string[] parts = fullName.Split('.');

    Console.WriteLine($"Searching for documentation for symbol: {fullName}");

    for (int i = parts.Length; i > 0; i--)
    {
      string sectionName = string.Join(".", parts.Skip(parts.Length - i));
      Console.WriteLine($"Checking section: {sectionName}");
      MarkdownDocumentation documentation = ParseDocumentation(mdContent, sectionName);
      if (documentation != null)
      {
        Console.WriteLine($"Found documentation for section: {sectionName}");
        return documentation;
      }
    }

    Console.WriteLine($"No documentation found for symbol: {fullName}");
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
