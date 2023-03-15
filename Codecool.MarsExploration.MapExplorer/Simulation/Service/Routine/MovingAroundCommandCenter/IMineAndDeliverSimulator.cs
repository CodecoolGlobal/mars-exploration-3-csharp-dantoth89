using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public interface IMineAndDeliverSimulator
{
    public bool Finished { get; }
    void MoveSimulator(Coordinate target, SimulationContext simulationContext);
}