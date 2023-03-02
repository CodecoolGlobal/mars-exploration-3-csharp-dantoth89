using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class LackOfResources:IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext)
    {
        return simulationContext.Rover.DiscoveredCoordinates.Count() >= Math.Pow(simulationContext.Map.Dimension, 2) * 0.8;
    }
}