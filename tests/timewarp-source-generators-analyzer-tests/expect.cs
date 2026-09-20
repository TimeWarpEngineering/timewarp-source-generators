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
}
