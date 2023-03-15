using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class SuccessOfCommandCentersAnalyzer:IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext)
    {
        return simulationContext.CommandCenters.Count >= 2;
    }
}