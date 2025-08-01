// Auto-generated documentation for TestClass
#nullable enable

namespace TimeWarp.SourceGenerators.TestConsole;

/// <summary>
/// A test class that demonstrates various documentation features. Implements string processing capabilities.
/// </summary>
/// <remarks>
/// The TestClass provides string processing functionality with configurable validation rules. It can operate in either normal or strict mode, where strict mode enforces additional validation rules such as maximum length constraints.
/// </remarks>
/// <seealso cref="IDataProcessor{T}"/>
/// <seealso cref="IDataProcessor{T}"/>
/// <see cref="System.String"/>
/// <see cref="System.ArgumentException"/>

public partial class TestClass
{
    // Documentation for GetMessage
    /// <summary>
    /// Gets a test message with optional prefix.
    /// </summary>
    /// <param name="prefix">Optional prefix to add to the message.</param>
    /// <returns>
    /// A formatted test message.
    /// </returns>
    public partial string GetMessage(string? prefix);

    // Documentation for Process
    /// <summary>
    /// Processes the input string according to configured rules.
    /// </summary>
    /// <param name="input">The input string to process.</param>
    /// <returns>
    /// The processed string.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
    /// <exception cref="ArgumentException">Thrown when input exceeds MaxLength in strict mode.</exception>
    public partial string Process(string input);

    // Documentation for Validate
    /// <summary>
    /// Validates if the input string meets processing requirements.
    /// </summary>
    /// <param name="input">The input string to validate.</param>
    /// <returns>
    /// True if the string is valid for processing; otherwise, false.
    /// </returns>
    public partial bool Validate(string input);

    // Documentation for TryProcessBatch
    /// <summary>
    /// Attempts to process multiple strings in batch mode.
    /// </summary>
    /// <param name="inputs">Array of strings to process.</param>
    /// <param name="results">Array to store processed results.</param>
    /// <returns>
    /// The number of successfully processed strings.
    /// </returns>
    /// <exception cref="ArgumentNullException">Thrown when inputs or results array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when results array is smaller than inputs array.</exception>
    public partial int TryProcessBatch(string[] inputs, string[] results);

    // Documentation for Reset
    /// <summary>
    /// Resets the processor to its initial state.
    /// </summary>
    public partial void Reset();


}
