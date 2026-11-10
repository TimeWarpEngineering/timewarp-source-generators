#region Purpose
// TWA0023 exemption cases: opt-out attribute, skipped shapes, overrides, and pragma.
#endregion

namespace TimeWarp.SourceGenerators.AnalyzerTests;

using static TimeWarp.SourceGenerators.AnalyzerTests.TypeStemIdentifierTestFixture;

internal static class TypeStemIdentifierExemptionsAnalyzerTests
{
  public static readonly (string Name, Func<Task> Body)[] Cases =
  [
    ("reasoned opt-out is clean", ReasonedOptOut_Should_BeClean),
    ("empty or whitespace reason still warns", EmptyReason_Should_Report),
    ("vendor prefix without attribute warns", VendorPrefixWithoutAttribute_Should_Report),
    ("foreach named as type is clean", ForeachNamedAsType_Should_BeClean),
    ("foreach mismatch warns", ForeachMismatch_Should_Report),
    ("discard is clean", Discard_Should_BeClean),
    ("array is skipped", Array_Should_BeClean),
    ("override and explicit interface members stay", OverrideAndExplicit_Should_ReportOnlyFreeNames),
    ("duplicate snippet throws", DuplicateSnippet_Should_Throw),
    ("same span twice throws", SameSpanTwice_Should_Throw),
    ("pragma disable is clean", PragmaDisable_Should_BeClean),
    ("IHttpClientFactory factory warns", HttpClientFactory_Should_Report),
    ("named role types are not skipped", NamedRoleTypes_Should_Report),
    ("enum members are skipped", EnumMembers_Should_BeClean),
  ];

  public static Task ReasonedOptOut_Should_BeClean() => Clean(
    """
    #region Purpose
    // Vendor-prefix clip is attribute-only.
    #endregion
    using TimeWarp.Architecture.Attributes;

    public class TimeWarpTerminal { }

    public class Sample
    {
      [TypeStemIdentifier("global collision with System.Terminal")]
      private readonly TimeWarpTerminal Terminal;
    }
    """);

  public static Task EmptyReason_Should_Report() => Flag(
    """
    #region Purpose
    // Empty or whitespace reason is not an opt-out.
    #endregion
    using System.Net.Http;
    using TimeWarp.Architecture.Attributes;

    public class Sample
    {
      [TypeStemIdentifier("")]
      private readonly HttpClient EmptyReason;

      [TypeStemIdentifier("   ")]
      private readonly HttpClient WhitespaceReason;
    }
    """,
    ("HttpClient EmptyReason", "EmptyReason", "HttpClient"),
    ("HttpClient WhitespaceReason", "WhitespaceReason", "HttpClient"));

  public static Task VendorPrefixWithoutAttribute_Should_Report() => Flag(
    """
    #region Purpose
    // TimeWarpTerminal → Terminal is not inferred without the attribute.
    #endregion

    public class TimeWarpTerminal { }

    public class Sample
    {
      private readonly TimeWarpTerminal Terminal;
    }
    """,
    ("TimeWarpTerminal Terminal", "Terminal", "TimeWarpTerminal"));

  public static Task ForeachNamedAsType_Should_BeClean() => Clean(
    """
    #region Purpose
    // Foreach variable uses the HttpClient stem.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      public void Run(HttpClient[] list)
      {
        foreach (HttpClient httpClient in list) { }
      }
    }
    """);

  public static Task ForeachMismatch_Should_Report() => Flag(
    """
    #region Purpose
    // Foreach variable c does not end with HttpClient.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      public void Run(HttpClient[] list)
      {
        foreach (HttpClient c in list) { }
      }
    }
    """,
    ("HttpClient c in list", "c", "HttpClient"));

  public static Task Discard_Should_BeClean() => Clean(
    """
    #region Purpose
    // Discard locals are out of scope.
    #endregion
    using System.Net.Http;

    public class Sample
    {
      public void Run(HttpClient[] list)
      {
        HttpClient _ = list[0];
      }
    }
    """);

  public static Task Array_Should_BeClean() => Clean(
    """
    #region Purpose
    // Arrays are skip-set; name the meaning.
    #endregion

    public class Sample
    {
      public void Use(int[] counts) { }
    }
    """);

