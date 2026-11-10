#region Purpose
// TWA0038: flags a C# source file longer than the line limit (500 by default), so a class that
// has taken on several jobs is split by responsibility before it grows further.
#endregion

#region Design
// Per file, not per type: partials are separate files and need no special case.
// DefaultMaximumLines is the limit. `dotnet_diagnostic.TWA0038.max_lines` overrides it per tree.
// Generated code is skipped by GeneratedCodeAnalysisFlags.None (auto-generated header, *.g.cs,
// *.Designer.cs, *.generated.cs, and editorconfig generated_code = true).
// IsExemptPath also skips obj/ and goldens/ path segments when the file lacks a generated header.
// Line count matches `wc -l`: a trailing line break does not open a counted line. The diagnostic
// anchors on the first token (as TWA0004 does) so a leading `#pragma warning disable` can suppress it.
// Id stays TWA0038 so an Architecture.Analyzers .editorconfig key still applies.
#endregion

namespace TimeWarp.SourceGenerators;

/// <summary>Roslyn analyzer for TWA0038: a source file is longer than the line limit.</summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class FileLengthAnalyzer : DiagnosticAnalyzer
{
  /// <summary>Diagnostic identifier TWA0038.</summary>
  public const string DiagnosticId = "TWA0038";

  /// <summary>Line limit when .editorconfig does not set <see cref="MaximumLinesOptionKey"/>.</summary>
  public const int DefaultMaximumLines = 500;

  /// <summary>.editorconfig key that overrides <see cref="DefaultMaximumLines"/> for a tree.</summary>
  public const string MaximumLinesOptionKey = "dotnet_diagnostic.TWA0038.max_lines";

  private static readonly DiagnosticDescriptor Rule =
    new
    (
      DiagnosticId,
      title: "Source file is longer than the line limit",
      messageFormat: "File '{0}' has {1} lines, over the {2}-line limit; extract a responsibility into its own type",
      category: "Maintainability",
      DiagnosticSeverity.Warning,
      isEnabledByDefault: true,
      description: "A long file is a sign that its type does more than one job. Refactor toward single responsibility by extracting named types; splitting one class across partial files is not the fix in product code."
    );

  /// <summary>Diagnostics this analyzer reports.</summary>
  public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

  /// <summary>Registers the syntax-tree action that reports TWA0038.</summary>
  public override void Initialize(AnalysisContext context)
  {
    context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
    context.EnableConcurrentExecution();
    context.RegisterSyntaxTreeAction(AnalyzeTree);
  }

  /// <summary>Line count as `wc -l` reports it: a trailing line break does not add a line.</summary>
  internal static int CountLines(SourceText text)
  {
    int count = text.Lines.Count;
    if (count > 0 && text.Lines[count - 1].Span.IsEmpty) count--;
    return count;
  }

  /// <summary>True when the path sits under an obj/ or goldens/ directory.</summary>
  internal static bool IsExemptPath(string? path)
  {
    if (string.IsNullOrEmpty(path)) return false;

    // Leading "/" lets a relative "obj/..." path match the same segment test as an absolute one.
    string normalized = "/" + path!.Replace('\\', '/');
    return normalized.Contains("/obj/", StringComparison.Ordinal)
      || normalized.Contains("/goldens/", StringComparison.Ordinal);
  }

  private static void AnalyzeTree(SyntaxTreeAnalysisContext context)
  {
    if (IsExemptPath(context.Tree.FilePath)) return;

    int maximumLines = MaximumLines(context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Tree));
    int lineCount = CountLines(context.Tree.GetText(context.CancellationToken));
    if (lineCount <= maximumLines) return;

    SyntaxToken firstToken = context.Tree.GetRoot(context.CancellationToken).GetFirstToken();
    Location location = firstToken.RawKind == 0
      ? Location.Create(context.Tree, new TextSpan(0, 0))
      : firstToken.GetLocation();

    string fileName = Path.GetFileName(context.Tree.FilePath);
    context.ReportDiagnostic(Diagnostic.Create(Rule, location, fileName, lineCount, maximumLines));
  }

  private static int MaximumLines(AnalyzerConfigOptions options) =>
    options.TryGetValue(MaximumLinesOptionKey, out string? value)
    && int.TryParse(value?.Trim(), out int parsed)
    && parsed > 0
      ? parsed
      : DefaultMaximumLines;
}
