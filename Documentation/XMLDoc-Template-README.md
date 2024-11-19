# XMLDoc Markdown Template Guide

This guide explains how to use the markdown template for generating XMLDoc documentation.

## Template Structure

The template provides a standardized way to write documentation that maps directly to C# XMLDoc comments. The structure is hierarchical, separating class-level and member-level documentation.

### Class/Type Level Documentation

```markdown
# TypeName              <!-- Type name -->
Main description        <!-- Type-level <summary> -->

## Type Parameters      <!-- Type-level <typeparam> -->
## Remarks             <!-- Type-level <remarks> -->
## Examples            <!-- Type-level examples -->
## See Also            <!-- Type-level <seealso> -->
## Inheritance         <!-- Type-level <inheritdoc> -->
```

### Property Documentation

```markdown
## Properties

### PropertyName
Description            <!-- Property-level <summary> -->

#### Value            <!-- Property <value> -->
Value description
```

### Method Documentation

```markdown
## Methods

### MethodName
Description            <!-- Method-level <summary> -->

#### Parameters       <!-- Method-level <param> -->
#### Returns         <!-- Method-level <returns> -->
#### Exceptions      <!-- Method-level <exception> -->
#### Example         <!-- Method-level <example> -->
```

## XMLDoc Mapping

| Markdown Section | XMLDoc Element | Scope | Description |
|-----------------|----------------|-------|-------------|
| # TypeName + Description | `<summary>` | Type | Type overview |
| ## Type Parameters | `<typeparam>` | Type | Generic type parameters |
| ## Remarks | `<remarks>` | Type | Additional type details |
| ### PropertyName + Description | `<summary>` | Property | Property overview |
| #### Value | `<value>` | Property | Property value description |
| ### MethodName + Description | `<summary>` | Method | Method overview |
| #### Parameters | `<param>` | Method | Method parameters |
| #### Returns | `<returns>` | Method | Method return value |
| #### Exceptions | `<exception>` | Method/Type | Thrown exceptions |
| #### Example | `<example>` | Method/Type | Usage examples |

## Cross-References

- Use `@TypeName` to reference types
- Use `@TypeName.MemberName` to reference members
- Use `[LinkText](TypeName.md)` for documentation links

## Code Examples

Use fenced code blocks with language specifier:
````markdown
```csharp
// Your code here
```
````

## Best Practices

1. **Hierarchy**: Maintain proper section hierarchy (# → ## → ### → ####)
2. **Scope**: Keep documentation at appropriate scope (type vs member level)
3. **Completeness**: Include all relevant sections for each member
4. **Examples**: Provide practical examples at both type and method levels
5. **Cross-References**: Use proper syntax for type and member references

## Example Usage

See [XMLDoc-Template-Example.md](XMLDoc-Template-Example.md) for a complete example showing:
- Class-level documentation
- Property documentation
- Method documentation with parameters and returns
- Practical examples and cross-references

## Notes

- Sections can be omitted if not applicable
- Order of sections should follow the template for consistency
- Additional custom sections can be added after the standard sections
- Member-level documentation should be more specific than type-level
