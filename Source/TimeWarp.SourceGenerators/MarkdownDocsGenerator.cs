using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Text;
using System.Text.RegularExpressions;

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

        // Find all class declarations in C# files
        IncrementalValuesProvider<(ClassDeclarationSyntax ClassDeclaration, string? Namespace)> classDeclarations = 
            context.SyntaxProvider
                .CreateSyntaxProvider(
                    predicate: (s, _) => s is ClassDeclarationSyntax,
                    transform: (ctx, _) => 
                    {
                        var classDeclaration = (ClassDeclarationSyntax)ctx.Node;
                        var namespaceDecl = classDeclaration.Ancestors().OfType<BaseNamespaceDeclarationSyntax>().FirstOrDefault();
                        return (classDeclaration, namespaceDecl?.Name.ToString());
                    });

        // Find all .md files
        IncrementalValuesProvider<AdditionalText> markdownFiles = context.AdditionalTextsProvider
            .Where(file => file.Path.EndsWith(".md", StringComparison.OrdinalIgnoreCase));

        // Combine into pairs
        var pairs = classDeclarations.Combine(markdownFiles.Collect());

        // Generate documentation for matching pairs
        context.RegisterSourceOutput(pairs, (sourceContext, pair) =>
        {
            var ((classDeclaration, namespaceName), markdownTexts) = pair;
            var className = classDeclaration.Identifier.Text;

            // Find matching markdown file
            var matchingMd = markdownTexts.FirstOrDefault(md => 
                Path.GetFileNameWithoutExtension(md.Path).Equals(className, StringComparison.OrdinalIgnoreCase));

            if (matchingMd != null)
            {
                var markdownContent = matchingMd.GetText()?.ToString() ?? string.Empty;
                var xmlDocs = ConvertMarkdownToXmlDocs(markdownContent);
                
                var sourceText = SourceText.From($@"// Auto-generated documentation for {className}
{(namespaceName != null ? $"namespace {namespaceName};" : "")}

{xmlDocs}
public partial class {className}
{{
}}
", Encoding.UTF8);

                sourceContext.AddSource($"{className}.docs.g.cs", sourceText);
            }
        });
    }

    private static string ConvertMarkdownToXmlDocs(string markdownContent)
    {
        var builder = new StringBuilder();
        var reader = new StringReader(markdownContent);
        string? line;
        
        // State tracking
        var currentSection = "";
        var contentBuilder = new StringBuilder();
        
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("# ")) // Class name - skip
                continue;
                
            if (line.StartsWith("## "))
            {
                // Process previous section
                ProcessSection(builder, currentSection, contentBuilder.ToString().Trim());
                
                // Start new section
                currentSection = line.Substring(3).Trim();
                contentBuilder.Clear();
                continue;
            }
            
            // If we haven't hit any section yet, this is the summary
            if (string.IsNullOrEmpty(currentSection) && !string.IsNullOrWhiteSpace(line))
            {
                builder.AppendLine("/// <summary>");
                builder.AppendLine($"/// {line.Trim()}");
                builder.AppendLine("/// </summary>");
                continue;
            }
            
            // Skip sections we don't want
            if (currentSection.StartsWith("Properties") || 
                currentSection.StartsWith("Methods") || 
                currentSection.StartsWith("Examples"))
                continue;
                
            // Add content to current section
            if (!string.IsNullOrWhiteSpace(line))
            {
                contentBuilder.AppendLine(line);
            }
        }
        
        // Process the last section
        ProcessSection(builder, currentSection, contentBuilder.ToString().Trim());
        
        return builder.ToString();
    }
    
    private static void ProcessSection(StringBuilder builder, string section, string content)
    {
        if (string.IsNullOrEmpty(content))
            return;
            
        switch (section)
        {
            case "Remarks":
                builder.AppendLine("/// <remarks>");
                foreach (var line in content.Split('\n'))
                {
                    builder.AppendLine($"/// {line.Trim()}");
                }
                builder.AppendLine("/// </remarks>");
                break;
                
            case "See Also":
                foreach (var line in content.Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var match = Regex.Match(line, @"\[(.*?)\]\((.*?)\)");
                    if (match.Success)
                    {
                        builder.AppendLine($"/// <seealso cref=\"{match.Groups[1].Value.Replace("<", "{").Replace(">", "}")}\"/>");
                    }
                }
                break;
                
            case "Inheritance":
                var inheritDoc = content.Contains("@") 
                    ? content.Substring(content.IndexOf("@") + 1).Split(' ')[0]
                    : null;
                if (inheritDoc != null)
                {
                    builder.AppendLine($"/// <inheritdoc cref=\"{inheritDoc.Replace("<", "{").Replace(">", "}")}\"/>");
                }
                break;
                
            case "References":
                foreach (var line in content.Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var match = Regex.Match(line, @"@(\S+)");
                    if (match.Success)
                    {
                        builder.AppendLine($"/// <see cref=\"{match.Groups[1].Value.Replace("<", "{").Replace(">", "}")}\"/>");
                    }
                }
                break;
        }
    }
}
