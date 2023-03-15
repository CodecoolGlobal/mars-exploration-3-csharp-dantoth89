using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;

public class Command_Center : ICommand_Center
{
    public int Id { get; }
    public string Symbol { get; } = "@";
    public int Radius { get; }
    public bool IsItActive { get; set; } = false;
    public Coordinate ClosestMineral { get; }
    public Coordinate ClosestWater { get; }
    public List<MarsRoverModel> Rovers { get; set; }
    private ConfigurationModel _configuration;
    public Dictionary<Resources, int> DeliveredResources { get; set; } = new Dictionary<Resources, int>()
    {
        { Resources.Mineral, 0 }, { Resources.Water, 0 }
    };
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    public Coordinate Position { get; }

    public Command_Center(int id, Coordinate position, MarsRoverModel actualRover, ConfigurationModel config,
        Dictionary<Resources, Coordinate> closestResources)
    {
        Id = id;
        Position = position;
        Rovers = new List<MarsRoverModel>() { actualRover };
        Radius = config.CommandCenterSight;
        _configuration = config;
        ClosestMineral = closestResources[Resources.Mineral];
        ClosestWater = closestResources[Resources.Water];
    }

    public void ActivateWhenBuilt()
    {
        if (!IsItActive && DeliveredResources[Resources.Mineral] == _configuration.CommandCenterCost)
        {
            UseMineralsForConstruction(_configuration.CommandCenterCost);
            IsItActive = true;
        }
    }

    public bool DoWeHaveEnoughMineralForRover()
    {
        return DeliveredResources[Resources.Mineral] >= _configuration.RoverCost;
    }

    public bool DoWeHaveSlotForAnotherRover(Map map)
    {
        return _coordinateCalculator.GetEmptyAdjacentCoordinates(Position, 1, map, 1).Any();
    }

    public void UseMineralsForConstruction(int cost)
    {
        DeliveredResources[Resources.Mineral] -= cost;
    }
}