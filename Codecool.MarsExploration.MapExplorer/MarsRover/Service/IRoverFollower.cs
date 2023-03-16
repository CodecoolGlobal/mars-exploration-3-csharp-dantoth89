using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Service;

public interface IRoverFollower
{
    public List<MarsRoverModel> AllMarsRovers { get; set; }

    public int NumberOfMarsRovers();

    public Coordinate PositionOfMarsRover(string roverId);

    public HashSet<Coordinate> AllDiscoveredPlaces();

    public HashSet<(string, Coordinate)> AllFoundResources();
}