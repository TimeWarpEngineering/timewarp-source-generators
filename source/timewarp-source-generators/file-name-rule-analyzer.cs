namespace TimeWarp.SourceGenerators;

[Generator]
public class FileNameRuleAnalyzer : IIncrementalGenerator
{
  public const string DiagnosticId = "TW0001";
  private const string Category = "Naming";

  private static readonly DiagnosticDescriptor Rule = new(
    DiagnosticId,
    "File name should use kebab-case",
    "File '{0}' should use kebab-case naming convention (e.g., 'my-file.cs' or 'my-type.my-partial.cs')",
    Category,
    DiagnosticSeverity.Info,
    isEnabledByDefault: false,
    description: "C# file names should use kebab-case format with hyphens separating words, all lowercase. Multi-dot basenames are allowed when every segment is kebab-case (e.g., 'my-type.my-partial.cs')."
  );

  // Regex: single- or multi-dot basenames where every segment is kebab-case, then .cs
  private static readonly Regex KebabCasePattern = new(
    @"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*(?:\.[a-z][a-z0-9]*(?:-[a-z0-9]+)*)*\.cs$",
    RegexOptions.Compiled);

  // Default exception patterns
  private static readonly string[] DefaultExceptions =
  [
    "*.g.cs",
    "*.Generated.cs",
    "*.generated.cs",
    "*.designer.cs",
    "*.Designer.cs",
    "*.razor.cs",  // Razor component code-behind files must match their .razor file names
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

    // Skip build intermediates and toolchain output (gRPC stubs, SDK attributes, etc.).
    // Those basenames are not authored product paths and must not block TW0001 enablement.
    if (IsBuildOutputOrGeneratedPath(filePath))
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

  /// <summary>
  /// True when the path is under bin/obj (or common generated-output roots), not source.
  /// </summary>
  private static bool IsBuildOutputOrGeneratedPath(string filePath)
  {
    // Normalize so both Windows and Unix separators match.
    string normalized = filePath.Replace('\\', '/');

    // Path segments: /obj/, /bin/, and common emitted-generator output folders.
    if (normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("/artifacts/generated/", StringComparison.OrdinalIgnoreCase))
    {
      return true;
    }

    // TemporaryGeneratedFile_*.cs and similar toolchain temps (basename also covered by exceptions).
    string fileName = Path.GetFileName(normalized);
    if (fileName.StartsWith("TemporaryGeneratedFile_", StringComparison.OrdinalIgnoreCase))
      return true;

    return false;
  }

  private string[] GetConfiguredExceptions(AnalyzerConfigOptionsProvider configOptions, SyntaxTree tree)
  {
    // Get file-specific options
    AnalyzerConfigOptions options = configOptions.GetOptions(tree);

    // Try to get configured exceptions from .editorconfig
    if (options.TryGetValue(
      "dotnet_diagnostic.TW0001.excluded_files",
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