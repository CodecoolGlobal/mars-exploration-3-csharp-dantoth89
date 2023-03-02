using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.Service;

public class ConfigurationValidator:IConfigurationValidator
{
    private readonly ICoordinateCalculator _coordinateCalculator;
    
    public ConfigurationValidator(ICoordinateCalculator coordinateCalculator)
    {
        
        _coordinateCalculator = coordinateCalculator;
    }
    
    public bool Validate(ConfigurationModel configuration, Map map)
    {
        return FilePathIsNotEmpty(configuration.MapFilePath) &&
               LandingSpotIsNotOccupied(configuration.LandingSpot, map) &&
               ResourcesAreNotEmpty(configuration.NeededResourcesSymbols) &&
               TimeoutStepsGreaterThanZero(configuration.TimeoutSteps) &&
               LandingSpotHasFreeAdjacentCoordinate(configuration.LandingSpot, map);
    }
    
    private bool FilePathIsNotEmpty(string filePath)
    {
        return filePath != "";
    }
    
    private bool LandingSpotIsNotOccupied(Coordinate landingSpot, Map map)
    {
        return map.IsEmpty(landingSpot);
    }
    
    private bool LandingSpotHasFreeAdjacentCoordinate(Coordinate landingSpot, Map map)
    {
        return _coordinateCalculator.GetAdjacentCoordinates(landingSpot, map.Dimension).Any(coordinate => map.GetByCoordinate(coordinate) is null);
    }
    
    private bool ResourcesAreNotEmpty(IEnumerable<string> resources)
    {
        return resources.Any();
    }
    
    private bool TimeoutStepsGreaterThanZero(int timeoutSteps)
    {
        return timeoutSteps > 0;
    }
}
