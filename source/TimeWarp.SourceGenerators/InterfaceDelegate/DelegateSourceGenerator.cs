namespace TimeWarp.SourceCodeGenerators;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Immutable;

[Generator]
public partial class DelegateSourceGenerator : IIncrementalGenerator
{
    private const string delegateAttributeSource = @"
namespace TimeWarp
{
    using System;

    /// <summary>
    /// Use this attribute to indicate that delegate source should be generated
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    sealed class DelegateAttribute : Attribute
    {
        public string PropertyName { get; set; }
    }
}";

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // Add the attribute
        context.RegisterPostInitializationOutput(ctx => 
            ctx.AddSource("DelegateAttribute.g.cs", delegateAttributeSource));

        // Get all field declarations with attributes
        IncrementalValuesProvider<FieldDeclarationSyntax> fieldDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: static (s, _) => IsSyntaxTargetForGeneration(s),
                transform: static (ctx, _) => GetSemanticTargetForGeneration(ctx))
            .Where(static m => m is not null)!;

        // Combine with the compilation
        IncrementalValueProvider<(Compilation, ImmutableArray<FieldDeclarationSyntax>)> compilationAndFields
            = context.CompilationProvider.Combine(fieldDeclarations.Collect());

        // Generate the source
        context.RegisterSourceOutput(compilationAndFields,
            static (spc, source) => Execute(source.Item1, source.Item2, spc));
    }

    private static bool IsSyntaxTargetForGeneration(SyntaxNode node)
        => node is FieldDeclarationSyntax { AttributeLists.Count: > 0 };

    private static FieldDeclarationSyntax? GetSemanticTargetForGeneration(GeneratorSyntaxContext context)
    {
        var fieldDeclarationSyntax = (FieldDeclarationSyntax)context.Node;

        foreach (var variable in fieldDeclarationSyntax.Declaration.Variables)
        {
            var fieldSymbol = context.SemanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
            if (fieldSymbol?.GetAttributes().Any(ad => ad.AttributeClass?.ToDisplayString() == "TimeWarp.DelegateAttribute") == true)
            {
                return fieldDeclarationSyntax;
            }
        }

        return null;
    }

    private static void Execute(Compilation compilation, ImmutableArray<FieldDeclarationSyntax> fields, SourceProductionContext context)
    {
        if (fields.IsDefaultOrEmpty)
            return;

        var distinctFields = fields.Distinct();

        foreach (var fieldDeclaration in distinctFields)
        {
            foreach (var variable in fieldDeclaration.Declaration.Variables)
            {
                var semanticModel = compilation.GetSemanticModel(fieldDeclaration.SyntaxTree);
                var fieldSymbol = semanticModel.GetDeclaredSymbol(variable) as IFieldSymbol;
                
                if (fieldSymbol == null) continue;

                var interfaceType = fieldSymbol.Type;
                var namespaceName = fieldSymbol.ContainingNamespace.ToDisplayString();
                var classAccessibility = fieldSymbol.ContainingType.DeclaredAccessibility.ToString().ToLower();
                var fileName = fieldSymbol.ContainingType.Name;
                var className = fieldSymbol.ContainingType.Name;
                var typeArguments = string.Join(",", fieldSymbol.ContainingType.TypeArguments.Select(x => x.Name));
                
                if (!string.IsNullOrWhiteSpace(typeArguments))
                    typeArguments = $"<{typeArguments}>";

                var interfaceSources = new List<string>();
                var interfaceMembers = interfaceType.GetMembers();
                
                foreach (var interfaceMember in interfaceMembers)
                {
                    var memberSource = GenerateSourceForMember(interfaceMember, interfaceType.ToDisplayString(), fieldSymbol.Name);
                    if (!string.IsNullOrWhiteSpace(memberSource))
                        interfaceSources.Add(memberSource);
                }

                var interfaceSource = string.Join("\n", interfaceSources);
                var source =
$@"#nullable enable
namespace {namespaceName} {{
    {classAccessibility} partial class {className}{typeArguments}: {interfaceType.ToDisplayString()}
    {{
        {interfaceSource}
    }}
}}";

                source = CSharpSyntaxTree.ParseText(source)
                    .GetRoot()
                    .NormalizeWhitespace()
                    .ToFullString();

                context.AddSource($"{fileName}.g.cs", source);
            }
        }
    }

    private static string GenerateSourceForMember(ISymbol interfaceMember, string interfaceName, string fieldName)
    {
        return interfaceMember.Kind switch
        {
            SymbolKind.Method => interfaceMember is IMethodSymbol methodSymbol && 
                methodSymbol.MethodKind != MethodKind.PropertyGet && 
                methodSymbol.MethodKind != MethodKind.PropertySet
                    ? GenerateMethod(methodSymbol, interfaceName, fieldName)
                    : string.Empty,
            SymbolKind.Property => GenerateProperty(interfaceMember, interfaceName, fieldName),
            _ => string.Empty
        };
    }

    private static string GenerateProperty(ISymbol interfaceMember, string interfaceName, string fieldName)
    {
        var propertySymbol = interfaceMember as IPropertySymbol;
        if (propertySymbol == null) return string.Empty;

        string accessibility = propertySymbol.DeclaredAccessibility.ToString().ToLower();
        string propertyType = propertySymbol.Type.ToDisplayString();
        string propertyName = propertySymbol.Name;
        string getter = propertySymbol.GetMethod != null ? $" get => {fieldName}.{propertyName};" : "";
        string setter = propertySymbol.SetMethod != null ? $"set => {fieldName}.{propertyName} = value; " : "";

        return $"{accessibility} {propertyType} {propertyName} {{{getter} {setter}}}";
    }

    private static string GenerateMethod(IMethodSymbol methodSymbol, string interfaceName, string fieldName)
    {
        ITypeSymbol returnType = methodSymbol.ReturnType;

        string parameterDeclarations = GenerateParameterDeclarations(methodSymbol);
        string parameterList = GenerateParameterInvocation(methodSymbol);
        string generic = methodSymbol.IsGenericMethod ? $"<{string.Join(",", methodSymbol.TypeParameters)}>" : string.Empty;

        return $"public {returnType.ToDisplayString()} {methodSymbol.Name}{generic}({parameterDeclarations}) => {fieldName}.{methodSymbol.Name}({parameterList});";
    }

    private static string GenerateParameterInvocation(IMethodSymbol methodSymbol)
    {
        return string.Join(", ", methodSymbol.Parameters.Select(p => p.Name));
    }

    private static string GenerateParameterDeclarations(IMethodSymbol methodSymbol)
    {
        return string.Join(", ", methodSymbol.Parameters.Select(p => 
        {
            string parameterDeclaration = $"{p.Type.ToDisplayString()} {p.Name}";
            if (p.HasExplicitDefaultValue)
            {
                parameterDeclaration += $" = {p.ExplicitDefaultValue ?? "default"}";
            }
            return parameterDeclaration;
        }));
    }
}
