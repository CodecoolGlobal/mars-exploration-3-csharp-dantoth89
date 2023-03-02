using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public interface IExploringRoutine
{

    Coordinate ExploreMovement(SimulationContext simulationContext, IEnumerable<Coordinate> possiblePlaces);

}