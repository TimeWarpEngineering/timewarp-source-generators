namespace TimeWarp.SourceGenerators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class XmlDocsToMarkdownAnalyzer : DiagnosticAnalyzer
{
    public const string DiagnosticId = "TWA002";
    private const string Category = "Documentation";

    private static readonly DiagnosticDescriptor Rule = new(
        DiagnosticId,
        "Move XML documentation to markdown file",
        "XML documentation for '{0}' should be moved to a markdown file",
        Category,
        DiagnosticSeverity.Info,
        isEnabledByDefault: true,
        description: "XML documentation comments should be moved to separate markdown files for better maintainability."
    );

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        // This already excludes generated code from analysis
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // Register for class declarations
        context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeInterfaceDeclaration, SyntaxKind.InterfaceDeclaration);
    }

    private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
    {
        var classDeclaration = (ClassDeclarationSyntax)context.Node;
        AnalyzeTypeDeclaration(context, classDeclaration, classDeclaration.Identifier);
    }

    private void AnalyzeInterfaceDeclaration(SyntaxNodeAnalysisContext context)
    {
        var interfaceDeclaration = (InterfaceDeclarationSyntax)context.Node;
        AnalyzeTypeDeclaration(context, interfaceDeclaration, interfaceDeclaration.Identifier);
    }

    private void AnalyzeTypeDeclaration(SyntaxNodeAnalysisContext context, TypeDeclarationSyntax typeDeclaration, SyntaxToken identifier)
    {
        // Check if the type has XML documentation
        var trivia = typeDeclaration.GetLeadingTrivia();
        var hasXmlDocs = trivia.Any(t => t.HasStructure && t.GetStructure() is DocumentationCommentTriviaSyntax);

        if (!hasXmlDocs)
        {
            // Also check members for XML documentation
            foreach (var member in typeDeclaration.Members)
            {
                var memberTrivia = member.GetLeadingTrivia();
                if (memberTrivia.Any(t => t.HasStructure && t.GetStructure() is DocumentationCommentTriviaSyntax))
                {
                    hasXmlDocs = true;
                    break;
                }
            }
        }

        if (hasXmlDocs)
        {
            // Report diagnostic on the type name
            var diagnostic = Diagnostic.Create(Rule, identifier.GetLocation(), identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }
}