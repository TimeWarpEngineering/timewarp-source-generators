using TimeWarp.Generated;
using TimeWarp.SourceGenerators.TestConsole;

Console.WriteLine(HelloWorld.Message);

// Test kebab-case file matching
var kebabTest = new KebabCaseTest();
Console.WriteLine($"Kebab-case test: {kebabTest.GetTestMessage()}");

// Test PascalCase file matching (backward compatibility)
var pascalTest = new PascalCaseTest();
Console.WriteLine($"PascalCase test: {pascalTest.GetPascalMessage()}");
