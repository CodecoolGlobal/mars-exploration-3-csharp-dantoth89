using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Returning;

public interface IReturningRoutine
{
    Coordinate ReturnMovement(SimulationContext simulationContext, IEnumerable<Coordinate> possiblePlaces);
}