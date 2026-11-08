namespace TimeWarp.SourceGenerators;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(FileNameRuleCodeFixProvider)), Shared]
public class FileNameRuleCodeFixProvider : CodeFixProvider
{
  private const string Title = "Rename file to kebab-case";

  public sealed override ImmutableArray<string> FixableDiagnosticIds => 
    ImmutableArray.Create(FileNameRuleAnalyzer.DiagnosticId);

  public sealed override FixAllProvider GetFixAllProvider() => 
    WellKnownFixAllProviders.BatchFixer;

  public sealed override Task RegisterCodeFixesAsync(CodeFixContext context)
  {
    var diagnostic = context.Diagnostics.First();
    var document = context.Document;
    
    // Get the current file name
    var filePath = document.FilePath;
    if (string.IsNullOrEmpty(filePath))
      return;
      
    var currentFileName = Path.GetFileName(filePath);
    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(currentFileName);
    var extension = Path.GetExtension(currentFileName);
    
    // Convert to kebab-case
    var newFileNameWithoutExtension = ConvertToKebabCase(fileNameWithoutExtension);
    var newFileName = newFileNameWithoutExtension + extension;
    
    // Only offer fix if the new name is different
    if (newFileName == currentFileName)
      return;
    
    // Register the code fix
    context.RegisterCodeFix(
      CodeAction.Create(
        title: $"{Title}: '{newFileName}'",
        createChangedSolution: c => RenameFileAsync(document, newFileName, c),
        equivalenceKey: Title),
      diagnostic);
      
    return Task.CompletedTask;
  }

  private async Task<Solution> RenameFileAsync(
    Document document, 
    string newFileName, 
    CancellationToken cancellationToken)
  {
    var solution = document.Project.Solution;
    
    // Get the new document with renamed file
    var newDocument = document.WithName(newFileName);
    
    // Remove old document and add new one
    var newSolution = solution
      .RemoveDocument(document.Id)
      .AddDocument(
        newDocument.Id,
        newFileName,
        await document.GetTextAsync(cancellationToken),
        document.Folders,
        document.FilePath != null ? Path.Combine(Path.GetDirectoryName(document.FilePath)!, newFileName) : null);
    
    return newSolution;
  }

  private static string ConvertToKebabCase(string input)
  {
    if (string.IsNullOrEmpty(input))
      return input;
    
    // Handle already kebab-case
    if (Regex.IsMatch(input, @"^[a-z][a-z0-9]*(?:-[a-z0-9]+)*$"))
      return input;
    
    var result = new StringBuilder();
    bool previousWasUpper = false;
    bool previousWasNumber = false;
    
    for (int i = 0; i < input.Length; i++)
    {
      char c = input[i];
      
      if (char.IsUpper(c))
      {
        // Add hyphen before uppercase letter if:
        // - Not at start
        // - Previous char was lowercase or number
        // - Or this is start of new word in acronym (next char is lowercase)
        if (i > 0 && (!previousWasUpper || (i + 1 < input.Length && char.IsLower(input[i + 1]))))
        {
          result.Append('-');
        }
        
        result.Append(char.ToLowerInvariant(c));
        previousWasUpper = true;
        previousWasNumber = false;
      }
      else if (char.IsDigit(c))
      {
        // Add hyphen before number if previous wasn't number and we're not at start
        if (i > 0 && !previousWasNumber)
        {
          result.Append('-');
        }
        
        result.Append(c);
        previousWasUpper = false;
        previousWasNumber = true;
      }
      else if (char.IsLower(c))
      {
        result.Append(c);
        previousWasUpper = false;
        previousWasNumber = false;
      }
      else if (c == '-' || c == '_')
      {
        // Replace underscores with hyphens, avoid double hyphens
        if (result.Length > 0 && result[result.Length - 1] != '-')
        {
          result.Append('-');
        }
        previousWasUpper = false;
        previousWasNumber = false;
      }
    }
    
    // Clean up any double hyphens or trailing hyphens
    var cleaned = Regex.Replace(result.ToString(), @"-+", "-");
    cleaned = cleaned.Trim('-');
    
    return cleaned;
  }
}