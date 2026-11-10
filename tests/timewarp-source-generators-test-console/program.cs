#region Purpose
// Prints TW0001 fixture messages from kebab-case and PascalCase types.
#endregion

// TW0001 fixtures: kebab-case, multi-dot kebab partials, and one excluded PascalCase file.
var kebabTest = new KebabCaseTest();
Console.WriteLine($"Kebab-case test: {kebabTest.GetTestMessage()}");

Console.WriteLine($"Multi-dot close-modal: {ApplicationStateCloseModal.ActionName}");
Console.WriteLine($"Multi-dot fetch-weather: {WeatherForecastsStateFetchWeatherForecasts.ActionName}");

var pascalTest = new PascalCaseTest();
Console.WriteLine($"PascalCase test: {pascalTest.GetPascalMessage()}");
