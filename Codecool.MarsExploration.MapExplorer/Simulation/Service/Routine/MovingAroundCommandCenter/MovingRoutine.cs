using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public class MovingRoutine:BaseRoutine
{
    public Coordinate GoToTarget(Coordinate target, SimulationContext simulationContext, List<Coordinate> possiblePlaces)
    {
       return ReachTargetPlace(simulationContext, target, possiblePlaces)!;
    }
    protected override Coordinate GetNextStepVisitedOrNot(SimulationContext simulationContext,
        IEnumerable<Coordinate> possiblePlaces, Coordinate target)
    {
        foreach (var place in possiblePlaces)
        {
            if (!simulationContext.VisitedPlaces.Contains(place))
            {
                return CalculateBestPossiblePlace(target, possiblePlaces.Except(simulationContext.VisitedPlaces));
            }
        }
        return CalculateBestPossiblePlace(target, possiblePlaces);
    }
}