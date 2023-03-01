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

}