using Microsoft.CodeAnalysis;

namespace TimeWarp.SourceGenerators;

[Generator]
public class HelloWorldGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register a simple diagnostic and generate source
    context.RegisterPostInitializationOutput
    (
      (postContext) =>
      {
        postContext.ReportDiagnostic
        (
          Diagnostic.Create
          (
            new DiagnosticDescriptor
            (
              id: "TW0001",
              title: "HelloWorld Generator Loaded", 
              messageFormat: "The HelloWorld generator has been loaded and initialized",
              category: "TimeWarp.SourceGenerators",
              DiagnosticSeverity.Info,
              isEnabledByDefault: true
            ),
            Location.None
          )
        );

        postContext.AddSource
        (
          "HelloWorld.g.cs",
          """
          namespace TimeWarp.Generated;
          
          public static class HelloWorld 
          {
            public const string Message = "Hello from TimeWarp Source Generator!";
          }
          """
        );
      }
    );
  }
}
