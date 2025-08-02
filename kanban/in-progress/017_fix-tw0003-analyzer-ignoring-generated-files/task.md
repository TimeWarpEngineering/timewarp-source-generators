# Fix TW0003 Analyzer to Ignore Generated Files

## Description
The TW0003 file naming analyzer is incorrectly checking files in build output directories (obj/, bin/) which contain auto-generated build artifacts. The analyzer should skip validation for these directories as they are not user-written source code.

## Problem
The analyzer is reporting errors for generated files such as:
- `.NETCoreApp,Version=v10.0.AssemblyAttributes.cs` in obj/ directory
- `timewarp-code.AssemblyInfo.cs` in obj/ directory

These files are created by the build system and their naming conventions are controlled by the .NET SDK, not the user.

## Acceptance Criteria
- [ ] TW0003 analyzer skips files in `/obj/` directories
- [ ] TW0003 analyzer skips files in `/bin/` directories
- [ ] TW0003 analyzer skips files in other common build output directories
- [ ] User source files continue to be validated correctly
- [ ] Tests verify that generated files are ignored
- [ ] Tests verify that regular source files are still checked

## Technical Details
The fix should be implemented in the TW0003 analyzer by:
1. Checking if the file path contains `/obj/` or `/bin/`
2. Returning early without reporting diagnostics for these paths
3. Consider using normalized path comparison to handle different path separators

## Implementation Location
The fix should be applied in the TW0003 analyzer implementation, likely in the method that determines whether to analyze a given file.

## Test Cases
- Verify files in obj/ directory are ignored
- Verify files in bin/ directory are ignored
- Verify nested obj/bin directories are ignored (e.g., `/src/obj/`)
- Verify regular source files are still validated
- Verify edge cases like files named "obj.cs" in source directories are still checked

## References
- Original issue: `/home/steventcramer/worktrees/github.com/TimeWarpEngineering/timewarp-code/Cramer-2025-07-31-spike/analysis/tw0003-analyzer-issue.md`