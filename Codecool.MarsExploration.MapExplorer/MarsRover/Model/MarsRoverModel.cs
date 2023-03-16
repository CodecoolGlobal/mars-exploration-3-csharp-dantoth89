using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Model;

public record MarsRoverModel(string Id, Coordinate CurrentPosition, int Sight,
    HashSet<(string foundResourceSymbol, Coordinate foundResourceCoordinate)> FoundResources,
    HashSet<Coordinate> DiscoveredCoordinates)
{
    public Coordinate CurrentPosition { get; set; } = CurrentPosition;

    public HashSet<(string foundResourceSymbol, Coordinate foundResourceCoordinate)> FoundResources { get; set; } =
        FoundResources;

    public HashSet<Coordinate> DiscoveredCoordinates { get; set; } = DiscoveredCoordinates;
}