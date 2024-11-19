using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using System.Text;

namespace TimeWarp.SourceGenerators;

[Generator]
public class MarkdownDocsGenerator : IIncrementalGenerator
{
    private static readonly DiagnosticDescriptor MarkdownDocsGeneratorLoadedDescriptor = new(
        id: "TW0002",
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

        // Find all .cs files in the compilation
        IncrementalValuesProvider<SyntaxTree> csFiles = context.CompilationProvider
            .SelectMany((compilation, _) => compilation.SyntaxTrees);

        // Find all .md files
        IncrementalValuesProvider<AdditionalText> markdownFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

        // Combine into pairs
        var pairs = csFiles.Combine(markdownFiles.Collect());

        // Generate documentation for matching pairs
        context.RegisterSourceOutput(pairs, (sourceContext, pair) =>
        {
            var (csFile, markdownTexts) = pair;
            var csPath = csFile.FilePath;
            var csFileName = Path.GetFileNameWithoutExtension(csPath);

            // Find matching markdown file
            var matchingMd = markdownTexts.FirstOrDefault(md => 
                Path.GetFileNameWithoutExtension(md.Path).Equals(csFileName, StringComparison.OrdinalIgnoreCase));

            if (matchingMd != null)
            {
                var markdownContent = matchingMd.GetText()?.ToString() ?? string.Empty;
                
                // Convert markdown content to comments
                var commentedContent = ConvertToComments(markdownContent);
                
                // Generate the documentation file
                var sourceText = SourceText.From($@"// Auto-generated documentation for {csFileName}
{commentedContent}
", Encoding.UTF8);

                sourceContext.AddSource($"{csFileName}.docs.g.cs", sourceText);
            }
        });
    }

    private static string ConvertToComments(string markdownContent)
    {
        var builder = new StringBuilder();
        var reader = new StringReader(markdownContent);
        string? line;
        
        while ((line = reader.ReadLine()) != null)
        {
            builder.AppendLine($"// {line}");
        }
        
        return builder.ToString();
    }
}
