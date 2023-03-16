using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public interface IAnalyzer
{
    bool Analyze(SimulationContext simulationContext, IRoverFollower roverFollower);
}