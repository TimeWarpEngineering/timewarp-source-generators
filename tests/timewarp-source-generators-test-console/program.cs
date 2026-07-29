using TimeWarp.SourceGenerators.TestConsole;

// Test kebab-case file matching
var kebabTest = new KebabCaseTest();
Console.WriteLine($"Kebab-case test: {kebabTest.GetTestMessage()}");

// Multi-dot kebab-case partial fixtures (TW0001)
Console.WriteLine($"Multi-dot close-modal: {ApplicationStateCloseModal.ActionName}");
Console.WriteLine($"Multi-dot fetch-weather: {WeatherForecastsStateFetchWeatherForecasts.ActionName}");

// Test PascalCase file matching (backward compatibility)
var pascalTest = new PascalCaseTest();
Console.WriteLine($"PascalCase test: {pascalTest.GetPascalMessage()}");
