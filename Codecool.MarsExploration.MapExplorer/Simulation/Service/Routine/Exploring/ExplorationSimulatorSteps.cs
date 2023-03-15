using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service.CommandCenterCordinator;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;
using Codecool.MarsExploration.MapExplorer.SimulationRepository;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public class ExplorationSimulatorSteps : IExplorationSimulationSteps
{
    private ICoordinateCalculator _coordinateCalculator;
    private IAnalyzer _successAnalyzer;
    private IAnalyzer _timeoutAnalyzer;
    private IAnalyzer _lackOfResourcesAnalyzer;
    private IAnalyzer _successOfCommandCenters;
    private ILogger _logger;
    private IExploringRoutine _exploringRoutine;
    private ISimulationRepository _simulationRepository;
    internal int NumberOfResources = 0;
    internal int PrevNumberOfResources = 0;
    private readonly PlacingCommandCenter _placingCommandCenter;

    public ExplorationSimulatorSteps(ICoordinateCalculator coordinateCalculator, IAnalyzer successAnalyzer,
        IAnalyzer timeoutAnalyzer, IAnalyzer lackOfResourcesAnalyzer, ILogger logger,
        IExploringRoutine exploringRoutine, ISimulationRepository simulationRepository, IAnalyzer successOfCommandCenters)
    {
        _coordinateCalculator = coordinateCalculator;
        _successAnalyzer = successAnalyzer;
        _timeoutAnalyzer = timeoutAnalyzer;
        _successOfCommandCenters = successOfCommandCenters;
        _lackOfResourcesAnalyzer = lackOfResourcesAnalyzer;
        _logger = logger;
        _exploringRoutine = exploringRoutine;
        _simulationRepository = simulationRepository;
        _placingCommandCenter = new PlacingCommandCenter(this);
    }

    public void Steps(SimulationContext simulationContext, ConfigurationModel config)
    {
        Movement(simulationContext);
        Scan(simulationContext);
        Analysis(_successOfCommandCenters, _successAnalyzer, _timeoutAnalyzer, _lackOfResourcesAnalyzer, simulationContext);
        Log(simulationContext);
        IncrementStep(simulationContext);
        var newCMDCenter = _placingCommandCenter.PlaceCommandCenter(simulationContext,
            simulationContext.CommandCenters,
            config);
        if (newCMDCenter != null)
        { 
            simulationContext.CommandCenters.Add(newCMDCenter!);
            simulationContext.Map.Representation[newCMDCenter.Position.X, newCMDCenter.Position.Y] =
                newCMDCenter.Symbol;
        }
    }
    private void Movement(SimulationContext simulationContext)
    {
        var possibleCoordinates = _coordinateCalculator.GetEmptyAdjacentCoordinates(
            simulationContext.Rover.CurrentPosition, simulationContext.Map.Dimension, simulationContext.Map, 1);
        var nextPlace = _exploringRoutine.ExploreMovement(simulationContext, possibleCoordinates);
        simulationContext.Rover.CurrentPosition = nextPlace;
        simulationContext.VisitedPlaces.Add(nextPlace);
    }

    private void Scan(SimulationContext simulationContext)
    {
        var discoveredCoordinates = new List<Coordinate>();
        for (int i = 0; i < simulationContext.Rover.Sight; i++)
        {
            discoveredCoordinates.AddRange(_coordinateCalculator.GetAdjacentCoordinates(
                simulationContext.Rover.CurrentPosition, simulationContext.Map.Dimension, i));
        }
        simulationContext.Rover.DiscoveredCoordinates.UnionWith(discoveredCoordinates);
        simulationContext.Rover.DiscoveredCoordinates.Add(simulationContext.Rover.CurrentPosition);
        foreach (var coordinate in discoveredCoordinates)
        {
            if (simulationContext.SymbolsOfPreferredResources.Contains(
                    simulationContext.Map.Representation[coordinate.X, coordinate.Y]))
            {
                simulationContext.Rover.FoundResources.Add((
                    simulationContext.Map.Representation[coordinate.X, coordinate.Y]!, coordinate));
                NumberOfResources = simulationContext.Rover.FoundResources.Count;
            }
        }
    }

    private void Analysis(IAnalyzer commandCenters, IAnalyzer successAnalyzer, IAnalyzer timeoutAnalyzer, IAnalyzer lackOfResourcesAnalyzer,
        SimulationContext simulationContext)
    {
        if (commandCenters.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Colonized;
        else if (successAnalyzer.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Colonizable;
        else if (timeoutAnalyzer.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Timeout;
        else if (lackOfResourcesAnalyzer.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Error;
    }

    private void Log(SimulationContext simulationContext)
    {
        if (simulationContext.Outcome != null)
            _simulationRepository.Add(simulationContext.StepNumber, simulationContext.Rover.FoundResources.Count,
                simulationContext.Outcome);

        string message =
            simulationContext.Outcome != null
                ? $"STEP {simulationContext.StepNumber}; EVENT outcome; OUTCOME {simulationContext.Outcome}"
                : $"STEP {simulationContext.StepNumber}; ROVER: {simulationContext.Rover.Id}; EVENT position; POSITION {simulationContext.Rover.CurrentPosition}; Steps to Timeout {simulationContext.StepsToTimeOut}";
        _logger.Log(message);
    }

    private void IncrementStep(SimulationContext simulationContext)
    {
        simulationContext.StepNumber++;
        simulationContext.StepsToTimeOut--;
    }
}