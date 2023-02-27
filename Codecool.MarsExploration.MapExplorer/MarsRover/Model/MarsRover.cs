using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Model;

public record MarsRover(string Id, Coordinate CurrentPosition, int Sight, HashSet<(string foundResourceSymbol, Coordinate foundResourceCoordinate)> FoundResources);

