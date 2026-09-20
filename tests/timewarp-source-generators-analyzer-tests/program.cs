#region Purpose
// Runs TW0007 analyzer assertions and exits non-zero on failure.
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

public static class Program
{
  public static async Task<int> Main()
  {
    (string Name, Func<Task> Body)[] cases =
    [
      ("descriptor is warning, disabled, kebab default", Wrap(GlobalUsingsRuleAnalyzerTests.Descriptor_Should_BeWarningDisabledByDefaultWithKebabFilename)),
      ("namespace-first file-scoped using warns", GlobalUsingsRuleAnalyzerTests.NamespaceFirstFileScopedUsing_Should_ReportTw0007),
      ("usings before namespace warn", GlobalUsingsRuleAnalyzerTests.UsingsBeforeNamespace_Should_ReportTw0007),
      ("block-namespace using warns", GlobalUsingsRuleAnalyzerTests.BlockNamespaceUsing_Should_ReportTw0007),
      ("global using does not warn", GlobalUsingsRuleAnalyzerTests.GlobalUsing_Should_NotReport),
      ("using static does not warn", GlobalUsingsRuleAnalyzerTests.UsingStatic_Should_NotReport),
      ("using alias does not warn", GlobalUsingsRuleAnalyzerTests.UsingAlias_Should_NotReport),
      ("global-usings.cs itself does not warn", GlobalUsingsRuleAnalyzerTests.GlobalUsingsFile_Should_NotReport),
      ("PascalCase filename is configurable", GlobalUsingsRuleAnalyzerTests.PascalCaseFilename_Should_SkipConfiguredFileAndAppearInMessage),
      ("excluded_files skips matching files", GlobalUsingsRuleAnalyzerTests.ExcludedFiles_Should_NotReport),
      ("disabled by default does not warn", GlobalUsingsRuleAnalyzerTests.DisabledByDefault_Should_NotReportWithoutEnablement)
    ];

    int passed = 0;
    int failed = 0;

    foreach ((string Name, Func<Task> Body) testCase in cases)
    {
      try
      {
        await testCase.Body();
        passed++;
        Console.WriteLine($"PASS {testCase.Name}");
      }
      catch (Exception exception)
      {
        failed++;
        Console.WriteLine($"FAIL {testCase.Name}: {exception.Message}");
      }
    }

    Console.WriteLine();
    Console.WriteLine($"Passed: {passed}  Failed: {failed}  Total: {cases.Length}");
    return failed == 0 ? 0 : 1;
  }

  private static Func<Task> Wrap(Action action)
  {
    return () =>
    {
      action();
      return Task.CompletedTask;
    };
  }
}
