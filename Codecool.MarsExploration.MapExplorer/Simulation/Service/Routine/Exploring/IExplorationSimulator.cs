using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public interface IExplorationSimulator
{
    SimulationContext ExploringSimulator(Map map, ConfigurationModel configuration, int numberToRun,
        SimulationContext simulationContext);
}