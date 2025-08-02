// Auto-generated documentation for PartialPropertyTest
#nullable enable

namespace TimeWarp.SourceGenerators.TestConsole;

/// <summary>
/// This class demonstrates that partial properties are now supported in .NET 9 and C# 13.
/// </summary>
/// <remarks>
/// This test proves that we can now implement property-level documentation generation in the MarkdownDocsGenerator, as partial properties are fully supported. This was previously not possible in earlier versions of C#.
/// </remarks>

public partial class PartialPropertyTest
{
    // Documentation for TestProperty
    /// <summary>
    /// A test property demonstrating partial property support.
    /// </summary>
    /// <value>
    /// A string value that can be get and set, implemented using partial property syntax.
    /// </value>
    public partial string TestProperty { get => field ??= ""; set => field = value; }



}
