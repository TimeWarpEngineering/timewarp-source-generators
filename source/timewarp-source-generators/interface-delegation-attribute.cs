namespace TimeWarp.SourceGenerators;

/// <summary>
/// Marks a field or property as the implementation delegate for an interface.
/// The containing class must implement the interface matching the field/property type.
/// </summary>
/// <remarks>
/// <para>
/// This attribute enables Delphi-style interface delegation in C#. When applied to a field or property,
/// the source generator will automatically create forwarding implementations for all members of the
/// interface that matches the field/property's type.
/// </para>
/// <para>
/// The containing class must:
/// <list type="bullet">
/// <item>Be marked as <c>partial</c></item>
/// <item>Declare that it implements the interface matching the field/property type</item>
/// </list>
/// </para>
/// </remarks>
/// <example>
/// <code>
/// public partial class DataService : ILogger, IDataProcessor&lt;string&gt;
/// {
///     [Implements]
///     private readonly ILogger _logger;
///
///     [Implements]
///     private readonly IDataProcessor&lt;string&gt; _processor;
///
///     public DataService(ILogger logger, IDataProcessor&lt;string&gt; processor)
///     {
///         _logger = logger;
///         _processor = processor;
///     }
/// }
/// </code>
/// The generator will create forwarding implementations for all ILogger methods/properties to _logger
/// and all IDataProcessor&lt;string&gt; methods/properties to _processor.
/// </example>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ImplementsAttribute : Attribute
{
}
