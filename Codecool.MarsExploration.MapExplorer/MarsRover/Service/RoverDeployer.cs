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
        var landingCoordinate = _coordinateCalculator.GetEmptyAdjacentCoordinates(_configuration.LandingSpot, map.Dimension, map, 1)
            .ToList()[0];
        var numberOfRover = numberOfRoversDeployed+1;
        var foundResources = new HashSet<(string, Coordinate)>();
        return new MarsRoverModel($"rover-{numberOfRover}", landingCoordinate, _configuration.RoverSight, foundResources,
            new HashSet<Coordinate>());
    }

}