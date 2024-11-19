# Create Markdown Documentation Source Generator

## Description
Create a source generator that takes associated markdown files and generates documentation code files. For each source code file (e.g., MyClass.cs) with an associated markdown file (MyClass.md), generate a documentation file (MyClass.docs.cs) containing the markdown content as code comments.

## Steps
1. Create Documentation Generator
   - [ ] Create new MarkdownDocsGenerator class implementing IIncrementalGenerator
   - [ ] Add diagnostic output for generator initialization
   - [ ] Implement file pair discovery (.cs and .md files with matching names)

2. Implement Source Generation
   - [ ] Read content from markdown file
   - [ ] Convert markdown content to C# comments (prefix each line with //)
   - [ ] Generate .docs.cs file with commented content
   - [ ] Add source to compilation with appropriate naming

## Acceptance Criteria
- [ ] Project builds successfully
- [ ] Given a pair of files:
    ```
    MyClass.cs
    MyClass.md
    ```
  Generator produces:
    ```
    MyClass.docs.cs
    ```
- [ ] Generated file contains markdown content as C# comments
- [ ] Comments are properly formatted (each line prefixed with //)
- [ ] Generator only processes .cs files that have matching .md files

## Notes
- Keep the implementation simple for this step
- Focus only on converting markdown to comments, no special markdown processing
- File naming and matching is critical for proper pairing
