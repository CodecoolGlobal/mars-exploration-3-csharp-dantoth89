using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Model;

public record MarsRoverModel(string Id, Coordinate CurrentPosition, int Sight,
    HashSet<(string foundResourceSymbol, Coordinate foundResourceCoordinate)> FoundResources)
{
    public string Id { get; init; } = Id;
    public Coordinate CurrentPosition { get; set; } = CurrentPosition;
    public int Sight { get; init; } = Sight;
    public HashSet<(string foundResourceSymbol, Coordinate foundResourceCoordinate)> FoundResources { get; set; } =
        FoundResources;

}

