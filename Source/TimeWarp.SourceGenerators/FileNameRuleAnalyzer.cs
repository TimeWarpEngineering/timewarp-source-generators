using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace TimeWarp.SourceGenerators;

// TODO: Rewrite as IIncrementalGenerator for better performance and consistency with modern practices
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FileNameRuleAnalyzer : DiagnosticAnalyzer
{
  public const string DiagnosticId = "TW0003";
  private const string Category = "Naming";
  
  private static readonly DiagnosticDescriptor Rule = new(
    DiagnosticId,
    "File name should use kebab-case",
    "File '{0}' should use kebab-case naming convention (e.g., 'my-file.cs')",
    Category,
    DiagnosticSeverity.Info,
    isEnabledByDefault: false,
    description: "C# file names should use kebab-case format with hyphens separating words, all lowercase."
  );
  
  // Regex pattern for valid kebab-case file names
  private static readonly Regex KebabCasePattern = new(@"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*\.cs$", RegexOptions.Compiled);
  
  // Default exception patterns
  private static readonly string[] DefaultExceptions = new[]
  {
    "*.g.cs",
    "*.Generated.cs",
    "*.generated.cs",
    "*.designer.cs",
    "*.Designer.cs",
    "GlobalUsings.cs",
    "Directory.Build.props",
    "Directory.Build.targets",
    "Program.cs",
    "Startup.cs",
    "AssemblyInfo.cs"
  };
  
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);
  
  public override void Initialize(AnalysisContext context)
  {
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    context.EnableConcurrentExecution();
    
    context.RegisterSyntaxTreeAction(AnalyzeFileNaming);
  }
  
  private void AnalyzeFileNaming(SyntaxTreeAnalysisContext context)
  {
    string filePath = context.Tree.FilePath;
    
    // Skip if file path is empty or null
    if (string.IsNullOrEmpty(filePath))
      return;
    
    string fileName = Path.GetFileName(filePath);
    
    // Skip if not a C# file
    if (!fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
      return;
    
    // Get configured exceptions from .editorconfig
    string[] exceptions = GetConfiguredExceptions(context.Options);
    
    // Check if file matches any exception pattern
    if (IsFileExcepted(fileName, exceptions))
      return;
    
    // Check if file name follows kebab-case pattern
    if (!KebabCasePattern.IsMatch(fileName))
    {
      Location location = Location.Create(
        context.Tree,
        Microsoft.CodeAnalysis.Text.TextSpan.FromBounds(0, 0)
      );
      
      Diagnostic diagnostic = Diagnostic.Create(Rule, location, fileName);
      context.ReportDiagnostic(diagnostic);
    }
  }
  
  private string[] GetConfiguredExceptions(AnalyzerOptions options)
  {
    // Try to get configured exceptions from .editorconfig
    if (options.AnalyzerConfigOptionsProvider.GlobalOptions.TryGetValue(
      "dotnet_diagnostic.TW0003.excluded_files", 
      out string? configuredExceptions) && !string.IsNullOrEmpty(configuredExceptions))
    {
      // Split by semicolon and trim whitespace
      return configuredExceptions
        .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim())
        .ToArray();
    }
    
    // Return default exceptions if not configured
    return DefaultExceptions;
  }
  
  private bool IsFileExcepted(string fileName, string[] exceptions)
  {
    foreach (string exception in exceptions)
    {
      // Handle glob patterns
      if (exception.Contains("*"))
      {
        string pattern = exception
          .Replace(".", "\\.")
          .Replace("*", ".*");
        
        if (Regex.IsMatch(fileName, $"^{pattern}$", RegexOptions.IgnoreCase))
          return true;
      }
      else
      {
        // Exact match (case-insensitive)
        if (string.Equals(fileName, exception, StringComparison.OrdinalIgnoreCase))
          return true;
      }
    }
    
    return false;
  }
}