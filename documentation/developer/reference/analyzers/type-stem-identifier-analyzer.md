# TypeStemIdentifierAnalyzer (TWA0023)

## Overview

For a named type that already names the role, the identifier is the type name. Interfaces drop a leading `I` when the second character is uppercase. Two instances keep the type as the head (`CatalogHttpClient`, `BillingHttpClient`). Casing does not matter: `HttpClient` and `httpClient` both pass.

This rule moved from `TimeWarp.Architecture.Analyzers`. The id is still **TWA0023**. It ships disabled.

The opt-out attribute stays in `TimeWarp.Architecture.Attributes`. The analyzer matches the simple name `TypeStemIdentifierAttribute` and requires a non-empty reason. It does not reference that package.

## Rule Details

- **Rule ID**: TWA0023
- **Category**: Naming
- **Default Severity**: Warning (disabled)
- **Message**: Identifier '{0}' must end with type stem '{1}' (the type name already names the role; qualify with a prefix if there are two of this type)

```ini
[*.cs]
dotnet_diagnostic.TWA0023.severity = warning
```

## Checked

Fields, properties (not indexers), parameters (methods, constructors, primary constructors, lambdas), locals, out-var and deconstruction designations, `foreach`, and `catch`.

## Not checked

Primitives and untyped boxes (`string`, `List<T>`, `Task`, and the rest of that skip set), arrays, discards, method and type names, overrides, explicit interface implementations, setter `value`, extension `this`, enum members, and compiler-generated names.

```csharp
[TypeStemIdentifier("global collision with System.Terminal")]
private readonly TimeWarpTerminal Terminal;
```

An empty or whitespace reason does not opt out.

## Suppression

```csharp
#pragma warning disable TWA0023
```

```ini
dotnet_diagnostic.TWA0023.severity = none
```
