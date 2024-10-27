using Microsoft.CodeAnalysis;

namespace TimeWarp.SourceGenerators;

[Generator]
public class HelloWorldGenerator : IIncrementalGenerator
{
  public void Initialize(IncrementalGeneratorInitializationContext context)
  {
    // Register source generation
    context.RegisterPostInitializationOutput
    (
      (postContext) =>
      {
        postContext.AddSource
        (
          "HelloWorld.g.cs",
          """
          // TW0001: The HelloWorld generator has been loaded and initialized
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
