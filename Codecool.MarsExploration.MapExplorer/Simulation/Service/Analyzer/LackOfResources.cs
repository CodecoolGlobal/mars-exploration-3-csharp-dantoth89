using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class LackOfResources:IAnalyzer
{
    private HashSet<Coordinate> _exploredCoordinates;
    public LackOfResources(HashSet<Coordinate> exploredCoordinates)
    {
        _exploredCoordinates = exploredCoordinates;
    }
    
    public bool Analyze(SimulationContext simulationContext)
    {
        return _exploredCoordinates.Count() >= Math.Pow(simulationContext.Map.Dimension, 2) * 0.8;
    }
}