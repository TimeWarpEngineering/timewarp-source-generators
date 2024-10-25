// namespace TimeWarp.SourceGenerators
// {
//   [Generator]
//   public class SimpleDocumentationGenerator : ISourceGenerator
//   {
//     public void Initialize(GeneratorInitializationContext context)
//     {
//       // No initialization required
//     }
//
//     public void Execute(GeneratorExecutionContext context)
//     {
//       // Get all .cs files
//       var csFiles = context.Compilation.SyntaxTrees
//         .Where(st => Path.GetExtension(st.FilePath) == ".cs")
//         .ToDictionary(st => Path.GetFileNameWithoutExtension(st.FilePath), st => st);
//
//       // Get all .md files
//       var mdFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
//       foreach (var file in context.AdditionalFiles)
//       {
//         if (Path.GetExtension(file.Path) == ".md")
//         {
//           mdFiles.Add(Path.GetFileNameWithoutExtension(file.Path));
//         }
//       }
//
//       foreach (var (fileName, syntaxTree) in csFiles)
//       {
//         if (mdFiles.Contains(fileName))
//         {
//           var root = syntaxTree.GetRoot();
//           var newRoot = AddDocumentationToClasses(root);
//
//           if (root != newRoot)
//           {
//             context.AddSource($"{fileName}.Documented.cs", newRoot.ToFullString());
//           }
//         }
//       }
//     }
//
//     private static SyntaxNode AddDocumentationToClasses(SyntaxNode root)
//     {
//       return root.ReplaceNodes(
//         root.DescendantNodes().OfType<ClassDeclarationSyntax>(),
//         (node, _) => AddDocumentation(node)
//       );
//     }
//
//     private static ClassDeclarationSyntax AddDocumentation(ClassDeclarationSyntax classDeclaration)
//     {
//       var documentationTrivia = SyntaxFactory.ParseLeadingTrivia(
//         @"/// <summary>
// /// Hello World
// /// </summary>
// ");
//
//       return classDeclaration.WithLeadingTrivia(
//         classDeclaration.GetLeadingTrivia().InsertRange(0, documentationTrivia)
//       );
//     }
//   }
// }
