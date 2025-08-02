namespace TimeWarp.SourceGenerators;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(XmlDocsToMarkdownCodeFixProvider)), Shared]
public class XmlDocsToMarkdownCodeFixProvider : CodeFixProvider
{
    private const string Title = "Move XML documentation to markdown file";

    public sealed override ImmutableArray<string> FixableDiagnosticIds => ImmutableArray.Create(XmlDocsToMarkdownAnalyzer.DiagnosticId);

    public sealed override FixAllProvider GetFixAllProvider() => WellKnownFixAllProviders.BatchFixer;

    public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root == null) return;

        var diagnostic = context.Diagnostics.First();
        var diagnosticSpan = diagnostic.Location.SourceSpan;

        // Find the type declaration identified by the diagnostic
        var typeDeclaration = root.FindToken(diagnosticSpan.Start).Parent?.AncestorsAndSelf()
            .OfType<TypeDeclarationSyntax>()
            .FirstOrDefault();

        if (typeDeclaration != null)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    title: Title,
                    createChangedSolution: c => MoveDocsToMarkdownAsync(context.Document, typeDeclaration, c),
                    equivalenceKey: Title),
                diagnostic);
        }
    }

    private async Task<Solution> MoveDocsToMarkdownAsync(Document document, TypeDeclarationSyntax typeDeclaration, CancellationToken cancellationToken)
    {
        var solution = document.Project.Solution;
        var filePath = document.FilePath;
        
        if (string.IsNullOrEmpty(filePath))
            return solution;

        // Extract XML documentation from the type and its members
        var xmlDocs = ExtractXmlDocumentation(typeDeclaration);
        
        // Convert XML to markdown
        var markdownContent = ConvertXmlToMarkdown(typeDeclaration, xmlDocs);
        
        // Determine markdown file path
        var directory = Path.GetDirectoryName(filePath) ?? "";
        var sourceFileName = Path.GetFileNameWithoutExtension(filePath);
        
        // Use type name for markdown file (matches MarkdownDocsGenerator expectations)
        var markdownFileName = $"{typeDeclaration.Identifier.Text}.md";
        var markdownFilePath = Path.Combine(directory, markdownFileName);
        
        // Remove XML documentation from source
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root == null) return solution;
        
        var newRoot = RemoveXmlDocumentation(root, typeDeclaration);
        solution = solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        
        // Create markdown file (this is a simplified version - in a real implementation,
        // you'd need to handle adding the file to the project properly)
        await File.WriteAllTextAsync(markdownFilePath, markdownContent, cancellationToken).ConfigureAwait(false);
        
        return solution;
    }

    private Dictionary<string, XmlDocumentationInfo> ExtractXmlDocumentation(TypeDeclarationSyntax typeDeclaration)
    {
        var docs = new Dictionary<string, XmlDocumentationInfo>();
        
        // Extract type-level documentation
        var typeTrivia = typeDeclaration.GetLeadingTrivia()
            .Where(t => t.HasStructure)
            .Select(t => t.GetStructure())
            .OfType<DocumentationCommentTriviaSyntax>()
            .FirstOrDefault();
            
        if (typeTrivia != null)
        {
            docs[""] = ExtractDocumentationInfo(typeTrivia);
        }
        
        // Extract member documentation
        foreach (var member in typeDeclaration.Members)
        {
            var memberTrivia = member.GetLeadingTrivia()
                .Where(t => t.HasStructure)
                .Select(t => t.GetStructure())
                .OfType<DocumentationCommentTriviaSyntax>()
                .FirstOrDefault();
                
            if (memberTrivia != null)
            {
                var memberName = GetMemberName(member);
                if (!string.IsNullOrEmpty(memberName))
                {
                    docs[memberName] = ExtractDocumentationInfo(memberTrivia);
                }
            }
        }
        
        return docs;
    }

    private XmlDocumentationInfo ExtractDocumentationInfo(DocumentationCommentTriviaSyntax trivia)
    {
        var info = new XmlDocumentationInfo();
        
        foreach (var node in trivia.Content)
        {
            if (node is XmlElementSyntax element)
            {
                var name = element.StartTag.Name.LocalName.Text;
                var content = ExtractXmlElementContent(element);
                
                switch (name)
                {
                    case "summary":
                        info.Summary = content;
                        break;
                    case "remarks":
                        info.Remarks = content;
                        break;
                    case "returns":
                        info.Returns = content;
                        break;
                    case "value":
                        info.Value = content;
                        break;
                    case "param":
                        var paramName = element.StartTag.Attributes
                            .OfType<XmlNameAttributeSyntax>()
                            .FirstOrDefault()?.Identifier?.Identifier.Text ?? "";
                        if (!string.IsNullOrEmpty(paramName))
                        {
                            info.Parameters[paramName] = content;
                        }
                        break;
                    case "typeparam":
                        var typeParamName = element.StartTag.Attributes
                            .OfType<XmlNameAttributeSyntax>()
                            .FirstOrDefault()?.Identifier?.Identifier.Text ?? "";
                        if (!string.IsNullOrEmpty(typeParamName))
                        {
                            info.TypeParameters[typeParamName] = content;
                        }
                        break;
                    case "exception":
                        var exceptionType = element.StartTag.Attributes
                            .OfType<XmlCrefAttributeSyntax>()
                            .FirstOrDefault()?.Cref?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(exceptionType))
                        {
                            info.Exceptions[exceptionType] = content;
                        }
                        break;
                    case "seealso":
                        var cref = element.StartTag.Attributes
                            .OfType<XmlCrefAttributeSyntax>()
                            .FirstOrDefault()?.Cref?.ToString() ?? "";
                        if (!string.IsNullOrEmpty(cref))
                        {
                            info.SeeAlso.Add(cref);
                        }
                        break;
                }
            }
        }
        
        return info;
    }

    private string ExtractXmlElementContent(XmlElementSyntax element)
    {
        var content = new System.Text.StringBuilder();
        
        foreach (var node in element.Content)
        {
            if (node is XmlTextSyntax text)
            {
                foreach (var token in text.TextTokens)
                {
                    content.Append(token.Text.Trim());
                    if (token != text.TextTokens.Last())
                        content.Append(" ");
                }
            }
        }
        
        return content.ToString().Trim();
    }

    private string GetMemberName(MemberDeclarationSyntax member)
    {
        return member switch
        {
            MethodDeclarationSyntax method => method.Identifier.Text,
            PropertyDeclarationSyntax property => property.Identifier.Text,
            FieldDeclarationSyntax field => field.Declaration.Variables.FirstOrDefault()?.Identifier.Text ?? "",
            EventDeclarationSyntax @event => @event.Identifier.Text,
            ConstructorDeclarationSyntax => "Constructor",
            _ => ""
        };
    }

    private string ConvertXmlToMarkdown(TypeDeclarationSyntax typeDeclaration, Dictionary<string, XmlDocumentationInfo> xmlDocs)
    {
        var markdown = new System.Text.StringBuilder();
        var typeName = typeDeclaration.Identifier.Text;
        
        // Type header
        markdown.AppendLine($"# {typeName}");
        markdown.AppendLine();
        
        // Type documentation
        if (xmlDocs.TryGetValue("", out var typeDoc))
        {
            if (!string.IsNullOrEmpty(typeDoc.Summary))
            {
                markdown.AppendLine(typeDoc.Summary);
                markdown.AppendLine();
            }
            
            // Type parameters
            if (typeDoc.TypeParameters.Count > 0)
            {
                markdown.AppendLine("## Type Parameters");
                markdown.AppendLine();
                foreach (var (name, description) in typeDoc.TypeParameters)
                {
                    markdown.AppendLine($"`{name}` - {description}");
                }
                markdown.AppendLine();
            }
            
            // Remarks
            if (!string.IsNullOrEmpty(typeDoc.Remarks))
            {
                markdown.AppendLine("## Remarks");
                markdown.AppendLine();
                markdown.AppendLine(typeDoc.Remarks);
                markdown.AppendLine();
            }
            
            // See Also
            if (typeDoc.SeeAlso.Count > 0)
            {
                markdown.AppendLine("## See Also");
                markdown.AppendLine();
                foreach (var cref in typeDoc.SeeAlso)
                {
                    // Convert cref to markdown link format
                    var cleanCref = cref.Replace("T:", "").Replace("M:", "").Replace("P:", "");
                    markdown.AppendLine($"[{cleanCref}]({cleanCref})");
                }
                markdown.AppendLine();
            }
        }
        
        // Properties
        var properties = typeDeclaration.Members.OfType<PropertyDeclarationSyntax>().ToList();
        if (properties.Any(p => xmlDocs.ContainsKey(GetMemberName(p))))
        {
            markdown.AppendLine("## Properties");
            markdown.AppendLine();
            
            foreach (var property in properties)
            {
                var propertyName = property.Identifier.Text;
                if (xmlDocs.TryGetValue(propertyName, out var propDoc))
                {
                    markdown.AppendLine($"### {propertyName}");
                    markdown.AppendLine();
                    
                    if (!string.IsNullOrEmpty(propDoc.Summary))
                    {
                        markdown.AppendLine(propDoc.Summary);
                        markdown.AppendLine();
                    }
                    
                    if (!string.IsNullOrEmpty(propDoc.Value))
                    {
                        markdown.AppendLine("#### Value");
                        markdown.AppendLine();
                        markdown.AppendLine(propDoc.Value);
                        markdown.AppendLine();
                    }
                }
            }
        }
        
        // Methods
        var methods = typeDeclaration.Members.OfType<MethodDeclarationSyntax>().ToList();
        if (methods.Any(m => xmlDocs.ContainsKey(GetMemberName(m))))
        {
            markdown.AppendLine("## Methods");
            markdown.AppendLine();
            
            foreach (var method in methods)
            {
                var methodName = method.Identifier.Text;
                if (xmlDocs.TryGetValue(methodName, out var methodDoc))
                {
                    markdown.AppendLine($"### {methodName}");
                    markdown.AppendLine();
                    
                    if (!string.IsNullOrEmpty(methodDoc.Summary))
                    {
                        markdown.AppendLine(methodDoc.Summary);
                        markdown.AppendLine();
                    }
                    
                    // Parameters
                    if (methodDoc.Parameters.Count > 0)
                    {
                        markdown.AppendLine("#### Parameters");
                        markdown.AppendLine();
                        foreach (var (name, description) in methodDoc.Parameters)
                        {
                            markdown.AppendLine($"`{name}` - {description}");
                        }
                        markdown.AppendLine();
                    }
                    
                    // Returns
                    if (!string.IsNullOrEmpty(methodDoc.Returns))
                    {
                        markdown.AppendLine("#### Returns");
                        markdown.AppendLine();
                        markdown.AppendLine(methodDoc.Returns);
                        markdown.AppendLine();
                    }
                    
                    // Exceptions
                    if (methodDoc.Exceptions.Count > 0)
                    {
                        markdown.AppendLine("#### Exceptions");
                        markdown.AppendLine();
                        foreach (var (type, description) in methodDoc.Exceptions)
                        {
                            markdown.AppendLine($"`{type}` - {description}");
                        }
                        markdown.AppendLine();
                    }
                }
            }
        }
        
        return markdown.ToString();
    }

    private SyntaxNode RemoveXmlDocumentation(SyntaxNode root, TypeDeclarationSyntax typeDeclaration)
    {
        // Remove XML documentation from type and all members
        var rewriter = new RemoveXmlDocumentationRewriter();
        return rewriter.Visit(root);
    }

    private class RemoveXmlDocumentationRewriter : CSharpSyntaxRewriter
    {
        public override SyntaxNode? VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as ClassDeclarationSyntax;
        }

        public override SyntaxNode? VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as InterfaceDeclarationSyntax;
        }

        public override SyntaxNode? VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as MethodDeclarationSyntax;
        }

        public override SyntaxNode? VisitPropertyDeclaration(PropertyDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as PropertyDeclarationSyntax;
        }

        public override SyntaxNode? VisitFieldDeclaration(FieldDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as FieldDeclarationSyntax;
        }

        public override SyntaxNode? VisitEventDeclaration(EventDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as EventDeclarationSyntax;
        }

        public override SyntaxNode? VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            return RemoveXmlDocumentationFromNode(node) as ConstructorDeclarationSyntax;
        }

        private SyntaxNode RemoveXmlDocumentationFromNode(SyntaxNode node)
        {
            var leadingTrivia = node.GetLeadingTrivia();
            var newTrivia = leadingTrivia.Where(t => !t.HasStructure || !(t.GetStructure() is DocumentationCommentTriviaSyntax));
            return node.WithLeadingTrivia(newTrivia);
        }
    }

    private class XmlDocumentationInfo
    {
        public string Summary { get; set; } = "";
        public string Remarks { get; set; } = "";
        public string Returns { get; set; } = "";
        public string Value { get; set; } = "";
        public Dictionary<string, string> Parameters { get; } = new();
        public Dictionary<string, string> TypeParameters { get; } = new();
        public Dictionary<string, string> Exceptions { get; } = new();
        public List<string> SeeAlso { get; } = new();
    }
}