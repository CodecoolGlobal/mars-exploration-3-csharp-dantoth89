using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Service.Placer;

namespace Codecool.MarsExploration.MapExplorer.MarsRover.Service;

public class RoverDeployer:IRoverDeployer
{
    private ConfigurationModel _configuration;
    private ICoordinateCalculator _coordinateCalculator;

    public RoverDeployer(ConfigurationModel configuration, ICoordinateCalculator coordinateCalculator)
    {
        _configuration = configuration;
        _coordinateCalculator = coordinateCalculator;
    }

    public MarsRoverModel DeployMarsRover(int numberOfRoversDeployed, Map map)
    {
        var possibleLandingCoordinates = _coordinateCalculator.GetAdjacentCoordinates(_configuration.LandingSpot, map.Dimension, 1);
        var landingCoordinate = possibleLandingCoordinates.First(coord => map.Representation[coord.X, coord.Y] == null);
        var numberOfRover = numberOfRoversDeployed+1;
        var foundResources = new HashSet<(string, Coordinate)>();
        return new MarsRoverModel($"rover-{numberOfRover}", landingCoordinate, 2, foundResources,
            new HashSet<Coordinate>());
    }

}