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
        IncrementalValuesProvider<AdditionalText> markdownFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

        // Combine .cs and .md files that have matching names
        var combined = context.CompilationProvider.Combine(markdownFiles.Collect());

        // Generate documentation for each pair
        context.RegisterSourceOutput(combined, (sourceContext, pair) =>
        {
            var (compilation, markdownTexts) = pair;
            
            foreach (var markdownFile in markdownTexts)
            {
                var mdPath = markdownFile.Path;
                var csPath = Path.ChangeExtension(mdPath, ".cs");
                
                // Only process if corresponding .cs file exists
                if (!File.Exists(csPath)) continue;

                var className = Path.GetFileNameWithoutExtension(csPath);
                var markdownContent = markdownFile.GetText()?.ToString() ?? string.Empty;
                
                // Convert markdown content to comments
                var commentedContent = ConvertToComments(markdownContent);
                
                // Generate the documentation file
                var sourceText = SourceText.From($@"// Auto-generated documentation for {className}
{commentedContent}
", Encoding.UTF8);

                sourceContext.AddSource($"{className}.docs.g.cs", sourceText);
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
