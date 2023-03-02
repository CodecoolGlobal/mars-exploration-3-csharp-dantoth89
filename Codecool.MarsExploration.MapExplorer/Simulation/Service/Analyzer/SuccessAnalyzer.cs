using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class SuccessAnalyzer:IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext)
    {
        int minerals = 0;
        int water = 0;
        
        foreach (var (foundResourceSymbol, foundResourceCoordinate) in simulationContext.Rover.FoundResources)
        {
            if (foundResourceSymbol == "%") minerals++;
            else if (foundResourceSymbol == "*") water++;
        }

        return minerals >= 4 && water >= 3;
    }
}
