#region Purpose
// Shared TWA0023 harness: attribute stub, enablement, and snippet location.
#endregion

#region Design
// TypeStemIdentifierAttribute stays in TimeWarp.Architecture.Attributes. The stub is matched by
// simple name. Locate maps a unique snippet to the identifier span the diagnostic must occupy.
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

internal static class TypeStemIdentifierTestFixture
{
  internal const string AttributeStub =
    """
    namespace TimeWarp.Architecture.Attributes
    {
      [System.AttributeUsage(
        System.AttributeTargets.Field | System.AttributeTargets.Property | System.AttributeTargets.Parameter,
        AllowMultiple = false,
        Inherited = false)]
      public sealed class TypeStemIdentifierAttribute : System.Attribute
      {
        public string Reason { get; }
        public TypeStemIdentifierAttribute(string reason) => Reason = reason;
      }
    }
    """;

  internal static readonly string[] Enabled = [TypeStemIdentifierAnalyzer.DiagnosticId];
  internal static async Task Clean(string source)
  {
    ImmutableArray<Diagnostic> diagnostics = await AnalyzeAsync(source);
    Expect.Equal(0, diagnostics.Length, "Expected no TWA0023.");
  }

  internal static async Task Flag(string source, params (string Snippet, string Identifier, string Stem)[] flags)
  {
    ImmutableArray<Diagnostic> diagnostics = await AnalyzeAsync(source);
    Expect.Equal(flags.Length, diagnostics.Length, "TWA0023 flag count.");
    foreach ((string snippet, string identifier, string stem) in flags)
    {
      (int line, int start, int end) = Locate(source, snippet, identifier);
      Diagnostic diagnostic = Expect.Require(
        diagnostics,
        line,
        start,
        end,
        $"'{identifier}'",
        $"TWA0023 on {identifier}."
      );
      Expect.Contains($"'{stem}'", diagnostic.GetMessage(), $"Stem for {identifier} is {stem}.");
    }
  }

  internal static Task<ImmutableArray<Diagnostic>> AnalyzeAsync(string source) =>
    AnalyzerTestDriver.AnalyzeAsync(
      new TypeStemIdentifierAnalyzer(),
      [("TypeStemIdentifierAttribute.cs", AttributeStub), ("Feature.cs", source)],
      enabledIds: Enabled
    );

  internal static (int Line, int Start, int End) Locate(string source, string snippet, string identifier)
  {
    int index = source.IndexOf(snippet, StringComparison.Ordinal);
    if (index < 0)
    {
      throw new InvalidOperationException($"Snippet not found: {snippet}");
    }

    int idOffset = snippet.LastIndexOf(identifier, StringComparison.Ordinal);
    if (idOffset < 0)
    {
      throw new InvalidOperationException($"Identifier '{identifier}' is not in '{snippet}'.");
    }

    int absolute = index + idOffset;
    int line = 1;
    int column = 1;
    for (int cursor = 0; cursor < absolute; cursor++)
    {
      if (source[cursor] == '\n')
      {
        line++;
        column = 1;
      }
      else
      {
        column++;
      }
    }

    return (line, column, column + identifier.Length);
  }
}
