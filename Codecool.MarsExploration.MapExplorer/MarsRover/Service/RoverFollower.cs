using System.Data;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Service;

public class RoverFollower:IRoverFollower
{
    public List<MarsRoverModel> AllMarsRovers { get; set; } = new List<MarsRoverModel>();
    public int NumberOfMarsRovers()
    {
        return AllMarsRovers.Count;
    }

    public Coordinate PositionOfMarsRover(string roverId)
    {
        try
        {
            return AllMarsRovers.Find(rover => rover.Id == roverId).CurrentPosition;
        }
        catch(Exception)
        {
            return new Coordinate(-1, -1);
        }
    }

    public HashSet<Coordinate> AllDiscoveredPlaces()
    {
        HashSet<Coordinate> allDiscovered = new();
        foreach (var rover in AllMarsRovers)
        {
            allDiscovered.UnionWith(rover.DiscoveredCoordinates);
        }

        return allDiscovered;
    }
}