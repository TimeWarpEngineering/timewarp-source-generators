#region Purpose
// TWA0023 naming cases: descriptor, clean stems, and flagged mismatches.
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

using static TimeWarp.SourceGenerators.AnalyzerTests.TypeStemIdentifierTestFixture;

internal static class TypeStemIdentifierNamingAnalyzerTests
{
  public static readonly (string Name, Func<Task> Body)[] Cases =
  [
    ("descriptor is warning and off by default", Descriptor_Should_BeWarningDisabledByDefault),
    ("disabled by default does not warn", DisabledByDefault_Should_NotReport),
    ("field named as type is clean", FieldNamedAsType_Should_BeClean),
    ("parameter named as type is clean", ParameterNamedAsType_Should_BeClean),
    ("local named as type is clean", LocalNamedAsType_Should_BeClean),
    ("shortened type name warns", ShortenedTypeName_Should_Report),
    ("qualifier dropping the type head warns", QualifierDroppingTypeHead_Should_Report),
    ("qualified type head is clean", QualifiedTypeHead_Should_BeClean),
    ("receiving person qualifier is clean", ReceivingPerson_Should_BeClean),
    ("interface I strip exact stem is clean", InterfaceExactStem_Should_BeClean),
    ("interface I strip qualified stem is clean", InterfaceQualifiedStem_Should_BeClean),
    ("interface I strip mismatch warns", InterfaceMismatch_Should_Report),
    ("ILogger stem is Logger", LoggerStem_Should_BeClean),
    ("primitives are skipped", Primitives_Should_BeClean),
    ("untyped boxes are skipped", UntypedBoxes_Should_BeClean),
    ("IEnumerable of Task is skipped", EnumerableOfTask_Should_BeClean),
  ];

  public static Task Descriptor_Should_BeWarningDisabledByDefault()
  {
    DiagnosticDescriptor descriptor = new TypeStemIdentifierAnalyzer().SupportedDiagnostics[0];
    Expect.Equal(TypeStemIdentifierAnalyzer.DiagnosticId, descriptor.Id, "Id stays TWA0023.");
    Expect.True(!descriptor.IsEnabledByDefault, "TWA0023 ships off.");
    Expect.Equal(DiagnosticSeverity.Warning, descriptor.DefaultSeverity, "Default severity is warning.");
    Expect.Equal("Naming", descriptor.Category, "Category stays Naming.");
    return Task.CompletedTask;
  }

  public static async Task DisabledByDefault_Should_NotReport()
  {
    const string source =
      """
      #region Purpose
      // Discovery is not the OriginHomeDiscovery stem.
      #endregion

      public class OriginHomeDiscovery { }

      public class Sample
      {
        private readonly OriginHomeDiscovery Discovery;
      }
      """;

    ImmutableArray<Diagnostic> diagnostics = await AnalyzerTestDriver.AnalyzeAsync(
      new TypeStemIdentifierAnalyzer(),
      [("TypeStemIdentifierAttribute.cs", AttributeStub), ("Feature.cs", source)]
    );
    Expect.Equal(0, diagnostics.Length, "Off-by-default means a mismatch is silent until enabled.");
  }

  public static Task FieldNamedAsType_Should_BeClean() => Clean(
    """
    #region Purpose
    // Exact type-stem field.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      private readonly HttpClient HttpClient;
    }
    """);

  public static Task ParameterNamedAsType_Should_BeClean() => Clean(
    """
    #region Purpose
    // Exact type-stem parameter.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      public void Process(HttpClient httpClient) { }
    }
    """);

  public static Task LocalNamedAsType_Should_BeClean() => Clean(
    """
    #region Purpose
    // Exact type-stem local.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      public void Run()
      {
        HttpClient httpClient = new HttpClient();
      }
    }
    """);

  public static Task ShortenedTypeName_Should_Report() => Flag(
    """
    #region Purpose
    // Discovery is not the OriginHomeDiscovery stem.
    #endregion

    public class OriginHomeDiscovery { }

    public class Sample
    {
      private readonly OriginHomeDiscovery Discovery;
    }
    """,
    ("OriginHomeDiscovery Discovery", "Discovery", "OriginHomeDiscovery"));

  public static Task QualifierDroppingTypeHead_Should_Report() => Flag(
    """
    #region Purpose
    // CatalogClient drops the HttpClient head.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      private readonly HttpClient CatalogClient;
    }
    """,
    ("HttpClient CatalogClient", "CatalogClient", "HttpClient"));

  public static Task QualifiedTypeHead_Should_BeClean() => Clean(
    """
    #region Purpose
    // Two of the same type keep HttpClient as the head.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      private readonly HttpClient CatalogHttpClient;
      private readonly HttpClient BillingHttpClient;
    }
    """);

  public static Task ReceivingPerson_Should_BeClean() => Clean(
    """
    #region Purpose
    // Person stem with a qualifying prefix.
    #endregion

    public class Person { }

    public class Sample
    {
      private readonly Person ReceivingPerson;
    }
    """);

  public static Task InterfaceExactStem_Should_BeClean() => Clean(
    """
    #region Purpose
    // IFileSystem strips to FileSystem.
    #endregion

    public interface IFileSystem { }

    public class Sample
    {
      private readonly IFileSystem FileSystem;
    }
    """);

  public static Task InterfaceQualifiedStem_Should_BeClean() => Clean(
    """
    #region Purpose
    // Two file systems qualify the FileSystem stem.
    #endregion

    public interface IFileSystem { }

    public class Sample
    {
      private readonly IFileSystem LocalFileSystem;
    }
    """);

  public static Task InterfaceMismatch_Should_Report() => Flag(
    """
    #region Purpose
    // fs does not end with FileSystem.
    #endregion

    public interface IFileSystem { }

    public class Sample
    {
      public void Use(IFileSystem fs) { }
    }
    """,
    ("IFileSystem fs", "fs", "FileSystem"));

  public static Task LoggerStem_Should_BeClean() => Clean(
    """
    #region Purpose
    // ILogger<T> stem is Logger after I-strip; not in the skip set.
    #endregion
    namespace Microsoft.Extensions.Logging
    {
      public interface ILogger<TCategoryName> { }
    }

    public class Widget { }

    public class Sample
    {
      public void Use(
        Microsoft.Extensions.Logging.ILogger<Widget> logger,
        Microsoft.Extensions.Logging.ILogger<Widget> catalogLogger)
      { }
    }
    """);

  public static Task Primitives_Should_BeClean() => Clean(
    """
    #region Purpose
    // string / int / bool are SpecialType skips — name the meaning, not the type.
    #endregion

    public class Sample
    {
      public void Use(string title, int count, bool ready) { }
    }
    """);

  public static Task UntypedBoxes_Should_BeClean() => Clean(
    """
    #region Purpose
    // List / Dictionary are untyped boxes.
    #endregion
    using System.Collections.Generic;
    using System.Net.Http;

    public class Sample
    {
      public void Use(List<HttpClient> pending, Dictionary<string, int> map) { }
    }
    """);

  public static Task EnumerableOfTask_Should_BeClean() => Clean(
    """
    #region Purpose
    // IEnumerable`1 and Task are both skip-set boxes.
    #endregion
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Sample
    {
      public void Use(IEnumerable<Task> items) { }
    }
    """);

}
