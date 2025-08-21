# Analyzer Release Tracking Strategy

## Overview

This project uses a simplified approach to analyzer release tracking by keeping all rules in the "unshipped" state indefinitely. This document explains why this approach makes sense for many projects.

## The Traditional Approach

The Roslyn analyzer infrastructure provides two files for tracking analyzer releases:
- `AnalyzerReleases.Unshipped.md` - New rules not yet released
- `AnalyzerReleases.Shipped.md` - Rules that have been released in a NuGet package

When you publish a NuGet package, rules traditionally move from Unshipped to Shipped, creating a permanent record of:
- What rules were added in each release
- The rule's ID, category, and default severity
- Prevention of breaking changes to shipped rules

## The "Always Unshipped" Approach

For many projects, especially those in active development or internal use, keeping all rules in Unshipped provides several benefits:

### Benefits

1. **Flexibility** - You can change rule severity, category, or behavior without RS2001 errors
2. **Simplicity** - No need to manage release tracking or move rules between files
3. **Experimentation** - Easy to iterate on analyzer design without commitment
4. **Internal Use** - Perfect for analyzers used within a team or organization

### When This Makes Sense

- Internal or team-specific analyzers
- Experimental or evolving analyzer rules
- Projects where backward compatibility isn't critical
- Analyzers distributed through source rather than NuGet
- During initial development and testing phases

### Trade-offs

- No formal release history
- No enforcement of backward compatibility
- May confuse users expecting traditional versioning

## Implementation

### Disable RS2001 Warnings

To prevent warnings about changing "shipped" rules, add to your analyzer project:

```xml
<PropertyGroup>
  <NoWarn>RS2001</NoWarn>
</PropertyGroup>
```

### Document Your Approach

Make it clear in your README that analyzer rules may change:

```markdown
## Analyzer Versioning

This project keeps all analyzer rules in "unshipped" status to allow for flexibility during development. 
Rule IDs, severities, and behaviors may change between versions. 
Pin to specific versions if you need stability.
```

### Example Structure

```
AnalyzerReleases.Unshipped.md:
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
TWG001 | SourceGenerator | Info | MarkdownDocsGenerator  
TWA001 | Naming | Info | FileNameRuleAnalyzer, disabled by default
TWA002 | Documentation | Info | XmlDocsToMarkdownAnalyzer

AnalyzerReleases.Shipped.md:
## Release 1.0
(empty - we keep everything unshipped)
```

## When to Use Traditional Tracking

Consider moving to traditional shipped/unshipped tracking when:

1. **Public NuGet Package** - You're publishing for wide consumption
2. **Stable API** - Your analyzers have stabilized
3. **Enterprise Use** - You need formal change tracking
4. **Breaking Change Management** - You need to communicate changes clearly

## Migration Path

If you later decide to adopt traditional tracking:

1. Determine your stable rule set
2. Move rules to Shipped with a version number
3. Remove the RS2001 suppression
4. Follow standard practices going forward

## Best Practices

1. **Document Everything** - Be clear about your approach
2. **Use Semantic Versioning** - Even without shipped tracking
3. **Communicate Changes** - Use release notes
4. **Consider Your Users** - Internal vs. external usage

## Conclusion

The "always unshipped" approach trades formal tracking for flexibility. It's a pragmatic choice for many scenarios, especially during active development. The key is being intentional and transparent about your choice.