using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
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
    private ILogger _logger;
    private IExploringRoutine _exploringRoutine;
    private ISimulationRepository _simulationRepository;
    private int _numberOfResources = 0;
    private int _prevNumberOfResources = 0;

    public ExplorationSimulatorSteps(ICoordinateCalculator coordinateCalculator, IAnalyzer successAnalyzer,
        IAnalyzer timeoutAnalyzer, IAnalyzer lackOfResourcesAnalyzer, ILogger logger,
        IExploringRoutine exploringRoutine, ISimulationRepository simulationRepository)
    {
        _coordinateCalculator = coordinateCalculator;
        _successAnalyzer = successAnalyzer;
        _timeoutAnalyzer = timeoutAnalyzer;
        _lackOfResourcesAnalyzer = lackOfResourcesAnalyzer;
        _logger = logger;
        _exploringRoutine = exploringRoutine;
        _simulationRepository = simulationRepository;
    }

    public void Steps(SimulationContext simulationContext)
    {

        Movement(simulationContext);
        Scan(simulationContext);
        Analysis(_successAnalyzer, _timeoutAnalyzer, _lackOfResourcesAnalyzer, simulationContext);
        Log(simulationContext);
        IncrementStep(simulationContext);
        
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
                _numberOfResources = simulationContext.Rover.FoundResources.Count;
            }
        }
    }

    private bool NewResourceFound()
    {
        var isTrue = _numberOfResources > _prevNumberOfResources;
        _prevNumberOfResources = _numberOfResources;
        return isTrue;
    }

    public IEnumerable<Coordinate> ScanForPlaceForCommandCenter(SimulationContext simulationContext)
    {
        var rangeOfMineral = new List<Coordinate>();
        var rangeOfWater = new List<Coordinate>();
        foreach (var mineral in 
                 simulationContext.Rover.FoundResources.Where(res=> res.foundResourceSymbol=="%"))
        {
            foreach (var water in 
                     simulationContext.Rover.FoundResources.Where(res=> res.foundResourceSymbol=="*"))
            {
                if (Math.Abs(mineral.foundResourceCoordinate.X - water.foundResourceCoordinate.X) <=
                    simulationContext.CommandCenterSight * 2 &&
                    Math.Abs(mineral.foundResourceCoordinate.Y - water.foundResourceCoordinate.Y) <=
                    simulationContext.CommandCenterSight * 2)
                {
                    for (int i = 0; i < simulationContext.CommandCenterSight; i++)
                    {
                        rangeOfMineral.AddRange(_coordinateCalculator.GetAdjacentCoordinates(
                            mineral.foundResourceCoordinate, simulationContext.Map.Dimension, i));
                    }
                    for (int i = 0; i < simulationContext.CommandCenterSight; i++)
                    {
                        rangeOfWater.AddRange(_coordinateCalculator.GetAdjacentCoordinates(
                            water.foundResourceCoordinate, simulationContext.Map.Dimension, i));
                    }
                }
            }
        }
        return rangeOfMineral.Intersect(rangeOfWater);
    }

    public bool IsThereAnotherCommandCenter(IEnumerable<Coordinate> possiblePlacesForCMDC, List<Command_Center> commandCenters)
    {
        return possiblePlacesForCMDC.Intersect
            (commandCenters.Select
                (CMDC => CMDC.Position).ToList()).Count() == 0;
    }

private void Analysis(IAnalyzer successAnalyzer, IAnalyzer timeoutAnalyzer, IAnalyzer lackOfResourcesAnalyzer,
        SimulationContext simulationContext)
    {
        if (successAnalyzer.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Colonizable;
        else if (timeoutAnalyzer.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Timeout;
        else if (lackOfResourcesAnalyzer.Analyze(simulationContext))
            simulationContext.Outcome = ExplorationOutcome.Error;
    }

    private void Log(SimulationContext simulationContext)
    {
        // if(simulationContext.Outcome != null)
        //     _simulationRepository.Add(simulationContext.StepNumber,simulationContext.Rover.FoundResources.Count,simulationContext.Outcome);
        //
        string message =
            simulationContext.Outcome != null
                ? $"STEP {simulationContext.StepNumber}; EVENT outcome; OUTCOME {simulationContext.Outcome}"
                : $"STEP {simulationContext.StepNumber}; EVENT position; POSITION {simulationContext.Rover.CurrentPosition}; Steps to Timeout {simulationContext.StepsToTimeOut}";
        _logger.Log(message);
    }

    private void IncrementStep(SimulationContext simulationContext)
    {
        simulationContext.StepNumber++;
        simulationContext.StepsToTimeOut--;
    }
}