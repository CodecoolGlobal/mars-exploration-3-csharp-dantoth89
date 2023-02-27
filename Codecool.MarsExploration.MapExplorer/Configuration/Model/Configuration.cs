using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.Model;

public record Configuration(string MapFilePath, Coordinate LandingSpot, IEnumerable<string> NeededResourcesSymbols, int TimeoutSteps);


