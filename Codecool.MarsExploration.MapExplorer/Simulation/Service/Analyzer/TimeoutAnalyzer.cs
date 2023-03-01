using Codecool.MarsExploration.MapExplorer.Simulation.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;

public class TimeoutAnalyzer:IAnalyzer
{
    public bool Analyze(SimulationContext simulationContext)
    {
        return simulationContext.StepNumber >= simulationContext.StepsToTimeOut;
    }
}