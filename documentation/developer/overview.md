# Developer Documentation

## Structure

This documentation follows a clear hierarchy:

### [Conceptual](./conceptual/overview.md)
Understanding the design decisions and philosophy:
- [Analyzer Release Tracking](./conceptual/analyzer-release-tracking.md) - Why we keep rules "unshipped"

### [How-to Guides](./how-to-guides/overview.md)
Step-by-step instructions:
- [Configure File Name Analyzer](./how-to-guides/configure-file-name-analyzer.md)

### [Reference](./reference/overview.md)
Detailed technical documentation:
- [Analyzers](./reference/analyzers/overview.md)
  - [File Name Rule Analyzer](./reference/analyzers/file-name-rule-analyzer.md)
- [Source Generators](./reference/source-generators/overview.md)
  - XMLDoc Templates

### [Roadmap](./roadmap.md)
Future plans and feature ideas

## Quick Links

- Configure analyzers: `.editorconfig`
- Add analyzer: Reference the NuGet package
- Suppress warnings: Use `#pragma` or `.editorconfig`

## Principles

1. **Kebab-case everywhere** - All files and directories use kebab-case
2. **Opt-in by default** - Features start disabled
3. **Performance first** - Built with incremental generators
4. **Flexible configuration** - Full control via `.editorconfig`