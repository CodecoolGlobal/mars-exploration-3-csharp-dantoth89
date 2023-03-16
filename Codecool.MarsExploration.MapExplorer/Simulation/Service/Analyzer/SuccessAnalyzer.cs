using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class SuccessAnalyzer : IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext, IRoverFollower roverFollower)
    {
        int minerals = 0;
        int water = 0;

        foreach (var (foundResourceSymbol, foundResourceCoordinate) in roverFollower.AllFoundResources())
        {
            if (foundResourceSymbol == "%") minerals++;
            else if (foundResourceSymbol == "*") water++;
        }

        return minerals >= 20 && water >= 20;
    }
}