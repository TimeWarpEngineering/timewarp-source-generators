#region Purpose
// Turns a matching markdown file into XML doc comments and partial members.
#endregion
#region Design
// Split from MarkdownDocsGenerator so the generator file stays under the 500-line cap.
// Matching and source emission stay on the generator; this file owns markdown parsing.
#endregion

namespace TimeWarp.SourceGenerators;

public partial class MarkdownDocsGenerator
{
  private static (string ClassDocs, string MethodDocs, string PropertyDocs) ConvertMarkdownToXmlDocs(string markdownContent, ClassDeclarationSyntax classDeclaration)
  {
    var classBuilder = new StringBuilder();
    var methodBuilder = new StringBuilder();
    var propertyBuilder = new StringBuilder();
    var reader = new StringReader(markdownContent);
    string? line;

    // State tracking
    var currentSection = "";
    var contentBuilder = new StringBuilder();
    var inMethodSection = false;
    var inPropertySection = false;
    var currentMethod = "";
    var currentMethodDescription = "";
    var currentProperty = "";
    var currentPropertyDescription = "";

    // Get method signatures from the class declaration
    var methodSignatures = classDeclaration.Members
        .OfType<MethodDeclarationSyntax>()
        .ToDictionary(
            m => m.Identifier.Text,
            m => $"public partial {m.ReturnType} {m.Identifier}({string.Join(", ", m.ParameterList.Parameters.Select(p => $"{p.Type} {p.Identifier}{(p.Default != null ? " = " + p.Default.Value.ToString() : "")}"))})"
        );

    // Get property signatures from the class declaration (only partial properties)
    var propertySignatures = classDeclaration.Members
        .OfType<PropertyDeclarationSyntax>()
        .Where(p => p.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
        .ToDictionary(
            p => p.Identifier.Text,
            p =>
            {
              var modifiers = string.Join(" ", p.Modifiers.Select(m => m.Text));
              var accessors = new List<string>();

              if (p.AccessorList != null)
              {
                foreach (var accessor in p.AccessorList.Accessors)
                {
                  if (accessor.Keyword.Text == "get")
                  {
                    // For non-nullable reference types, initialize with default
                    var typeStr = p.Type?.ToString() ?? "";
                    if (typeStr == "string" && !typeStr.EndsWith("?"))
                    {
                      accessors.Add("get => field ??= \"\"");
                    }
                    else
                    {
                      accessors.Add("get => field");
                    }
                  }
                  else if (accessor.Keyword.Text == "set")
                  {
                    accessors.Add("set => field = value");
                  }
                  else if (accessor.Keyword.Text == "init")
                  {
                    accessors.Add("init => field = value");
                  }
                }
              }

              var accessorString = string.Join("; ", accessors);
              return $"{modifiers} {p.Type} {p.Identifier} {{ {accessorString}; }}";
            }
        );

    while ((line = reader.ReadLine()) != null)
    {
      if (line.StartsWith("# ")) // Class name - skip
        continue;

      if (line.StartsWith("## "))
      {
        // Process previous section
        if (inMethodSection)
          ProcessMethodSection(methodBuilder, currentMethod, currentMethodDescription, contentBuilder.ToString().Trim(), methodSignatures);
        else if (inPropertySection)
          ProcessPropertySection(propertyBuilder, currentProperty, currentPropertyDescription, contentBuilder.ToString().Trim(), propertySignatures);
        else
          ProcessSection(classBuilder, currentSection, contentBuilder.ToString().Trim());

        // Start new section
        currentSection = line.Substring(3).Trim();
        inMethodSection = currentSection == "Methods";
        inPropertySection = currentSection == "Properties";
        contentBuilder.Clear();
        currentMethodDescription = "";
        currentPropertyDescription = "";
        continue;
      }

      if (line.StartsWith("### ") && inMethodSection)
      {
        // Process previous method if exists
        if (!string.IsNullOrEmpty(currentMethod))
        {
          ProcessMethodSection(methodBuilder, currentMethod, currentMethodDescription, contentBuilder.ToString().Trim(), methodSignatures);
        }

        // Start new method
        currentMethod = line.Substring(4).Trim();
        contentBuilder.Clear();
        currentMethodDescription = "";
        continue;
      }

      if (line.StartsWith("### ") && inPropertySection)
      {
        // Process previous property if exists
        if (!string.IsNullOrEmpty(currentProperty))
        {
          ProcessPropertySection(propertyBuilder, currentProperty, currentPropertyDescription, contentBuilder.ToString().Trim(), propertySignatures);
        }

        // Start new property
        currentProperty = line.Substring(4).Trim();
        contentBuilder.Clear();
        currentPropertyDescription = "";
        continue;
      }

      // If we haven't hit any section yet, this is the class summary
      if (string.IsNullOrEmpty(currentSection) && !string.IsNullOrWhiteSpace(line))
      {
        classBuilder.AppendLine("/// <summary>");
        classBuilder.AppendLine($"/// {line.Trim()}");
        classBuilder.AppendLine("/// </summary>");
        continue;
      }

      // Capture method description (text before any #### subsections)
      if (inMethodSection && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("####") && string.IsNullOrEmpty(currentMethodDescription))
      {
        currentMethodDescription = line.Trim();
        continue;
      }

      // Capture property description (text before any #### subsections)
      if (inPropertySection && !string.IsNullOrWhiteSpace(line) && !line.StartsWith("####") && string.IsNullOrEmpty(currentPropertyDescription))
      {
        currentPropertyDescription = line.Trim();
        continue;
      }

      // Add content to current section
      if (!string.IsNullOrWhiteSpace(line))
      {
        contentBuilder.AppendLine(line);
      }
    }

    // Process the last section
    if (inMethodSection)
      ProcessMethodSection(methodBuilder, currentMethod, currentMethodDescription, contentBuilder.ToString().Trim(), methodSignatures);
    else if (inPropertySection)
      ProcessPropertySection(propertyBuilder, currentProperty, currentPropertyDescription, contentBuilder.ToString().Trim(), propertySignatures);
    else
      ProcessSection(classBuilder, currentSection, contentBuilder.ToString().Trim());

    return (classBuilder.ToString(), methodBuilder.ToString(), propertyBuilder.ToString());
  }

  private static void ProcessSection(StringBuilder builder, string section, string content)
  {
    if (string.IsNullOrEmpty(content))
      return;

    switch (section)
    {
      case "Remarks":
        builder.AppendLine("/// <remarks>");
        foreach (var line in content.Split('\n'))
        {
          builder.AppendLine($"/// {line.Trim()}");
        }
        builder.AppendLine("/// </remarks>");
        break;

      case "See Also":
        foreach (var line in content.Split('\n'))
        {
          if (string.IsNullOrWhiteSpace(line)) continue;
          var match = Regex.Match(line, @"\[(.*?)\]\((.*?)\)");
          if (match.Success)
          {
            var type = match.Groups[1].Value;
            // Convert angle brackets to curly braces for XML documentation
            var xmlType = type.Replace('<', '{').Replace('>', '}');
            builder.AppendLine($"/// <seealso cref=\"{xmlType}\"/>");
          }
        }
        break;

      case "Inheritance":
        if (content.Contains("@"))
        {
          var match = Regex.Match(content, @"@(\S+)");
          if (match.Success)
          {
            var type = match.Groups[1].Value;
            // Extract the type name and keep T as the generic parameter
            var baseType = type.Split('<')[0];
            builder.AppendLine($"/// <seealso cref=\"{baseType}{{T}}\"/>");
          }
        }
        break;

      case "References":
        foreach (var line in content.Split('\n'))
        {
          if (string.IsNullOrWhiteSpace(line)) continue;
          var match = Regex.Match(line, @"@(\S+)");
          if (match.Success)
          {
            var type = match.Groups[1].Value;
            // Handle fully qualified type names
            if (!type.Contains("."))
            {
              type = "System." + type;
            }
            builder.AppendLine($"/// <see cref=\"{type}\"/>");
          }
        }
        break;
    }
  }

  private static void ProcessMethodSection(StringBuilder builder, string methodName, string description, string content, Dictionary<string, string> methodSignatures)
  {
    if (string.IsNullOrEmpty(methodName))
      return;

    // Handle constructor specially
    if (methodName == "Constructor")
      methodName = methodSignatures.Keys.FirstOrDefault(k => k.Contains("ctor")) ?? "";

    if (!methodSignatures.ContainsKey(methodName))
      return;

    builder.AppendLine($"    // Documentation for {methodName}");

    // Add method summary if available
    if (!string.IsNullOrEmpty(description))
    {
      builder.AppendLine("    /// <summary>");
      builder.AppendLine($"    /// {description}");
      builder.AppendLine("    /// </summary>");
    }

    var reader = new StringReader(content);
    string? line;
    var currentSubSection = "";
    var subSectionContent = new StringBuilder();

    while ((line = reader.ReadLine()) != null)
    {
      if (line.StartsWith("#### "))
      {
        // Process previous subsection
        ProcessMethodSubSection(builder, currentSubSection, subSectionContent.ToString().Trim());

        // Start new subsection
        currentSubSection = line.Substring(5).Trim();
        subSectionContent.Clear();
        continue;
      }

      // Add content to current subsection
      if (!string.IsNullOrWhiteSpace(line))
      {
        subSectionContent.AppendLine(line);
      }
    }

    // Process the last subsection
    ProcessMethodSubSection(builder, currentSubSection, subSectionContent.ToString().Trim());

    // Add the method signature with a semicolon
    builder.AppendLine($"    {methodSignatures[methodName]};");
    builder.AppendLine();
  }

  private static void ProcessMethodSubSection(StringBuilder builder, string subSection, string content)
  {
    if (string.IsNullOrEmpty(content))
      return;

    switch (subSection)
    {
      case "Parameters":
        foreach (var line in content.Split('\n'))
        {
          if (string.IsNullOrWhiteSpace(line)) continue;
          var match = Regex.Match(line, @"`(.*?)`\s*-\s*(.*)");
          if (match.Success)
          {
            builder.AppendLine($"    /// <param name=\"{match.Groups[1].Value}\">{match.Groups[2].Value.Trim()}</param>");
          }
        }
        break;

      case "Returns":
        builder.AppendLine("    /// <returns>");
        foreach (var line in content.Split('\n'))
        {
          builder.AppendLine($"    /// {line.Trim()}");
        }
        builder.AppendLine("    /// </returns>");
        break;

      case "Exceptions":
        foreach (var line in content.Split('\n'))
        {
          if (string.IsNullOrWhiteSpace(line)) continue;
          var match = Regex.Match(line, @"`(.*?)`\s*-\s*(.*)");
          if (match.Success)
          {
            builder.AppendLine($"    /// <exception cref=\"{match.Groups[1].Value}\">{match.Groups[2].Value.Trim()}</exception>");
          }
        }
        break;
    }
  }

  private static void ProcessPropertySection(StringBuilder builder, string propertyName, string description, string content, Dictionary<string, string> propertySignatures)
  {
    if (string.IsNullOrEmpty(propertyName))
      return;

    if (!propertySignatures.ContainsKey(propertyName))
      return;

    builder.AppendLine($"    // Documentation for {propertyName}");

    // Add property summary if available
    if (!string.IsNullOrEmpty(description))
    {
      builder.AppendLine("    /// <summary>");
      builder.AppendLine($"    /// {description}");
      builder.AppendLine("    /// </summary>");
    }

    var reader = new StringReader(content);
    string? line;
    var currentSubSection = "";
    var subSectionContent = new StringBuilder();

    while ((line = reader.ReadLine()) != null)
    {
      if (line.StartsWith("#### "))
      {
        // Process previous subsection
        ProcessPropertySubSection(builder, currentSubSection, subSectionContent.ToString().Trim());

        // Start new subsection
        currentSubSection = line.Substring(5).Trim();
        subSectionContent.Clear();
        continue;
      }

      // Add content to current subsection
      if (!string.IsNullOrWhiteSpace(line))
      {
        subSectionContent.AppendLine(line);
      }
    }

    // Process the last subsection
    ProcessPropertySubSection(builder, currentSubSection, subSectionContent.ToString().Trim());

    // Add the property signature
    builder.AppendLine($"    {propertySignatures[propertyName]}");
    builder.AppendLine();
  }

  private static void ProcessPropertySubSection(StringBuilder builder, string subSection, string content)
  {
    if (string.IsNullOrEmpty(content))
      return;

    switch (subSection)
    {
      case "Value":
        builder.AppendLine("    /// <value>");
        foreach (var line in content.Split('\n'))
        {
          builder.AppendLine($"    /// {line.Trim()}");
        }
        builder.AppendLine("    /// </value>");
        break;
    }
  }
}
