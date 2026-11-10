#region Purpose
// Compiles a snippet with GlobalUsingsRuleAnalyzer and returns TW0007 diagnostics.
#endregion

#region Design
// Uses CompilationWithAnalyzers (Workspaces) so tests exercise the real DiagnosticAnalyzer
// pipeline, including AnalyzerConfigOptions for filename and excluded_files.
// Disabled-by-default is honored unless enableTw0007 is true (SpecificDiagnosticOptions Warn).
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

internal static class AnalyzerTestDriver
{
  public static async Task<ImmutableArray<Diagnostic>> GetDiagnosticsAsync(
    string source,
    string filePath,
    Dictionary<string, string>? analyzerConfig = null,
    bool enableTw0007 = true)
  {
    SyntaxTree tree = CSharpSyntaxTree.ParseText(
      source,
      new CSharpParseOptions(LanguageVersion.Latest),
      path: filePath
    );

    ImmutableDictionary<string, ReportDiagnostic> specificOptions = enableTw0007
      ? ImmutableDictionary<string, ReportDiagnostic>.Empty.Add(
          GlobalUsingsRuleAnalyzer.DiagnosticId,
          ReportDiagnostic.Warn
        )
      : ImmutableDictionary<string, ReportDiagnostic>.Empty;

    CSharpCompilation compilation = CSharpCompilation.Create(
      "AnalyzerTest",
      [tree],
      [MetadataReference.CreateFromFile(typeof(object).Assembly.Location)],
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithSpecificDiagnosticOptions(specificOptions)
    );

    AnalyzerOptions analyzerOptions = new(
      ImmutableArray<AdditionalText>.Empty,
      new DictionaryAnalyzerConfigOptionsProvider(analyzerConfig ?? [])
    );

    CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(
      [new GlobalUsingsRuleAnalyzer()],
      analyzerOptions
    );

    ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    return [.. diagnostics.Where(d => d.Id == GlobalUsingsRuleAnalyzer.DiagnosticId)];
  }

  public static Task<ImmutableArray<Diagnostic>> AnalyzeAsync(
    DiagnosticAnalyzer analyzer,
    string source,
    string filePath,
    Dictionary<string, string>? analyzerConfig = null,
    string[]? enabledIds = null)
  {
    return AnalyzeAsync(analyzer, [(filePath, source)], analyzerConfig, enabledIds);
  }

  public static async Task<ImmutableArray<Diagnostic>> AnalyzeAsync(
    DiagnosticAnalyzer analyzer,
    (string Path, string Source)[] sources,
    Dictionary<string, string>? analyzerConfig = null,
    string[]? enabledIds = null)
  {
    CSharpParseOptions parseOptions = new(LanguageVersion.Latest);
    SyntaxTree[] trees =
    [
      .. sources.Select(source => CSharpSyntaxTree.ParseText(source.Source, parseOptions, path: source.Path))
    ];

    ImmutableDictionary<string, ReportDiagnostic> specificOptions =
      ImmutableDictionary<string, ReportDiagnostic>.Empty;
    if (enabledIds is not null)
    {
      foreach (string id in enabledIds)
      {
        specificOptions = specificOptions.Add(id, ReportDiagnostic.Warn);
      }
    }

    CSharpCompilation compilation = CSharpCompilation.Create(
      "AnalyzerTest",
      trees,
      References,
      new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary)
        .WithSpecificDiagnosticOptions(specificOptions)
    );

    AnalyzerOptions analyzerOptions = new(
      ImmutableArray<AdditionalText>.Empty,
      new DictionaryAnalyzerConfigOptionsProvider(analyzerConfig ?? [])
    );

    CompilationWithAnalyzers compilationWithAnalyzers = compilation.WithAnalyzers(
      [analyzer],
      analyzerOptions
    );

    string diagnosticId = analyzer.SupportedDiagnostics[0].Id;
    ImmutableArray<Diagnostic> diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();
    return [.. diagnostics.Where(diagnostic => diagnostic.Id == diagnosticId)];
  }

  private static readonly ImmutableArray<MetadataReference> References = CreateReferences();

  private static ImmutableArray<MetadataReference> CreateReferences()
  {
    string[] locations =
    [
      typeof(object).Assembly.Location,
      typeof(Console).Assembly.Location,
      typeof(Enumerable).Assembly.Location,
      typeof(HttpClient).Assembly.Location,
      typeof(HttpStatusCode).Assembly.Location,
      typeof(List<>).Assembly.Location,
      typeof(Attribute).Assembly.Location,
      typeof(Task).Assembly.Location
    ];

    return
    [
      .. locations
        .Distinct(StringComparer.Ordinal)
        .Select(location => MetadataReference.CreateFromFile(location))
    ];
  }
}

internal sealed class DictionaryAnalyzerConfigOptionsProvider : AnalyzerConfigOptionsProvider
{
  private readonly AnalyzerConfigOptions Options;

  public DictionaryAnalyzerConfigOptionsProvider(Dictionary<string, string> options)
  {
    Options = new DictionaryAnalyzerConfigOptions(options);
  }

  public override AnalyzerConfigOptions GlobalOptions => Options;

  public override AnalyzerConfigOptions GetOptions(SyntaxTree tree) => Options;

  public override AnalyzerConfigOptions GetOptions(AdditionalText textFile) => Options;
}

internal sealed class DictionaryAnalyzerConfigOptions : AnalyzerConfigOptions
{
  private readonly Dictionary<string, string> Options;

  public DictionaryAnalyzerConfigOptions(Dictionary<string, string> options)
  {
    Options = options;
  }

  public override bool TryGetValue(string key, out string value)
  {
    if (Options.TryGetValue(key, out string? found) && found is not null)
    {
      value = found;
      return true;
    }

    value = null!;
    return false;
  }
}
