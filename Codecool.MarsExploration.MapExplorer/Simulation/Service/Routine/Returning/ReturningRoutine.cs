using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Returning;

public class ReturningRoutine : BaseRoutine, IReturningRoutine
{
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    Coordinate nextStep = new Coordinate(-1, -1);
    
    protected override Coordinate GetNextStepVisitedOrNot(SimulationContext simulationContext,
        IEnumerable<Coordinate> possiblePlaces, Coordinate target)
    {
        foreach (var place in possiblePlaces)
        {
            if (!simulationContext.VisitedForReturn.Contains(place))
            {
                return CalculateBestPossiblePlace(target, possiblePlaces.Except(simulationContext.VisitedForReturn));
            }
        }
        
        return CalculateBestPossiblePlace(target, possiblePlaces);
    }
        
    public Coordinate ReturnMovement(SimulationContext simulationContext, IEnumerable<Coordinate> possiblePlaces)
    {
        return ReachTargetPlace(simulationContext, simulationContext.LocationOfSpaceship, possiblePlaces)!;
    }
}