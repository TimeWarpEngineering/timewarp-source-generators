#region Purpose
// Decides whether the consuming repository owns a syntax tree, so TWA0038 does not report .cs files
// a NuGet package compiles in (contentFiles, or a Compile item from the package's build props).
#endregion

#region Design
// Inputs are build_property keys made visible by build/TimeWarp.SourceGenerators.props:
// NuGetPackageRoot, ProjectDir, RepositoryRoot (a TimeWarp Directory.Build.props convention).
// Each key may be absent (a consumer that excludes build assets, or a test), so every rule degrades:
// - Package root: under NuGetPackageRoot, or a "/.nuget/packages/" segment when the key is absent.
// - Build output: an obj/ or goldens/ segment.
// - Outside roots: only with both ProjectDir and RepositoryRoot known, a rooted tree under neither is
//   not owned. With either key absent the rule is off, so a linked file elsewhere in the repo still
//   warns; package content then relies on the package-root rule alone.
// exclude_paths globs: "**" spans segments, "*" and "?" stay inside one segment. A pattern matches
// the path suffix at a segment boundary, so "tools/legacy/*.cs" works without knowing the root.
// No file IO (EnforceExtendedAnalyzerRules): "." and ".." are collapsed lexically.
#endregion

namespace TimeWarp.SourceGenerators;

/// <summary>Path rules that decide whether the consuming repository owns a source file.</summary>
internal static class SourceFileOwnership
{
  /// <summary>Global option holding the NuGet global packages folder.</summary>
  public const string NuGetPackageRootKey = "build_property.NuGetPackageRoot";

  /// <summary>Global option holding the consuming project's directory.</summary>
  public const string ProjectDirKey = "build_property.ProjectDir";

  /// <summary>Global option holding the consuming repository root (TimeWarp convention).</summary>
  public const string RepositoryRootKey = "build_property.RepositoryRoot";

  private static readonly TimeSpan MatchTimeout = TimeSpan.FromSeconds(1);

  /// <summary>Roots read from the options; each is null when its key is absent or empty.</summary>
  internal sealed record Roots(string? NuGetPackageRoot, string? ProjectDir, string? RepositoryRoot);

  /// <summary>Reads the roots from the tree options, then the global options.</summary>
  public static Roots ReadRoots(AnalyzerConfigOptions treeOptions, AnalyzerConfigOptions globalOptions) =>
    new
    (
      ReadDirectory(treeOptions, globalOptions, NuGetPackageRootKey),
      ReadDirectory(treeOptions, globalOptions, ProjectDirKey),
      ReadDirectory(treeOptions, globalOptions, RepositoryRootKey)
    );

  /// <summary>True when the consuming repository owns <paramref name="path"/>.</summary>
  public static bool IsOwned(string? path, Roots roots)
  {
    if (string.IsNullOrEmpty(path)) return true;

    string normalized = Normalize(path!);
    if (IsBuildOutput(normalized)) return false;
    if (IsPackageContent(normalized, roots.NuGetPackageRoot)) return false;
    return !IsOutsideRoots(normalized, roots);
  }

  /// <summary>True when the path sits under an obj/ or goldens/ directory.</summary>
  public static bool IsBuildOutput(string normalized)
  {
    // Leading "/" lets a relative "obj/..." path match the same segment test as an absolute one.
    string segmented = "/" + normalized;
    return segmented.Contains("/obj/", StringComparison.Ordinal)
      || segmented.Contains("/goldens/", StringComparison.Ordinal);
  }

  /// <summary>True when any glob in the <c>;</c>- or <c>,</c>-separated list matches the path.</summary>
  public static bool MatchesAnyGlob(string? path, string? globList)
  {
    if (string.IsNullOrEmpty(path) || string.IsNullOrWhiteSpace(globList)) return false;

    string normalized = Normalize(path!);
    foreach (string glob in globList!.Split([';', ','], StringSplitOptions.RemoveEmptyEntries))
    {
      string trimmed = glob.Trim();
      if (trimmed.Length == 0) continue;

      if (Regex.IsMatch(normalized, GlobToRegex(trimmed), RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, MatchTimeout))
        return true;
    }

    return false;
  }

  /// <summary>Converts a glob to a regex anchored at a segment boundary and the path end.</summary>
  internal static string GlobToRegex(string glob)
  {
    string pattern = glob.Replace('\\', '/');
    bool anchored = pattern.StartsWith("/", StringComparison.Ordinal);
    StringBuilder builder = new(anchored ? "^" : "(?:^|/)");

    int index = 0;
    while (index < pattern.Length)
    {
      char current = pattern[index];
      if (current == '*' && index + 1 < pattern.Length && pattern[index + 1] == '*')
      {
        bool slashFollows = index + 2 < pattern.Length && pattern[index + 2] == '/';
        builder.Append(slashFollows ? "(?:.*/)?" : ".*");
        index += slashFollows ? 3 : 2;
        continue;
      }

      builder.Append(current switch
      {
        '*' => "[^/]*",
        '?' => "[^/]",
        _ => Regex.Escape(current.ToString())
      });
      index++;
    }

    builder.Append('$');
    return builder.ToString();
  }

  /// <summary>Forward slashes, with "." and ".." segments collapsed lexically.</summary>
  internal static string Normalize(string path)
  {
    string slashed = path.Replace('\\', '/');
    bool rooted = slashed.StartsWith("/", StringComparison.Ordinal);
    List<string> segments = [];
    foreach (string segment in slashed.Split('/'))
    {
      if (segment.Length == 0 || segment == ".") continue;
      if (segment == ".." && segments.Count > 0 && segments[segments.Count - 1] != "..")
      {
        segments.RemoveAt(segments.Count - 1);
        continue;
      }

      segments.Add(segment);
    }

    string joined = string.Join("/", segments);
    return rooted ? "/" + joined : joined;
  }

  private static bool IsPackageContent(string normalized, string? nuGetPackageRoot)
  {
    if (nuGetPackageRoot is not null) return IsUnder(normalized, nuGetPackageRoot);

    return ("/" + normalized).Contains("/.nuget/packages/", StringComparison.OrdinalIgnoreCase);
  }

  private static bool IsOutsideRoots(string normalized, Roots roots)
  {
    // Both keys are required: ProjectDir is always set by the SDK, but RepositoryRoot only by a
    // Directory.Build.props convention. ProjectDir alone would drop owned linked files.
    if (roots.ProjectDir is null || roots.RepositoryRoot is null) return false;
    if (!IsRooted(normalized)) return false;

    return !IsUnder(normalized, roots.ProjectDir) && !IsUnder(normalized, roots.RepositoryRoot);
  }

  private static bool IsRooted(string normalized) =>
    normalized.StartsWith("/", StringComparison.Ordinal)
    || (normalized.Length >= 2 && normalized[1] == ':');

  private static bool IsUnder(string normalized, string directory) =>
    normalized.StartsWith(directory, StringComparison.OrdinalIgnoreCase);

  private static string? ReadDirectory(AnalyzerConfigOptions treeOptions, AnalyzerConfigOptions globalOptions, string key)
  {
    if (!treeOptions.TryGetValue(key, out string? value) || string.IsNullOrWhiteSpace(value))
      globalOptions.TryGetValue(key, out value);

    if (string.IsNullOrWhiteSpace(value)) return null;

    string normalized = Normalize(value!.Trim());
    if (normalized.Length == 0) return null;
    return normalized.EndsWith("/", StringComparison.Ordinal) ? normalized : normalized + "/";
  }
}
