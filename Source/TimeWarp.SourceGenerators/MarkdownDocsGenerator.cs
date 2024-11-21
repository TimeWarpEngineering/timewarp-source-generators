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
                var (classDocs, methodDocs) = ConvertMarkdownToXmlDocs(markdownContent);
                
                var sourceText = SourceText.From($@"// Auto-generated documentation for {className}
{(namespaceName != null ? $"namespace {namespaceName};" : "")}

{classDocs}
public partial class {className}
{{
{methodDocs}
}}
", Encoding.UTF8);

                sourceContext.AddSource($"{className}.docs.g.cs", sourceText);
            }
        });
    }

    private static (string ClassDocs, string MethodDocs) ConvertMarkdownToXmlDocs(string markdownContent)
    {
        var classBuilder = new StringBuilder();
        var methodBuilder = new StringBuilder();
        var reader = new StringReader(markdownContent);
        string? line;
        
        // State tracking
        var currentSection = "";
        var contentBuilder = new StringBuilder();
        var inMethodSection = false;
        var currentMethod = "";
        var currentMethodDescription = "";
        
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("# ")) // Class name - skip
                continue;
                
            if (line.StartsWith("## "))
            {
                // Process previous section
                if (inMethodSection)
                    ProcessMethodSection(methodBuilder, currentMethod, currentMethodDescription, contentBuilder.ToString().Trim());
                else
                    ProcessSection(classBuilder, currentSection, contentBuilder.ToString().Trim());
                
                // Start new section
                currentSection = line.Substring(3).Trim();
                inMethodSection = currentSection == "Methods";
                contentBuilder.Clear();
                currentMethodDescription = "";
                continue;
            }

            if (line.StartsWith("### ") && inMethodSection)
            {
                // Process previous method if exists
                if (!string.IsNullOrEmpty(currentMethod))
                {
                    ProcessMethodSection(methodBuilder, currentMethod, currentMethodDescription, contentBuilder.ToString().Trim());
                }

                // Start new method
                currentMethod = line.Substring(4).Trim();
                contentBuilder.Clear();
                currentMethodDescription = "";
                continue;
            }
            
            // If we haven't hit any section yet, this is the class summary
            if (string.IsNullOrEmpty(currentSection) && !string.IsNullOrWhiteSpace(line))
            {
                classBuilder.AppendLine("/// <summary>");
                classBuilder.AppendLine($"/// {line.Trim()}");
                classBuilder.AppendLine("/// </summary>");
                continue;
            }

            // Capture method description (text before any #### subsections)
            if (inMethodSection && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("####") && string.IsNullOrEmpty(currentMethodDescription))
            {
                currentMethodDescription = line.Trim();
                continue;
            }
            
            // Add content to current section
            if (!string.IsNullOrWhiteSpace(line))
            {
                contentBuilder.AppendLine(line);
            }
        }
        
        // Process the last section
        if (inMethodSection)
            ProcessMethodSection(methodBuilder, currentMethod, currentMethodDescription, contentBuilder.ToString().Trim());
        else
            ProcessSection(classBuilder, currentSection, contentBuilder.ToString().Trim());
        
        return (classBuilder.ToString(), methodBuilder.ToString());
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
                        var crefValue = match.Groups[1].Value.Replace("<", "{").Replace(">", "}");
                        builder.AppendLine($"/// <seealso cref=\"{crefValue}\"/>");
                    }
                }
                break;
                
            case "Inheritance":
                var inheritDoc = content.Contains("@") 
                    ? content.Substring(content.IndexOf("@") + 1).Split(' ')[0]
                    : null;
                if (inheritDoc != null)
                {
                    var crefValue = inheritDoc.Replace("<", "{").Replace(">", "}");
                    builder.AppendLine($"/// <inheritdoc cref=\"{crefValue}\"/>");
                }
                break;
                
            case "References":
                foreach (var line in content.Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var match = Regex.Match(line, @"@(\S+)");
                    if (match.Success)
                    {
                        var crefValue = match.Groups[1].Value.Replace("<", "{").Replace(">", "}");
                        builder.AppendLine($"/// <see cref=\"{crefValue}\"/>");
                    }
                }
                break;
        }
    }

    private static void ProcessMethodSection(StringBuilder builder, string methodName, string description, string content)
    {
        if (string.IsNullOrEmpty(methodName))
            return;

        builder.AppendLine($"    // Documentation for {methodName}");

        // Add method summary if available
        if (!string.IsNullOrEmpty(description))
        {
            builder.AppendLine("    /// <summary>");
            builder.AppendLine($"    /// {description}");
            builder.AppendLine("    /// </summary>");
        }

        var reader = new StringReader(content);
        string? line;
        var currentSubSection = "";
        var subSectionContent = new StringBuilder();
        
        while ((line = reader.ReadLine()) != null)
        {
            if (line.StartsWith("#### "))
            {
                // Process previous subsection
                ProcessMethodSubSection(builder, currentSubSection, subSectionContent.ToString().Trim());
                
                // Start new subsection
                currentSubSection = line.Substring(5).Trim();
                subSectionContent.Clear();
                continue;
            }
            
            // Add content to current subsection
            if (!string.IsNullOrWhiteSpace(line))
            {
                subSectionContent.AppendLine(line);
            }
        }
        
        // Process the last subsection
        ProcessMethodSubSection(builder, currentSubSection, subSectionContent.ToString().Trim());
        
        builder.AppendLine();
    }

    private static void ProcessMethodSubSection(StringBuilder builder, string subSection, string content)
    {
        if (string.IsNullOrEmpty(content))
            return;

        switch (subSection)
        {
            case "Parameters":
                foreach (var line in content.Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var match = Regex.Match(line, @"`(.*?)`\s*-\s*(.*)");
                    if (match.Success)
                    {
                        builder.AppendLine($"    /// <param name=\"{match.Groups[1].Value}\">{match.Groups[2].Value.Trim()}</param>");
                    }
                }
                break;

            case "Returns":
                builder.AppendLine("    /// <returns>");
                foreach (var line in content.Split('\n'))
                {
                    builder.AppendLine($"    /// {line.Trim()}");
                }
                builder.AppendLine("    /// </returns>");
                break;

            case "Exceptions":
                foreach (var line in content.Split('\n'))
                {
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var match = Regex.Match(line, @"`(.*?)`\s*-\s*(.*)");
                    if (match.Success)
                    {
                        var exceptionType = match.Groups[1].Value.Replace("<", "{").Replace(">", "}");
                        builder.AppendLine($"    /// <exception cref=\"{exceptionType}\">{match.Groups[2].Value.Trim()}</exception>");
                    }
                }
                break;
        }
    }
}
