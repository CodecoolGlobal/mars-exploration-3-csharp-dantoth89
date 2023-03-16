using System.Diagnostics;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public class MineOrDeliverySimulator : IMineAndDeliverSimulator
{
    private ILogger _logger;
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private MovingRoutine _movingRoutine = new();
    public bool Finished { get; private set; } = true;

    public MineOrDeliverySimulator(ILogger logger)
    {
        _logger = logger;
    }

    Coordinate nextPlace = new Coordinate(-1, -1);

    public void MoveSimulator(Coordinate target, SimulationContext simulationContext)
    {
        if (!_coordinateCalculator
                .GetAdjacentCoordinates(target, simulationContext.Map.Dimension, 1)
                .Contains(nextPlace))
        {
            var possibleCoordinates = _coordinateCalculator.GetEmptyAdjacentCoordinates(
                    simulationContext.Rover.CurrentPosition, simulationContext.Map.Dimension, simulationContext.Map,
                    1)
                .ToList();
            nextPlace = _movingRoutine.GoToTarget(target, simulationContext, possibleCoordinates);
            simulationContext.Rover.CurrentPosition = nextPlace;
            simulationContext.VisitedPlaces.Add(nextPlace);
        }
        else Finished = !Finished;
    }
}