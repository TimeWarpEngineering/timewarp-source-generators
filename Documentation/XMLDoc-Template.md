# [Class/Type Name]

<!-- This section maps to class-level <summary> -->
Brief description of the class/type goes here.

## Type Parameters
<!-- This section maps to class-level <typeparam> -->
- `T` - Description of type parameter T
- `TKey` - Description of type parameter TKey
- `TValue` - Description of type parameter TValue

## Remarks
<!-- This section maps to class-level <remarks> -->
Additional information about the type that supplements the main description.

## Properties

### [PropertyName]
<!-- Each property gets its own subsection -->
Description of the property.

#### Value
<!-- This section maps to property <value> -->
Description of what the property represents or returns.

## Methods

### [MethodName]
<!-- Each method gets its own subsection with its own summary -->
Description of what the method does.

#### Parameters
<!-- Method-specific <param> tags -->
- `parameterName` - Description of the parameter
- `otherParam` - Description of another parameter

#### Returns
<!-- Method-specific <returns> tag -->
Description of what the method returns.

#### Exceptions
<!-- Method-specific <exception> tags -->
- `ArgumentException` - Thrown when an argument is invalid
- `InvalidOperationException` - Thrown when the operation cannot be performed

#### Example
<!-- Method-specific <example> tag -->
```csharp
// Example usage of this specific method
var result = instance.MethodName(42);
```

## Examples
<!-- Class-level examples showing how the type is used -->
```csharp
// Example usage of the class
var instance = new MyClass<int>();
var result = instance.MethodOne(42);
result = instance.MethodTwo("test");
```

## Exceptions
<!-- Class-level exceptions that apply to multiple members -->
- `ObjectDisposedException` - Thrown when the instance is disposed
- `InvalidOperationException` - Thrown when the instance is in an invalid state

## See Also
<!-- This section maps to <seealso> -->
- [RelatedClass](RelatedClass.md)
- [AnotherClass](AnotherClass.md)

## Inheritance
<!-- This section maps to <inheritdoc/> -->
<!-- Use one of the following formats: -->
Inherits documentation from base class/interface.
- `inheritdoc` - Inherit all documentation
- `inheritdoc cref="BaseClass"` - Inherit specific documentation

## References
<!-- Additional cross-references using <see> -->
- See [OtherClass](OtherClass.md) for related functionality
- Implementation details in [ImplementationClass](ImplementationClass.md)

---
<!-- Special syntax for cross-references in text -->
You can reference other types using `@TypeName` or methods using `@TypeName.MethodName`.
For generic types, use `@TypeName<T>` or `@TypeName{T}` format.
