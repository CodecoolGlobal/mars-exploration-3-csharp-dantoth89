namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

public class ExploringRoutine : IExploringRoutine
{
    private int _reachedTarget = -1;
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private Coordinate _idealStartingPlace;
    
    private Coordinate CalculateMapCentral(SimulationContext simulationContext)
    {
        var sizeOfMap = simulationContext.Map.Dimension;
        var central = (int)sizeOfMap / 2;
        return new Coordinate(central, central);
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
}