  public static Task OverrideAndExplicit_Should_ReportOnlyFreeNames() => Flag(
    """
    #region Purpose
    // Override and explicit interface names are not free; the base/interface still is.
    #endregion
    using System.Net.Http;

    public abstract class Base
    {
      public abstract HttpClient Client { get; }
      public abstract void Run(HttpClient client);
    }

    public class Derived : Base
    {
      public override HttpClient Client => null!;
      public override void Run(HttpClient client) { }
    }

    public interface IHasClient
    {
      HttpClient Client { get; }
    }

    public class Explicit : IHasClient
    {
      HttpClient IHasClient.Client => null!;
    }
    """,
    ("abstract HttpClient Client", "Client", "HttpClient"),
    ("abstract void Run(HttpClient client)", "client", "HttpClient"),
    ("  HttpClient Client { get; }", "Client", "HttpClient"));

  public static Task DuplicateSnippet_Should_Throw()
  {
    const string source = "HttpClient Client { get; }\nHttpClient Client { get; }\n";
    try
    {
      Locate(source, "HttpClient Client { get; }", "Client");
    }
    catch (InvalidOperationException exception)
      when (exception.Message.Contains("not unique", StringComparison.Ordinal))
    {
      return Task.CompletedTask;
    }

    throw new InvalidOperationException("Locate accepted a snippet that occurs twice.");
  }

  public static async Task SameSpanTwice_Should_Throw()
  {
    try
    {
      await Flag(
        """
        #region Purpose
        // Two snippets that name the same property must not both pass.
        #endregion
        using System.Net.Http;

        public class Sample
        {
          public HttpClient Client { get; }
          public void Run(HttpClient client) { }
        }
        """,
        ("public HttpClient Client", "Client", "HttpClient"),
        ("HttpClient Client { get; }", "Client", "HttpClient")
      );
    }
    catch (InvalidOperationException exception)
      when (exception.Message.Contains("already matched", StringComparison.Ordinal))
    {
      return;
    }

    throw new InvalidOperationException("Flag accepted two snippets for one diagnostic.");
  }

  public static Task PragmaDisable_Should_BeClean()
  {
    // The disable directive is assembled so this file does not contain a suppression line.
    string disable = "#pragma warning disable " + TypeStemIdentifierAnalyzer.DiagnosticId;
    string restore = "#pragma warning restore " + TypeStemIdentifierAnalyzer.DiagnosticId;
    string source =
      """
      #region Purpose
      // Standard Roslyn valve for locals and other sites without the attribute.
      #endregion
      using System.Net.Http;

      public class Sample
      {

      """
      + "    " + disable + "\n"
      + "      private readonly HttpClient CatalogClient;\n"
      + "    " + restore + "\n"
      + """
      }
      """;
    return Clean(source);
  }

  public static Task HttpClientFactory_Should_Report() => Flag(
    """
    #region Purpose
    // IHttpClientFactory is not in the skip set; factory is a true positive.
    #endregion

    public interface IHttpClientFactory { }

    public class Sample
    {
      public void Use(IHttpClientFactory factory) { }
    }
    """,
    ("IHttpClientFactory factory", "factory", "HttpClientFactory"));

  public static Task NamedRoleTypes_Should_Report() => Flag(
    """
    #region Purpose
    // True positives: these types name the role and are not in the skip set.
    #endregion
    using System;
    using System.Net;
    using System.Threading;

    namespace Microsoft.Extensions.Logging
    {
      public interface ILogger<TCategoryName> { }
    }

    public class Widget { }

    public class Sample
    {
      public void Use(
        Microsoft.Extensions.Logging.ILogger<Widget> log,
        DateTime dt,
        Guid id,
        TimeSpan ts,
        CancellationToken ct,
        HttpStatusCode code)
      { }
    }
    """,
    ("ILogger<Widget> log", "log", "Logger"),
    ("DateTime dt", "dt", "DateTime"),
    ("Guid id", "id", "Guid"),
    ("TimeSpan ts", "ts", "TimeSpan"),
    ("CancellationToken ct", "ct", "CancellationToken"),
    ("HttpStatusCode code", "code", "HttpStatusCode"));

  public static Task EnumMembers_Should_BeClean() => Clean(
    """
    #region Purpose
    // Enum members are named values, not a role of the enum type.
    #endregion

    public enum Color
    {
      Red,
      Blue
    }
    """);

}
