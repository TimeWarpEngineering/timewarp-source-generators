namespace TimeWarp.SourceGenerators;

[Generator]
public partial class MarkdownDocsGenerator : IIncrementalGenerator
{
  // Regex pattern for checking if a file name is kebab-case
  private static readonly Regex KebabCasePattern = new(@"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$", RegexOptions.Compiled);
  private static readonly DiagnosticDescriptor MarkdownDocsGeneratorLoadedDescriptor = new(
      id: "TW0003",
      title: "MarkdownDocs Generator Loaded",
      messageFormat: "The MarkdownDocs generator has been loaded and initialized",
      category: "SourceGenerator",
      DiagnosticSeverity.Info,
      isEnabledByDefault: true
  );

  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Report initialization diagnostic
    IncrementalValueProvider<bool> initializationTrigger = context.CompilationProvider
        .Select((compilation, _) => true);

    context.RegisterSourceOutput(initializationTrigger, (sourceContext, _) =>
    {
      sourceContext.ReportDiagnostic(
              Diagnostic.Create(MarkdownDocsGeneratorLoadedDescriptor, Location.None)
          );
    });

    // Find all class declarations in C# files
    IncrementalValuesProvider<(ClassDeclarationSyntax ClassDeclaration, string? Namespace, string FilePath)> classDeclarations =
        context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (s, _) => s is ClassDeclarationSyntax,
                transform: (ctx, _) =>
                {
                  var classDeclaration = (ClassDeclarationSyntax)ctx.Node;
                  var namespaceDecl = classDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
                  var filePath = ctx.Node.SyntaxTree.FilePath;
                  return (classDeclaration, namespaceDecl?.Name.ToString(), filePath);
                });

    // Find all .md files
    IncrementalValuesProvider<AdditionalText> markdownFiles = context.AdditionalTextsProvider
        .Where(file => file.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

    // Group class declarations by class name to avoid duplicates
    var groupedClasses = classDeclarations
        .Collect()
        .Select((classes, _) => classes
            .GroupBy(c => (c.ClassDeclaration.Identifier.Text, c.Namespace))
            .Select(g => g.First())
            .ToArray());

    // Combine into pairs
    var pairs = groupedClasses.Combine(markdownFiles.Collect());

    // Generate documentation for matching pairs
    context.RegisterSourceOutput(pairs, (sourceContext, pair) =>
    {
      var (classGroups, markdownTexts) = pair;

      foreach (var (classDeclaration, namespaceName, filePath) in classGroups)
      {
        var className = classDeclaration.Identifier.Text;

        // Extract source file name without extension
        var sourceFileName = Path.GetFileNameWithoutExtension(filePath);

        // Find matching markdown file using appropriate strategy
        AdditionalText? matchingMd = null;

        // If source file is kebab-case, try to match kebab-case markdown first
        if (IsKebabCase(sourceFileName))
        {
          matchingMd = markdownTexts.FirstOrDefault(md =>
                  Path.GetFileNameWithoutExtension(md.Path).Equals(sourceFileName, StringComparison.OrdinalIgnoreCase));
        }

        // Fall back to class name match (for backward compatibility or PascalCase files)
        if (matchingMd == null)
        {
          matchingMd = markdownTexts.FirstOrDefault(md =>
                  Path.GetFileNameWithoutExtension(md.Path).Equals(className, StringComparison.OrdinalIgnoreCase));
        }

        // Also try kebab-case of the class name (PascalCase source + kebab markdown)
        if (matchingMd == null)
        {
          string kebabClassName = ConvertToKebabCase(className);
          matchingMd = markdownTexts.FirstOrDefault(md =>
                  Path.GetFileNameWithoutExtension(md.Path).Equals(kebabClassName, StringComparison.OrdinalIgnoreCase));
        }

        if (matchingMd != null)
        {
          var markdownContent = matchingMd.GetText()?.ToString() ?? string.Empty;
          var (classDocs, methodDocs, propertyDocs) = ConvertMarkdownToXmlDocs(markdownContent, classDeclaration);

          var sourceText = SourceText.From($@"// Auto-generated documentation for {className}
#nullable enable

{(namespaceName != null ? $"namespace {namespaceName};" : "")}

{classDocs}
public partial class {className}
{{
{propertyDocs}
{methodDocs}
}}
", Encoding.UTF8);

          sourceContext.AddSource($"{className}.docs.g.cs", sourceText);
        }
      }
    });
  }


  private static string ConvertToKebabCase(string pascalCase)
  {
    if (string.IsNullOrEmpty(pascalCase))
      return pascalCase;

    // Insert hyphens before uppercase letters (except the first character)
    var kebabCase = Regex.Replace(pascalCase, "(?<!^)([A-Z])", "-$1");

    // Convert to lowercase
    return kebabCase.ToLowerInvariant();
  }

  private static bool IsKebabCase(string fileName)
  {
    return KebabCasePattern.IsMatch(fileName);
  }
}
