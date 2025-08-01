namespace TimeWarp.SourceGenerators;

/// <summary>
/// A simple source generator that generates a HelloWorld class and reports a diagnostic when loaded.
/// The Info diagnostic (TW0001) can be viewed in:
/// 1. Visual Studio: View -> Output Window -> Show output from: Build
/// 2. Visual Studio: View -> Error List (make sure "Information" messages are enabled)
/// 3. Visual Studio: View -> Other Windows -> Source Generator Diagnostics Window
/// 
/// Note: Info level diagnostics may not be visible in command-line build output.
/// To verify the generator is working from CLI:
/// 1. Check that HelloWorld.g.cs is generated in the project's Generated folder
/// 2. Run the test console app to see the generated message: dotnet run --project Tests/TimeWarp.SourceGenerators.TestConsole
/// </summary>
[Generator]
public class HelloWorldGenerator : IIncrementalGenerator
{
  private static readonly DiagnosticDescriptor HelloWorldGeneratorLoadedDescriptor = new(
    id: "TW0001",
    title: "HelloWorld Generator Loaded",
    messageFormat: "The HelloWorld generator has been loaded and initialized",
    category: "SourceGenerator",
    // Using Info severity as this is informational output
    // Best viewed in Visual Studio's Output Window or Error List
    DiagnosticSeverity.Info,
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
