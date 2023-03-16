using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine;

public abstract class BaseRoutine
{
    protected int _reachedTarget = -1;
    protected ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    protected Coordinate _idealStartingPlace;


    protected Coordinate? ReachTargetPlace(SimulationContext simulationContext, Coordinate idealTargetPlace,
        IEnumerable<Coordinate> possiblePlaces)
    {
        var x = simulationContext.Rover.CurrentPosition.X;
        var y = simulationContext.Rover.CurrentPosition.Y;

        if (x > idealTargetPlace.X)
            x--;
        if (x < idealTargetPlace.X)
            x++;
        if (y > idealTargetPlace.Y)
            y--;
        if (y < idealTargetPlace.Y)
            y++;

        if (x == idealTargetPlace.X && y == idealTargetPlace.Y)
        {
            _reachedTarget++;
        }

        return Move(simulationContext, possiblePlaces, x, y, idealTargetPlace);
    }

    protected Coordinate Move(SimulationContext simulationContext, IEnumerable<Coordinate> possiblePlaces, int x, int y,
        Coordinate target)
    {
        if (possiblePlaces.Contains(new Coordinate(x, y))
            &&
            !simulationContext.VisitedPlaces.Contains(new Coordinate(x, y)))
            return new Coordinate(x, y);
        else
        {
            var commonPossiblePlaces =
                possiblePlaces.Intersect(GetNotOccupiedNeighboursOfNextStep(simulationContext));
            if (commonPossiblePlaces.Any())
                return GetNextStepVisitedOrNot(simulationContext, commonPossiblePlaces, target);
            else
                return GetNextStepVisitedOrNot(simulationContext, possiblePlaces, target);
        }
    }

    protected IEnumerable<Coordinate> GetNotOccupiedNeighboursOfNextStep(SimulationContext simulationContext)
    {
        return _coordinateCalculator.GetEmptyAdjacentCoordinates(simulationContext.Rover.CurrentPosition,
            simulationContext.Map.Dimension, simulationContext.Map);
    }

    protected abstract Coordinate GetNextStepVisitedOrNot(SimulationContext simulationContext,
        IEnumerable<Coordinate> possiblePlaces, Coordinate target);

    protected Coordinate CalculateBestPossiblePlace(Coordinate TargetPlace, IEnumerable<Coordinate> possibleplaces)
    {
        var distances = new Dictionary<Coordinate, int>();
        foreach (var coord in possibleplaces)
        {
            distances.Add(coord, (Math.Abs(TargetPlace.X - coord.X) + Math.Abs(TargetPlace.Y - coord.Y)));
        }

        return distances.MinBy(pair => pair.Value).Key;
    }
}