# Spikes Folder Overview

## What is a Spike?

A spike is a time-boxed investigation or experiment designed to answer a specific technical question or prove a concept. The term comes from Extreme Programming (XP) and represents a narrow, deep dive into a particular problem.

## Purpose of This Folder

The `spikes` folder contains experimental code and proof-of-concept implementations that:

- **Explore new features** - Test whether a new language feature or API works as expected
- **Validate assumptions** - Verify technical approaches before implementing them in production code
- **Answer questions** - Provide concrete answers to "Can we...?" or "How does...?" questions
- **Reduce risk** - Identify potential problems early before committing to an implementation

## What Belongs Here

- Small, focused code experiments
- Technical investigations
- Proof-of-concept implementations
- Performance benchmarks
- API exploration code
- Feature feasibility tests

## What Doesn't Belong Here

- Production code
- Full implementations
- Long-term maintained code
- Unit tests for production features

## Guidelines

1. **Keep it simple** - Spikes should be minimal code to answer the question
2. **Document findings** - Include comments or markdown files explaining what was learned
3. **Time-box it** - Don't spend too long; if it's taking too long, the approach might be too complex
4. **Delete when done** - Once the knowledge is captured and applied, consider removing old spikes
5. **Name clearly** - Use descriptive names that indicate what question the spike answers

## Example Structure

```
spikes/
├── overview.md (this file)
├── can-we-use-partial-properties/
│   ├── test.cs
│   └── findings.md
├── performance-string-vs-stringbuilder/
│   ├── benchmark.cs
│   └── results.md
└── api-compatibility-check/
    └── test-api-calls.cs
```

## Remember

Spikes are throwaway code. The value is in the learning, not the code itself. Once you've answered your question, apply that knowledge to your production code and document any important findings.