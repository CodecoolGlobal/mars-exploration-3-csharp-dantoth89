using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public interface IExplorationSimulator
{
    public List<SimulationContext> SimulationContexts { get; set; }

    void ExploringSimulator(Map map, ConfigurationModel configuration, int numberToRun,
        List<Command_Center> commandCenters);
}