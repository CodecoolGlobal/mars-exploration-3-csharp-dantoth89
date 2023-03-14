using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.Model;

// public record ConfigurationModel(string MapFilePath, Coordinate LandingSpot, IEnumerable<string> NeededResourcesSymbols,
//     int TimeoutSteps, int RoverCost, int RoverSight, int CommandCenterSight, int LoggerType);

// Logger useage x
// Map useage 
// Rover cost? x
// Rover sight x
// CMD sight x
// requirements for colonization 

public class ConfigurationModel
{
    public string MapFilePath;
    public Coordinate LandingSpot;
    public IEnumerable<string> NeededResourcesSymbols;
    public int TimeoutSteps;
    public int RoverCost;
    public int RoverSight;
    public int CommandCenterSight;
    public bool LoggerType;

    public ConfigurationModel(string mapFilePath, Coordinate landingSpot, IEnumerable<string> neededResourcesSymbols, int timeoutSteps, int roverCost, int roverSight, int commandCenterSight, bool loggerType)
    {
        MapFilePath = mapFilePath;
        LandingSpot = landingSpot;
        NeededResourcesSymbols = neededResourcesSymbols;
        TimeoutSteps = timeoutSteps;
        RoverCost = roverCost;
        RoverSight = roverSight;
        CommandCenterSight = commandCenterSight;
        LoggerType = loggerType;
    }
}

 
