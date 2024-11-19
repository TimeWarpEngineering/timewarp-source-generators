namespace TimeWarp.SourceGenerators.TestConsole;

public partial class TestClass : IDataProcessor<string>
{
    private readonly bool _strictMode;
    private int _processCount;

    /// <summary>
    /// Gets or sets the maximum length allowed for processing strings.
    /// </summary>
    /// <value>
    /// The maximum string length. Default is 100.
    /// </value>
    public int MaxLength { get; set; } = 100;

    /// <summary>
    /// Gets the number of strings processed since initialization.
    /// </summary>
    /// <value>
    /// The total count of processed strings.
    /// </value>
    public int ProcessedCount => _processCount;

    /// <summary>
    /// Gets the last processed message.
    /// </summary>
    /// <value>
    /// The last message that was successfully processed.
    /// </value>
    public string? LastMessage { get; private set; }

    /// <summary>
    /// Initializes a new instance of the TestClass.
    /// </summary>
    /// <param name="strictMode">Whether to enable strict validation mode.</param>
    public TestClass(bool strictMode = false)
    {
        _strictMode = strictMode;
        _processCount = 0;
    }

    /// <summary>
    /// Gets a test message with optional prefix.
    /// </summary>
    /// <param name="prefix">Optional prefix to add to the message.</param>
    /// <returns>A formatted test message.</returns>
    public string GetMessage(string? prefix = null)
    {
        var message = "This is a test class";
        return string.IsNullOrEmpty(prefix) ? message : $"{prefix}: {message}";
    }

    /// <summary>
    /// Processes the input string according to configured rules.
    /// </summary>
    /// <param name="input">The input string to process.</param>
    /// <returns>The processed string.</returns>
    /// <exception cref="ArgumentNullException">Thrown when input is null.</exception>
    /// <exception cref="ArgumentException">Thrown when input exceeds MaxLength in strict mode.</exception>
    public string Process(string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (_strictMode && input.Length > MaxLength)
            throw new ArgumentException($"Input exceeds maximum length of {MaxLength}", nameof(input));

        _processCount++;
        LastMessage = input.Trim().ToUpperInvariant();
        return LastMessage;
    }

    /// <summary>
    /// Validates if the input string meets processing requirements.
    /// </summary>
    /// <param name="input">The input string to validate.</param>
    /// <returns>True if the string is valid for processing; otherwise, false.</returns>
    public bool Validate(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return !_strictMode || input.Length <= MaxLength;
    }

    /// <summary>
    /// Attempts to process multiple strings in batch mode.
    /// </summary>
    /// <param name="inputs">Array of strings to process.</param>
    /// <param name="results">Array to store processed results.</param>
    /// <returns>The number of successfully processed strings.</returns>
    /// <exception cref="ArgumentNullException">Thrown when inputs or results array is null.</exception>
    /// <exception cref="ArgumentException">Thrown when results array is smaller than inputs array.</exception>
    public int TryProcessBatch(string[] inputs, string[] results)
    {
        if (inputs == null)
            throw new ArgumentNullException(nameof(inputs));
        if (results == null)
            throw new ArgumentNullException(nameof(results));
        if (results.Length < inputs.Length)
            throw new ArgumentException("Results array must be at least as large as inputs array", nameof(results));

        var successCount = 0;
        for (int i = 0; i < inputs.Length; i++)
        {
            if (Validate(inputs[i]))
            {
                try
                {
                    results[i] = Process(inputs[i]);
                    successCount++;
                }
                catch
                {
                    results[i] = string.Empty;
                }
            }
            else
            {
                results[i] = string.Empty;
            }
        }

        return successCount;
    }

    /// <summary>
    /// Resets the processor to its initial state.
    /// </summary>
    public void Reset()
    {
        _processCount = 0;
        LastMessage = null;
    }
}
