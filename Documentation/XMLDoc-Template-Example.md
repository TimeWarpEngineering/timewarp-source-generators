# JsonSerializer<T>

A strongly-typed JSON serializer that provides custom serialization and deserialization capabilities for type T.

## Type Parameters
- `T` - The type to serialize to and deserialize from JSON. Must be a reference type.

## Remarks
The JsonSerializer provides thread-safe JSON serialization with support for custom converters and formatting options. It handles circular references and maintains object references during serialization.

## Properties

### DefaultSettings
Current serializer settings including formatting options and converters.

#### Value
A JsonSerializerSettings instance containing the current configuration.

### IsIndented
Controls whether the output JSON should be formatted with indentation.

#### Value
True if JSON output should be indented; false for minimal output.

## Methods

### Serialize
Converts an object of type T to its JSON representation.

#### Parameters
- `value` - The object to serialize
- `indent` - Optional. Whether to indent the output. Defaults to the value of IsIndented

#### Returns
A string containing the JSON representation of the object.

#### Exceptions
- `ArgumentNullException` - Thrown when value is null
- `JsonSerializationException` - Thrown when serialization fails

#### Example
```csharp
var serializer = new JsonSerializer<Person>();
var person = new Person { Name = "John", Age = 30 };
string json = serializer.Serialize(person, indent: true);
```

### Deserialize
Converts a JSON string back to an object of type T.

#### Parameters
- `json` - The JSON string to deserialize
- `validateSchema` - Optional. Whether to validate against schema. Defaults to false

#### Returns
An instance of type T created from the JSON string.

#### Exceptions
- `ArgumentNullException` - Thrown when json is null
- `JsonSerializationException` - Thrown when deserialization fails
- `JsonSchemaValidationException` - Thrown when schema validation fails

#### Example
```csharp
var serializer = new JsonSerializer<Person>();
string json = @"{ ""name"": ""John"", ""age"": 30 }";
Person person = serializer.Deserialize(json);
```

## Examples
```csharp
// Create a serializer instance
var serializer = new JsonSerializer<Person>();

// Configure settings
serializer.DefaultSettings.Formatting = Formatting.Indented;
serializer.DefaultSettings.NullValueHandling = NullValueHandling.Ignore;

// Serialize an object
var person = new Person { Name = "John", Age = 30 };
string json = serializer.Serialize(person);

// Deserialize back to an object
Person deserializedPerson = serializer.Deserialize(json);
```

## See Also
- [JsonConverter](JsonConverter.md)
- [JsonSerializerSettings](JsonSerializerSettings.md)

## Inheritance
Implements @IJsonSerializer<T> interface.

## References
- See @JsonDocument for low-level JSON handling
- See @System.Text.Json for alternative serialization options
