# Conceptual Documentation

## Overview

This section covers the design philosophy and architectural decisions behind TimeWarp Source Generators.

## Topics

### [Analyzer Release Tracking](./analyzer-release-tracking.md)
Understanding why we keep all analyzer rules in "unshipped" status and the benefits of this approach.

## Core Concepts

### Analyzers
This package generates no source. TW0001 runs as an incremental generator that only reports diagnostics, and TW0007 is a diagnostic analyzer. XML documentation stays in the C# file.

### Opt-in Design
Features are disabled by default to avoid breaking existing code.

### Configuration First
Everything is configurable through `.editorconfig` for maximum flexibility.

### Kebab-case Convention
Following modern web standards, all file and directory names use kebab-case.