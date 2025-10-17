# Create Interface Delegation Source Generator

## Status: TO DO

## Overview

Create a source generator that implements Delphi-style interface delegation for C#. This allows classes to automatically delegate interface implementations to fields or properties, eliminating boilerplate forwarding code.

## Delphi Inspiration

In Delphi, the `implements` directive allows automatic delegation:

```delphi
type
  TMyClass = class(TInterfacedObject, ILogger, IDataProcessor)
  private
    FLogger: ILogger;
    FProcessor: IDataProcessor;
  public
    property Logger: ILogger read FLogger implements ILogger;
    property Processor: IDataProcessor read FProcessor implements IDataProcessor;
  end;
```

## C# Design

### Attribute Usage

Mark fields or properties with `[Implements]` to delegate interface implementations:

```csharp
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
}
```

### Generated Code

The generator produces delegation methods:

```csharp
// Generated: DataService.implements.g.cs
public partial class DataService
{
    // ILogger delegation to _logger
    public void Log(string message) => _logger.Log(message);
    public void LogError(string message, Exception ex) => _logger.LogError(message, ex);

    // IDataProcessor<string> delegation to _processor
    public string Process(string input) => _processor.Process(input);
    public bool Validate(string input) => _processor.Validate(input);
}
```

## Requirements

### Core Functionality

1. **Attribute Definition**
   - Create `[Implements]` attribute
   - Can be applied to fields and properties
   - No parameters needed (type is inferred)

2. **Generator Detection**
   - Find classes that implement one or more interfaces
   - Locate fields/properties marked with `[Implements]`
   - Extract the type from the marked member

3. **Validation**
   - Verify the containing class declares the interface matching the field/property type
   - Ensure the class is marked `partial`
   - Report diagnostic if interface not declared on class
   - Report diagnostic if class is not partial

4. **Code Generation**
   - Generate delegating methods for all interface methods
   - Generate delegating properties for all interface properties
   - Generate delegating events for all interface events
   - Handle generic interfaces correctly
   - Preserve XML documentation from interface

### Advanced Features

1. **Partial Implementation**
   - Allow manual implementation of specific members
   - Skip generation for manually implemented members
   - Detect method/property/event existence in semantic model

2. **Multiple Interface Delegation**
   - Support delegating different interfaces to different fields
   - Handle cases where one class delegates multiple interfaces

3. **Property Support**
   - Support both fields and properties as delegates
   - Handle read-only properties
   - Handle auto-properties

## Implementation Files

### 1. interface-delegation-attribute.cs

```csharp
namespace TimeWarp.SourceGenerators;

/// <summary>
/// Marks a field or property as the implementation delegate for an interface.
/// The containing class must implement the interface matching the field/property type.
/// </summary>
[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public class ImplementsAttribute : Attribute
{
}
```

### 2. interface-delegation-generator.cs

Key implementation steps:
1. Use `IIncrementalGenerator`
2. Find classes with interface implementations
3. Locate `[Implements]` attributes on fields/properties
4. Use semantic model to:
   - Resolve interface members
   - Check for existing implementations
   - Get full type information
5. Generate forwarding code for each interface member

### 3. Test Example

Create test in `tests/timewarp-source-generators-test-console/`:
- `i-logger.cs` - Simple logger interface
- `console-logger.cs` - Implementation of ILogger
- `data-service.cs` - Class that uses `[Implements]` for delegation

## Technical Considerations

### Member Types to Handle

1. **Methods**
   ```csharp
   public string Process(string input) => _processor.Process(input);
   ```

2. **Properties**
   ```csharp
   public string Name => _logger.Name;
   public int Count
   {
       get => _processor.Count;
       set => _processor.Count = value;
   }
   ```

3. **Events**
   ```csharp
   public event EventHandler DataChanged
   {
       add => _processor.DataChanged += value;
       remove => _processor.DataChanged -= value;
   }
   ```

### Generic Interfaces

Support generic interfaces with proper type parameter substitution:
```csharp
public partial class Service : IDataProcessor<string>
{
    [Implements]
    private readonly IDataProcessor<string> _processor;
}
```

### Diagnostic Codes

- **TW1001**: Class is not marked as partial
- **TW1002**: Field/property type does not match any interface on the class
- **TW1003**: Multiple fields marked with [Implements] for the same interface
- **TW1004**: Field marked with [Implements] but class does not implement the interface

## Benefits

1. **Reduces Boilerplate**: Eliminates repetitive forwarding methods
2. **Composition Over Inheritance**: Makes composition-based designs cleaner
3. **Maintainability**: Interface changes automatically update delegations
4. **Type Safety**: Compile-time verification of delegations
5. **Explicit**: Clear intent that interface is delegated

## Example Use Cases

### Logging Adapter
```csharp
public partial class ServiceWithLogging : IService, ILogger
{
    [Implements]
    private readonly ILogger _logger;

    [Implements]
    private readonly IService _innerService;
}
```

### Decorator Pattern
```csharp
public partial class CachingDataProcessor : IDataProcessor<string>
{
    [Implements]
    private readonly IDataProcessor<string> _inner;

    private readonly Dictionary<string, string> _cache = new();

    // Override specific methods manually
    public string Process(string input)
    {
        if (_cache.TryGetValue(input, out var cached))
            return cached;

        var result = _inner.Process(input);
        _cache[input] = result;
        return result;
    }

    // Validate is still delegated automatically
}
```

### Composite Pattern
```csharp
public partial class CompositeHandler : IHandler
{
    [Implements]
    private readonly IHandler _primaryHandler;

    private readonly IHandler _fallbackHandler;

    // Can mix delegation with custom logic
}
```

## References

- [Delphi Implements Directive Documentation](https://docwiki.embarcadero.com/RADStudio/Athens/en/Using_Implements_for_Delegation)
- [C# Source Generators Cookbook](https://github.com/dotnet/roslyn/blob/main/docs/features/source-generators.cookbook.md)
- [Incremental Generators](https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md)

## Notes

- The type inference approach eliminates redundancy compared to `[Implements(typeof(IInterface))]`
- Follows the same pattern as existing generators in this project
- Should work with C# 8.0+ (nullable reference types supported)
- Compatible with all .NET versions that support source generators
