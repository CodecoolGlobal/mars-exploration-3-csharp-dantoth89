using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.Model;

public record ConfigurationModel(string MapFilePath, Coordinate LandingSpot, IEnumerable<string> NeededResourcesSymbols, int TimeoutSteps);


