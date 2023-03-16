using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class SuccessOfCommandCentersAnalyzer:IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext, IRoverFollower roverFollower)
    {
        return simulationContext.CommandCenters.Count(center => center.IsItActive) >= 2;
    }
}