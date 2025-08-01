namespace TimeWarp.SourceGenerators.TestConsole;

/// <summary>
/// Defines core operations for data processing.
/// </summary>
/// <typeparam name="T">The type of data to process.</typeparam>
public interface IDataProcessor<T>
{
    /// <summary>
    /// Processes the input data and returns a result.
    /// </summary>
    /// <param name="input">The input data to process.</param>
    /// <returns>The processed result.</returns>
    T Process(T input);

    /// <summary>
    /// Validates if the input meets processing requirements.
    /// </summary>
    /// <param name="input">The input to validate.</param>
    /// <returns>True if the input is valid; otherwise, false.</returns>
    bool Validate(T input);
}
