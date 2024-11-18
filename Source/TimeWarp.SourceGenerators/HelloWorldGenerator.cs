using Microsoft.CodeAnalysis;

namespace TimeWarp.SourceGenerators;

[Generator]
public class HelloWorldGenerator : IIncrementalGenerator
{
  private static readonly DiagnosticDescriptor HelloWorldGeneratorLoadedDescriptor = new(
    id: "TW0001",
    title: "HelloWorld Generator Loaded",
    messageFormat: "The HelloWorld generator has been loaded and initialized",
    category: "SourceGenerator",
    DiagnosticSeverity.Warning, // Changed from Info to Warning to make it more visible
    isEnabledByDefault: true
  );

  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Create a pipeline that always produces a single value to trigger the diagnostic
    IncrementalValueProvider<bool> initializationTrigger = context.CompilationProvider
      .Select((compilation, _) => true);

    // Register action to report diagnostic
    context.RegisterSourceOutput(initializationTrigger, (sourceContext, _) =>
    {
      sourceContext.ReportDiagnostic(
        Diagnostic.Create(HelloWorldGeneratorLoadedDescriptor, Location.None)
      );
    });

    // Register source generation
    context.RegisterPostInitializationOutput(ctx =>
    {
      ctx.AddSource
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
    });
  }
}
