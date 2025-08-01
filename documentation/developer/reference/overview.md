# Reference Documentation

## Overview

Detailed technical reference for all components.

## Categories

### [Analyzers](./analyzers/overview.md)
Complete reference for all code analyzers:
- Configuration options
- Diagnostic messages
- Suppression methods

### [Source Generators](./source-generators/overview.md)
Complete reference for all source generators:
- Generated output structure
- XMLDoc templates
- Implementation patterns

## API Reference

### DiagnosticDescriptor Properties
- **Id**: Unique identifier (e.g., TW0003)
- **Category**: Grouping for related rules
- **Severity**: Default severity level
- **IsEnabledByDefault**: Whether active without configuration

### Configuration Keys
- `dotnet_diagnostic.<RuleId>.severity`
- `dotnet_diagnostic.<RuleId>.excluded_files`