using Codecool.MarsExploration.MapExplorer.Exploration;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

public class ExploringRoutine : BaseRoutine, IExploringRoutine
{
    private Coordinate CalculateMapCentral(SimulationContext simulationContext)
    {
        var sizeOfMap = simulationContext.Map.Dimension;
        var central = (int)sizeOfMap / 2;
        return new Coordinate(central, central);
    }

    public Coordinate ExploreMovement(SimulationContext simulationContext, IEnumerable<Coordinate> possiblePlaces)
    {
        Coordinate nextStep = new Coordinate(-1, -1);
        if (_idealStartingPlace == null)
        {
            _idealStartingPlace = CalculateIdealStartingPlace(simulationContext);
        }

        var targetPlaces = GenerateCheckpoints(simulationContext, _idealStartingPlace);
        if (_reachedTarget < 0)
        {
            nextStep = ReachTargetPlace(simulationContext, _idealStartingPlace, possiblePlaces);
        }
        else
        {
            if (_reachedTarget == targetPlaces.Count )
                simulationContext.Outcome = ExplorationOutcome.NoneOfThem;
            else
            nextStep = ReachTargetPlace(simulationContext, targetPlaces[_reachedTarget], possiblePlaces);
        }
        return nextStep!;
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

    private Coordinate CalculateIdealStartingPlace(SimulationContext simulationContext)
    {
        var currentPosition = simulationContext.Rover.CurrentPosition;
        if (currentPosition.X < simulationContext.Map.Dimension / 4)
        {
            if ((currentPosition.Y < simulationContext.Map.Dimension / 4))
                return new Coordinate(1 + simulationContext.Rover.Sight, 1 + simulationContext.Rover.Sight);
            else
                return new Coordinate(1 + simulationContext.Rover.Sight,
                    simulationContext.Map.Dimension - simulationContext.Rover.Sight);
        }
        else if (currentPosition.X > simulationContext.Map.Dimension / 4 * 3)
        {
            if ((currentPosition.Y < simulationContext.Map.Dimension / 4))
                return new Coordinate(simulationContext.Map.Dimension - simulationContext.Rover.Sight,
                    1 + simulationContext.Rover.Sight);
            else
                return new Coordinate(simulationContext.Map.Dimension - simulationContext.Rover.Sight,
                    simulationContext.Map.Dimension - simulationContext.Rover.Sight);
        }
        else
            return CalculateMapCentral(simulationContext);
    }

    private List<Coordinate> GenerateCheckpoints(SimulationContext simulationContext, Coordinate startingPlace)
    {
        int x = startingPlace.X;
        int y = startingPlace.Y;
        List<Coordinate> checkPoints = new List<Coordinate>();
        if (startingPlace == CalculateMapCentral(simulationContext))
        {
            var count = 0;
            GetTargetsFromCentral(simulationContext, x, y, count, checkPoints);
            return checkPoints;
        }
        else
        {
            int count = 0;
            Tuple<int, bool> initials = new Tuple<int, bool>(0, false);
            initials = GetInitialisationOfFindingTargetsFromCorners(simulationContext, startingPlace, initials.Item1,
                initials.Item2);
            var prefix = initials.Item1;
            var secondDiagonal = initials.Item2;
            y += prefix * (simulationContext.Map.Dimension - 2 * simulationContext.Rover.Sight);
            checkPoints.Add(new Coordinate(x, y));
            GetTargetsFromCorners(simulationContext, x, y, count, prefix, checkPoints, secondDiagonal);
            return checkPoints;
        }
    }

    private static void GetTargetsFromCentral(SimulationContext simulationContext, int x, int y, int count,
        List<Coordinate> checkPoints)
    {
        while (x < simulationContext.Map.Dimension && y < simulationContext.Map.Dimension
                                                   && x > 0 && y > 0)
        {
            count = count < 0 ? -1 * (count - 1) : -1 * (count + 1);
            y += 2 * simulationContext.Rover.Sight * count;

            checkPoints.Add(new Coordinate(x, y));
            count = -1 * count;
            x += 2 * simulationContext.Rover.Sight * count;

            checkPoints.Add(new Coordinate(x, y));
            count = -1 * count;
        }
    }

    private static Tuple<int, bool> GetInitialisationOfFindingTargetsFromCorners(SimulationContext simulationContext,
        Coordinate startingPlace, int prefix, bool secondDiagonal)
    {
        if (startingPlace == new Coordinate(1 + simulationContext.Rover.Sight, 1 + simulationContext.Rover.Sight))
        {
            prefix = +1;
            secondDiagonal = false;
        }

        if (startingPlace == new Coordinate(1 + simulationContext.Rover.Sight,
                simulationContext.Map.Dimension - simulationContext.Rover.Sight))
        {
            prefix = -1;
            secondDiagonal = true;
        }

        if (startingPlace == new Coordinate(simulationContext.Map.Dimension - simulationContext.Rover.Sight,
                1 + simulationContext.Rover.Sight))
        {
            prefix = +1;
            secondDiagonal = true;
        }

        if (startingPlace == new Coordinate(simulationContext.Map.Dimension - simulationContext.Rover.Sight,
                simulationContext.Map.Dimension - simulationContext.Rover.Sight))
        {
            prefix = -1;
            secondDiagonal = false;
        }

        return new Tuple<int, bool>(prefix, secondDiagonal);
    }

    private void GetTargetsFromCorners(SimulationContext simulationContext, int x, int y, int count, int prefix,
        List<Coordinate> checkPoints, bool secondDiagonal)
    {
        for (int i = 0; i < CalculateTurnings(simulationContext) / 2 - 1; i++)
        {
            count++;
            if (secondDiagonal)
                prefix = prefix * -1;
            x += prefix * (simulationContext.Map.Dimension - count * 2 * simulationContext.Rover.Sight);

            checkPoints.Add(new Coordinate(x, y));
            if (!secondDiagonal)
                prefix = prefix * -1;
            y += prefix * (simulationContext.Map.Dimension - count * 2 * simulationContext.Rover.Sight);

            checkPoints.Add(new Coordinate(x, y));
        }
    }

    private int CalculateTurnings(SimulationContext simulationContext)
    {
        return simulationContext.Map.Dimension / simulationContext.Rover.Sight;
    }
}