namespace TimeWarp.SourceGenerators;

[Generator]
public class FileNameRuleAnalyzer : IIncrementalGenerator
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
  private static readonly string[] DefaultExceptions =
  [
    "*.g.cs",
    "*.Generated.cs",
    "*.generated.cs",
    "*.designer.cs",
    "*.Designer.cs",
    "Directory.Build.props",
    "Directory.Build.targets",
    "Directory.Packages.props",
    "*AssemblyInfo.cs",
    "*.AssemblyInfo.cs",
    "*.AssemblyAttributes.cs",
    "*.GlobalUsings.g.cs",
    "AnalyzerReleases.Shipped.md",
    "AnalyzerReleases.Unshipped.md"
  ];
  
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Create a value provider that provides all syntax trees with config options
    IncrementalValuesProvider<(SyntaxTree tree, AnalyzerConfigOptionsProvider configOptions)> syntaxTreesWithConfig = context.CompilationProvider
      .Combine(context.AnalyzerConfigOptionsProvider)
      .SelectMany((source, _) =>
      {
        (Compilation compilation, AnalyzerConfigOptionsProvider configOptions) = source;
        return compilation.SyntaxTrees.Select(tree => (tree, configOptions));
      });
    
    // Register diagnostics for each syntax tree
    context.RegisterSourceOutput(syntaxTreesWithConfig, (spc, source) =>
    {
      (SyntaxTree tree, AnalyzerConfigOptionsProvider configOptions) = source;
      AnalyzeFileNaming(spc, tree, configOptions);
    });
  }
  
  private void AnalyzeFileNaming(SourceProductionContext context, SyntaxTree tree, AnalyzerConfigOptionsProvider configOptions)
  {
    string filePath = tree.FilePath;
    
    // Skip if file path is empty or null
    if (string.IsNullOrEmpty(filePath))
      return;
    
    string fileName = Path.GetFileName(filePath);
    
    // Skip if not a C# file
    if (!fileName.EndsWith(".cs", StringComparison.OrdinalIgnoreCase))
      return;
    
    // Get configured exceptions
    string[] exceptions = GetConfiguredExceptions(configOptions, tree);
    
    // Check if file matches any exception pattern
    if (IsFileExcepted(fileName, exceptions))
      return;
    
    // Check if file name follows kebab-case pattern
    if (!KebabCasePattern.IsMatch(fileName))
    {
      var location = Location.Create(
        tree,
        TextSpan.FromBounds(0, 0)
      );
      
      var diagnostic = Diagnostic.Create(Rule, location, fileName);
      context.ReportDiagnostic(diagnostic);
    }
  }
  
  private string[] GetConfiguredExceptions(AnalyzerConfigOptionsProvider configOptions, SyntaxTree tree)
  {
    // Get file-specific options
    AnalyzerConfigOptions options = configOptions.GetOptions(tree);
    
    // Try to get configured exceptions from .editorconfig
    if (options.TryGetValue(
      "dotnet_diagnostic.TW0003.excluded_files", 
      out string? configuredExceptions) && !string.IsNullOrEmpty(configuredExceptions))
    {
      // Split by semicolon and trim whitespace
      IEnumerable<string> additionalExceptions = configuredExceptions
        .Split([';'], StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());
      
      // Merge defaults with configured exceptions
      return [.. DefaultExceptions, .. additionalExceptions];
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