#region Purpose
// Reports file-level using directives that belong in the project global-usings file.
#endregion

#region Design
// DiagnosticAnalyzer (not an incremental generator) so every UsingDirectiveSyntax is visited:
// compilation-unit usings, file-scoped namespace usings, and block-namespace usings.
// Third-party GlobalUsingsAnalyzer 1.4.0 only walked CompilationUnitSyntax.Usings and missed
// TimeWarp's namespace-first file-scoped layout.
// Skip global using, using static, aliases, generated/build-output paths, the configured
// global-usings file, and editorconfig excluded_files.
// Default filename is kebab global-usings.cs (configurable). Disabled by default like TW0001.
#endregion

namespace TimeWarp.SourceGenerators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class GlobalUsingsRuleAnalyzer : DiagnosticAnalyzer
{
  public const string DiagnosticId = "TW0007";
  public const string FileNameOption = "dotnet_diagnostic.TW0007.filename";
  public const string ExcludedFilesOption = "dotnet_diagnostic.TW0007.excluded_files";
  public const string DefaultFileName = "global-usings.cs";

  private const string Category = "Usage";

  private static readonly DiagnosticDescriptor Rule = new(
    DiagnosticId,
    "File-level using should move to the global-usings file",
    "Move '{0}' to '{1}'",
    Category,
    DiagnosticSeverity.Warning,
    isEnabledByDefault: false,
    description: "File-level using directives should be moved to the project global-usings file as global using directives. Default filename is kebab-case global-usings.cs."
  );

  private static readonly string[] DefaultExceptions =
  [
    "*.g.cs",
    "*.Generated.cs",
    "*.generated.cs",
    "*.designer.cs",
    "*.Designer.cs",
    "*.razor.cs",
    "*AssemblyInfo.cs",
    "*.AssemblyInfo.cs",
    "*.AssemblyAttributes.cs",
    "*.GlobalUsings.g.cs"
  ];

  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => [Rule];

  public override void Initialize(AnalysisContext context)
  {
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    context.EnableConcurrentExecution();
    context.RegisterSyntaxNodeAction(AnalyzeUsingDirective, SyntaxKind.UsingDirective);
  }

  private void AnalyzeUsingDirective(SyntaxNodeAnalysisContext context)
  {
    var usingDirective = (UsingDirectiveSyntax)context.Node;

    if (usingDirective.GlobalKeyword.IsKind(SyntaxKind.GlobalKeyword))
      return;

    if (usingDirective.StaticKeyword.IsKind(SyntaxKind.StaticKeyword))
      return;

    if (usingDirective.Alias is not null)
      return;

    string filePath = usingDirective.SyntaxTree.FilePath;
    if (IsBuildOutputOrGeneratedPath(filePath))
      return;

    string fileName = Path.GetFileName(filePath);
    string globalUsingsFileName = GetConfiguredFileName(context);

    if (string.Equals(fileName, globalUsingsFileName, StringComparison.OrdinalIgnoreCase))
      return;

    string[] exceptions = GetConfiguredExceptions(context);
    if (IsFileExcepted(fileName, exceptions))
      return;

    string usingName = usingDirective.Name?.ToString() ?? usingDirective.ToString();
    var diagnostic = Diagnostic.Create(Rule, usingDirective.GetLocation(), usingName, globalUsingsFileName);
    context.ReportDiagnostic(diagnostic);
  }

  private static bool IsBuildOutputOrGeneratedPath(string filePath)
  {
    if (string.IsNullOrEmpty(filePath))
      return false;

    string normalized = filePath.Replace('\\', '/');

    if (normalized.Contains("/obj/", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("/bin/", StringComparison.OrdinalIgnoreCase)
        || normalized.Contains("/artifacts/generated/", StringComparison.OrdinalIgnoreCase))
    {
      return true;
    }

    string fileName = Path.GetFileName(normalized);
    if (fileName.StartsWith("TemporaryGeneratedFile_", StringComparison.OrdinalIgnoreCase))
      return true;

    return false;
  }

  private static string GetConfiguredFileName(SyntaxNodeAnalysisContext context)
  {
    AnalyzerConfigOptions options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
    if (options.TryGetValue(FileNameOption, out string? configuredFileName)
        && !string.IsNullOrWhiteSpace(configuredFileName))
    {
      return configuredFileName.Trim();
    }

    return DefaultFileName;
  }

  private static string[] GetConfiguredExceptions(SyntaxNodeAnalysisContext context)
  {
    AnalyzerConfigOptions options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
    if (options.TryGetValue(ExcludedFilesOption, out string? configuredExceptions)
        && !string.IsNullOrEmpty(configuredExceptions))
    {
      IEnumerable<string> additionalExceptions = configuredExceptions
        .Split([';'], StringSplitOptions.RemoveEmptyEntries)
        .Select(s => s.Trim());

      return [.. DefaultExceptions, .. additionalExceptions];
    }

    return DefaultExceptions;
  }

  private static bool IsFileExcepted(string fileName, string[] exceptions)
  {
    if (string.IsNullOrEmpty(fileName))
      return false;

    foreach (string exception in exceptions)
    {
      if (exception.Contains("*"))
      {
        string pattern = exception
          .Replace(".", "\\.")
          .Replace("*", ".*");

        if (Regex.IsMatch(fileName, $"^{pattern}$", RegexOptions.IgnoreCase))
          return true;
      }
      else if (string.Equals(fileName, exception, StringComparison.OrdinalIgnoreCase))
      {
        return true;
      }
    }

    return false;
  }
}
