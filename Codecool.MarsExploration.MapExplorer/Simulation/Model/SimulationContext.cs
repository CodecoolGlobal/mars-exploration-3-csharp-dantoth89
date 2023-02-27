using Codecool.MarsExploration.MapExplorer.Exploration;

using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Model;

public record SimulationContext(int StepNumber, int StepsToTimeOut, MarsRover.Model.MarsRover Rover, Coordinate LocationOfSpaceship, Map Map, IEnumerable<string> SymbolsOfPreferredResources, ExplorationOutcome Outcome);
