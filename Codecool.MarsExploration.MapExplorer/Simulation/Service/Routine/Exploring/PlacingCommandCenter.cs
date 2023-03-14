using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public class PlacingCommandCenter
{
    private ExplorationSimulatorSteps _explorationSimulatorSteps;
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    public PlacingCommandCenter(ExplorationSimulatorSteps explorationSimulatorSteps)
    {
        _explorationSimulatorSteps = explorationSimulatorSteps;
    }

    private bool NewResourceFound()
    {
        var isTrue = _explorationSimulatorSteps.NumberOfResources > _explorationSimulatorSteps.PrevNumberOfResources;
        _explorationSimulatorSteps.PrevNumberOfResources = _explorationSimulatorSteps.NumberOfResources;
        return isTrue;
    }

    public IEnumerable<Coordinate> ScanForPlaceForCommandCenter(SimulationContext simulationContext)
    {
        foreach (var mineral in 
                 simulationContext.Rover.FoundResources.Where(res=> res.foundResourceSymbol=="%"))
        {
            foreach (var water in 
                     simulationContext.Rover.FoundResources.Where(res=> res.foundResourceSymbol=="*"))
            {
                if (AreBothCoordinatesInSightRange(simulationContext, mineral, water) )
                {
                    return GetAreaOfResource(simulationContext, mineral).Intersect(GetAreaOfResource(simulationContext, water));
                }
            }
        }
        return new List<Coordinate>();
    }

    private static bool AreBothCoordinatesInSightRange(SimulationContext simulationContext, (string foundResourceSymbol, Coordinate foundResourceCoordinate) mineral, (string foundResourceSymbol, Coordinate foundResourceCoordinate) water)
    {
        return Math.Abs(mineral.foundResourceCoordinate.X - water.foundResourceCoordinate.X) <=
               simulationContext.CommandCenterSight * 2
               &&
               Math.Abs(mineral.foundResourceCoordinate.Y - water.foundResourceCoordinate.Y) <=
               simulationContext.CommandCenterSight * 2;
    }

    private List<Coordinate> GetAreaOfResource(SimulationContext simulationContext,
        (string foundResourceSymbol, Coordinate foundResourceCoordinate) resource)
    {
        var rangeOfResource = new List<Coordinate>();
        for (int i = 0; i < simulationContext.CommandCenterSight; i++)
        {
            rangeOfResource.AddRange(_coordinateCalculator.GetAdjacentCoordinates(
                resource.foundResourceCoordinate, simulationContext.Map.Dimension, i));
        }

        return rangeOfResource;
    }

    public bool IsThereAnotherCommandCenter(IEnumerable<Coordinate> possiblePlacesForCMDC, List<Command_Center> commandCenters)
    {
        return !possiblePlacesForCMDC.Intersect
        (commandCenters.Select
            (CMDC => CMDC.Position).ToList()).Any();
    }
    

}