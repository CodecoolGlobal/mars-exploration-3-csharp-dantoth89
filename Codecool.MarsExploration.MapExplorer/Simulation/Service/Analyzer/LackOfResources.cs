using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class LackOfResources:IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext, IRoverFollower rowerFollower)
    {
        return rowerFollower.AllDiscoveredPlaces().Count >= Math.Pow(simulationContext.Map.Dimension, 2) * 0.7;
    }
}