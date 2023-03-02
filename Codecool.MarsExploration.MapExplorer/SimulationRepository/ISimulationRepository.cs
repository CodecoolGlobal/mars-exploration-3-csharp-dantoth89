using Codecool.MarsExploration.MapExplorer.Exploration;

namespace Codecool.MarsExploration.MapExplorer.SimulationRepository;

public interface ISimulationRepository
{
    void Add(int numberOfSteps, int amountOfResources, ExplorationOutcome? outcome);
    void DeleteAll();
}