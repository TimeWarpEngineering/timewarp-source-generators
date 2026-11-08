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
This package generates no source. TW0001 runs as an incremental generator that only reports diagnostics, and TW0007 is a diagnostic analyzer. XML documentation stays in the C# file.

## API Reference

### DiagnosticDescriptor Properties
- **Id**: Unique identifier (e.g., TW0001)
- **Category**: Grouping for related rules
- **Severity**: Default severity level
- **IsEnabledByDefault**: Whether active without configuration

### Configuration Keys
- `dotnet_diagnostic.<RuleId>.severity`
- `dotnet_diagnostic.<RuleId>.excluded_files`