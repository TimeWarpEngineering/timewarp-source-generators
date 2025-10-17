namespace TimeWarp.SourceGenerators.TestConsole;

/// <summary>
/// Example service that delegates interface implementations using the [Implements] attribute.
/// Demonstrates Delphi-style interface delegation in C#.
/// </summary>
public partial class DataService : ILogger, IDataProcessor<string>
{
    [Implements]
    private readonly ILogger _logger;

    [Implements]
    private readonly IDataProcessor<string> _processor;

    public DataService(ILogger logger, IDataProcessor<string> processor)
    {
        _logger = logger;
        _processor = processor;
    }

    // This method is manually implemented to demonstrate partial implementation
    // The generator will skip this method and delegate the rest
    public string Process(string input)
    {
        _logger.Log($"Processing: {input}");
        var result = _processor.Process(input);
        _logger.Log($"Result: {result}");
        return result;
    }

    // All other members of ILogger and IDataProcessor<string> are automatically delegated
}
