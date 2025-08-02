namespace TimeWarp.SourceGenerators.TestConsole;

public partial class TestClass : IDataProcessor<string>
{
    private readonly bool StrictMode;
    private int ChickenCount;

    public partial int MaxLength { get; set; }

    public partial int ProcessedCount { get; }

    public partial string? LastMessage { get; set; }

    public TestClass(bool strictMode = false)
    {
        StrictMode = strictMode;
        ChickenCount = 0;
    }

    public partial string GetMessage(string? prefix)
    {
        var message = "This is a test class";
        return string.IsNullOrEmpty(prefix) ? message : $"{prefix}: {message}";
    }

    public partial string Process(string input)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input));

        if (StrictMode && input.Length > MaxLength)
            throw new ArgumentException($"Input exceeds maximum length of {MaxLength}", nameof(input));

        ChickenCount++;
        LastMessage = input.Trim().ToUpperInvariant();
        return LastMessage;
    }

    public partial bool Validate(string input)
    {
        if (string.IsNullOrEmpty(input))
            return false;

        return !StrictMode || input.Length <= MaxLength;
    }

    public partial int TryProcessBatch(string[] inputs, string[] results)
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

    public partial void Reset()
    {
        ChickenCount = 0;
        LastMessage = null;
    }
}

