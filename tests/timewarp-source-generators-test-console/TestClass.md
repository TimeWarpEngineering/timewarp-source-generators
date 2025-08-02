# TestClass

A test class that demonstrates various documentation features. Implements string processing capabilities.

## Remarks
The TestClass provides string processing functionality with configurable validation rules. It can operate in either normal or strict mode, where strict mode enforces additional validation rules such as maximum length constraints.

## Properties

### MaxLength
Gets or sets the maximum length allowed for processing strings.

#### Value
The maximum string length. Default is 100.

### ProcessedCount
Gets the number of strings processed since initialization.

#### Value
The total count of processed strings.

### LastMessage
Gets the last processed message.

#### Value
The last message that was successfully processed.

## Methods

### Constructor
Initializes a new instance of the TestClass.

#### Parameters
- `strictMode` - Whether to enable strict validation mode. Defaults to false.

#### Example
```csharp
// Create instance in normal mode
var processor = new TestClass();

// Create instance in strict mode
var strictProcessor = new TestClass(strictMode: true);
```

### GetMessage
Gets a test message with optional prefix.

#### Parameters
- `prefix` - Optional prefix to add to the message.

#### Returns
A formatted test message.

#### Example
```csharp
var processor = new TestClass();
string message = processor.GetMessage("Hello");  // Returns "Hello: This is a test class"
string defaultMessage = processor.GetMessage();  // Returns "This is a test class"
```

### Process
Processes the input string according to configured rules.

#### Parameters
- `input` - The input string to process.

#### Returns
The processed string.

#### Exceptions
- `ArgumentNullException` - Thrown when input is null.
- `ArgumentException` - Thrown when input exceeds MaxLength in strict mode.

#### Example
```csharp
var processor = new TestClass();
string result = processor.Process("  test  ");  // Returns "TEST"

// In strict mode with length validation
var strictProcessor = new TestClass(strictMode: true);
strictProcessor.MaxLength = 10;
string valid = strictProcessor.Process("Short");     // Works fine
string invalid = strictProcessor.Process("Very long string");  // Throws ArgumentException
```

### Validate
Validates if the input string meets processing requirements.

#### Parameters
- `input` - The input string to validate.

#### Returns
True if the string is valid for processing; otherwise, false.

#### Example
```csharp
var processor = new TestClass(strictMode: true);
processor.MaxLength = 5;
bool isValid = processor.Validate("Test");     // Returns true
bool isTooLong = processor.Validate("Testing"); // Returns false
```

### TryProcessBatch
Attempts to process multiple strings in batch mode.

#### Parameters
- `inputs` - Array of strings to process.
- `results` - Array to store processed results.

#### Returns
The number of successfully processed strings.

#### Exceptions
- `ArgumentNullException` - Thrown when inputs or results array is null.
- `ArgumentException` - Thrown when results array is smaller than inputs array.

#### Example
```csharp
var processor = new TestClass();
string[] inputs = new[] { "test1", "test2", "test3" };
string[] results = new string[3];

int successCount = processor.TryProcessBatch(inputs, results);
// results now contains: ["TEST1", "TEST2", "TEST3"]
// successCount is 3
```

### Reset
Resets the processor to its initial state.

#### Example
```csharp
var processor = new TestClass();
processor.Process("test");
Console.WriteLine(processor.ProcessedCount); // Outputs: 1
processor.Reset();
Console.WriteLine(processor.ProcessedCount); // Outputs: 0
```

## Examples
```csharp
// Create a processor in strict mode
var processor = new TestClass(strictMode: true);
processor.MaxLength = 10;

// Process some strings
if (processor.Validate("Hello"))
{
    string result = processor.Process("Hello");    // Returns "HELLO"
    Console.WriteLine(processor.ProcessedCount);   // Outputs: 1
    Console.WriteLine(processor.LastMessage);      // Outputs: "HELLO"
}

// Batch processing
string[] inputs = new[] { "test1", "test2", "too long string" };
string[] results = new string[3];
int succeeded = processor.TryProcessBatch(inputs, results);
// succeeded will be 2, as the third string exceeds MaxLength
```

## See Also
- [IDataProcessor<T>](IDataProcessor.md)

## Inheritance
Implements @IDataProcessor<string> interface.

## References
- See @System.String for string handling details
- See @System.ArgumentException for error conditions
