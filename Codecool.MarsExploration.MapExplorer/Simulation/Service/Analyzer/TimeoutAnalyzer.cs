using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class TimeoutAnalyzer : IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext, IRoverFollower roverFollower)
    {
        return simulationContext.StepsToTimeOut == 0;
    }
}