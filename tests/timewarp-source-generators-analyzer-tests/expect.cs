#region Purpose
// Minimal assertions for the analyzer test console (no xUnit/NUnit).
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

internal static class Expect
{
  public static void Equal<T>(T expected, T actual, string because)
  {
    if (!EqualityComparer<T>.Default.Equals(expected, actual))
    {
      throw new InvalidOperationException($"{because} Expected '{expected}', got '{actual}'.");
    }
  }

  public static void True(bool condition, string because)
  {
    if (!condition)
    {
      throw new InvalidOperationException(because);
    }
  }

  public static void Contains(string expectedSubstring, string actual, string because)
  {
    if (actual is null || !actual.Contains(expectedSubstring, StringComparison.Ordinal))
    {
      throw new InvalidOperationException($"{because} Expected '{actual}' to contain '{expectedSubstring}'.");
    }
  }

  public static void Span(
    Diagnostic diagnostic,
    int line,
    int startColumn,
    int endColumn,
    string because)
  {
    FileLinePositionSpan span = diagnostic.Location.GetLineSpan();
    int actualLine = span.StartLinePosition.Line + 1;
    int actualStart = span.StartLinePosition.Character + 1;
    int actualEnd = span.EndLinePosition.Character + 1;
    if (actualLine != line || actualStart != startColumn || actualEnd != endColumn)
    {
      throw new InvalidOperationException(
        $"{because} Expected {line}:{startColumn}-{endColumn}, got {actualLine}:{actualStart}-{actualEnd} ({diagnostic.GetMessage()})."
      );
    }
  }

  public static Diagnostic Require(
    ImmutableArray<Diagnostic> diagnostics,
    int line,
    int startColumn,
    int endColumn,
    string messagePart,
    string because)
  {
    foreach (Diagnostic diagnostic in diagnostics)
    {
      FileLinePositionSpan span = diagnostic.Location.GetLineSpan();
      int actualLine = span.StartLinePosition.Line + 1;
      int actualStart = span.StartLinePosition.Character + 1;
      int actualEnd = span.EndLinePosition.Character + 1;
      if (actualLine == line
          && actualStart == startColumn
          && actualEnd == endColumn
          && diagnostic.GetMessage().Contains(messagePart, StringComparison.Ordinal))
      {
        return diagnostic;
      }
    }

    string seen = diagnostics.Length == 0
      ? "(none)"
      : string.Join(
        "; ",
        diagnostics.Select(diagnostic =>
        {
          FileLinePositionSpan span = diagnostic.Location.GetLineSpan();
          return $"{span.StartLinePosition.Line + 1}:{span.StartLinePosition.Character + 1}-{span.EndLinePosition.Character + 1} {diagnostic.GetMessage()}";
        })
      );
    throw new InvalidOperationException($"{because} No diagnostic at {line}:{startColumn}-{endColumn} containing '{messagePart}'. Saw: {seen}");
  }
}
