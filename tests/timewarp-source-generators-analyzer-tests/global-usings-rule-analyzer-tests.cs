#region Purpose
// TW0007 cases: namespace-first warns; skips for global/static/alias/global-usings file; configurable filename.
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

internal static class GlobalUsingsRuleAnalyzerTests
{
  public static async Task NamespaceFirstFileScopedUsing_Should_ReportTw0007()
  {
    const string source =
      """
      namespace Sample;
      using System.Text;
      public class Widget
      {
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/widget.cs"
    );

    Expect.Equal(1, diagnostics.Length, "Namespace-first file-scoped using should warn.");
    Expect.Contains("System.Text", diagnostics[0].GetMessage(), "Message should name the using.");
    Expect.Contains(
      GlobalUsingsRuleAnalyzer.DefaultFileName,
      diagnostics[0].GetMessage(),
      "Default filename should be kebab global-usings.cs."
    );
  }

  public static async Task UsingsBeforeNamespace_Should_ReportTw0007()
  {
    const string source =
      """
      using System.Text;
      namespace Sample;
      public class Widget
      {
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/widget.cs"
    );

    Expect.Equal(1, diagnostics.Length, "Compilation-unit using before namespace should warn.");
    Expect.Contains("System.Text", diagnostics[0].GetMessage(), "Message should name the using.");
  }

  public static async Task BlockNamespaceUsing_Should_ReportTw0007()
  {
    const string source =
      """
      namespace Sample
      {
        using System.Text;
        public class Widget
        {
        }
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/widget.cs"
    );

    Expect.Equal(1, diagnostics.Length, "Block-namespace using should warn.");
  }

  public static async Task GlobalUsing_Should_NotReport()
  {
    const string source =
      """
      namespace Sample;
      global using System.Text;
      public class Widget
      {
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/widget.cs"
    );

    Expect.Equal(0, diagnostics.Length, "global using should not warn.");
  }

  public static async Task UsingStatic_Should_NotReport()
  {
    const string source =
      """
      namespace Sample;
      using static System.Math;
      public class Widget
      {
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/widget.cs"
    );

    Expect.Equal(0, diagnostics.Length, "using static should not warn.");
  }

  public static async Task UsingAlias_Should_NotReport()
  {
    const string source =
      """
      namespace Sample;
      using Text = System.Text;
      public class Widget
      {
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/widget.cs"
    );

    Expect.Equal(0, diagnostics.Length, "using alias should not warn.");
  }

  public static async Task GlobalUsingsFile_Should_NotReport()
  {
    const string source =
      """
      using System.Text;
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      source,
      "/src/global-usings.cs"
    );

    Expect.Equal(0, diagnostics.Length, "Usings in global-usings.cs itself should not warn.");
  }

  public static async Task PascalCaseFilename_Should_SkipConfiguredFileAndAppearInMessage()
  {
    Dictionary<string, string> config = new()
    {
      [GlobalUsingsRuleAnalyzer.FileNameOption] = "GlobalUsings.cs"
    };

    ImmutableArray<Diagnostic> skipped = await AnalyzerTestDriver.GetDiagnosticsAsync(
      "using System.Text;",
      "/src/GlobalUsings.cs",
      config
    );
    Expect.Equal(0, skipped.Length, "Configured PascalCase GlobalUsings.cs should be skipped.");

    ImmutableArray<Diagnostic> reported = await AnalyzerTestDriver.GetDiagnosticsAsync(
      """
      namespace Sample;
      using System.Text;
      public class Widget
      {
      }
      """,
      "/src/widget.cs",
      config
    );

    Expect.Equal(1, reported.Length, "Namespace-first using should still warn with PascalCase filename.");
    Expect.Contains("GlobalUsings.cs", reported[0].GetMessage(), "Message should use the configured filename.");
  }

  public static async Task ExcludedFiles_Should_NotReport()
  {
    Dictionary<string, string> config = new()
    {
      [GlobalUsingsRuleAnalyzer.ExcludedFilesOption] = "legacy-usings.cs"
    };

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      """
      namespace Sample;
      using System.Text;
      public class Widget
      {
      }
      """,
      "/src/legacy-usings.cs",
      config
    );

    Expect.Equal(0, diagnostics.Length, "excluded_files should skip matching basenames.");
  }

  public static async Task DisabledByDefault_Should_NotReportWithoutEnablement()
  {
    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.GetDiagnosticsAsync(
      """
      namespace Sample;
      using System.Text;
      public class Widget
      {
      }
      """,
      "/src/widget.cs",
      enableTw0007: false
    );

    Expect.Equal(0, diagnostics.Length, "TW0007 is disabled by default until opted in.");
  }

  public static void Descriptor_Should_BeWarningDisabledByDefaultWithKebabFilename()
  {
    GlobalUsingsRuleAnalyzer analyzer = new();
    DiagnosticDescriptor descriptor = analyzer.SupportedDiagnostics.Single();
    Expect.Equal("TW0007", descriptor.Id, "Diagnostic id.");
    Expect.Equal(DiagnosticSeverity.Warning, descriptor.DefaultSeverity, "Default severity is warning.");
    Expect.True(!descriptor.IsEnabledByDefault, "Must stay opt-in so package bumps do not go red.");
    Expect.Equal("global-usings.cs", GlobalUsingsRuleAnalyzer.DefaultFileName, "Default filename is kebab.");
  }
}
