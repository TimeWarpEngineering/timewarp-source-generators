using Microsoft.CodeAnalysis;

namespace TimeWarp.SourceGenerators;

[Generator]
public class HelloWorldGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register a simple diagnostic
    context.ReportDiagnostic
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

    // Generate a simple class
    context.RegisterPostInitializationOutput
    (
      (context) =>
      {
        context.AddSource
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
