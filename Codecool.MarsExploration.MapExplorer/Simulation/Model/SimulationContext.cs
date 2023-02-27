using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Model;

public record SimulationContext(int StepNumber, int StepsToTimeOut, MarsRoverModel Rover,
    Coordinate LocationOfSpaceship, Map Map, IEnumerable<string> SymbolsOfPreferredResources,
    ExplorationOutcome? Outcome)
{
    public int StepNumber { get; set; } = StepNumber;
    public ExplorationOutcome? Outcome { get; set; } = Outcome;
}
