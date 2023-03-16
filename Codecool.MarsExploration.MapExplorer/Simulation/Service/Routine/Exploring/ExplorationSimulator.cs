using System.Diagnostics.Metrics;
using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service.CommandCenterCordinator;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public class ExplorationSimulator : IExplorationSimulator
{
    private IRoverDeployer _roverDeployer;
    private IConfigurationValidator _configurationValidator;
    private IExplorationSimulationSteps _explorationSimulationSteps;
    private IRoverFollower _roverFollower;
    private IMineAndDeliverSimulator _mineOrDeliverSimulator;
    private ILogger _logger;
    public List<SimulationContext> SimulationContexts { get; set; } = new List<SimulationContext>();

    public ExplorationSimulator(IRoverDeployer roverDeployer,
        IConfigurationValidator configurationValidator,
        IExplorationSimulationSteps explorationSimulationSteps,
        IRoverFollower roverFollower,
        IMineAndDeliverSimulator mineOrDeliverSimulator,
        ILogger logger)
    {
        _roverDeployer = roverDeployer;
        _configurationValidator = configurationValidator;
        _explorationSimulationSteps = explorationSimulationSteps;
        _roverFollower = roverFollower;
        _mineOrDeliverSimulator = mineOrDeliverSimulator;
        _logger = logger;
    }

    private SimulationContext simulationContext;
    private Coordinate roverStarting;

    public void ExploringSimulator(Map map, ConfigurationModel configuration, int numberToRun,
        List<Command_Center> commandCenters)
    {
        int prevStepNumbers = 0;
        roverStarting = configuration.LandingSpot;
        int prevnumberOfCMDCs = commandCenters.Count;
        int actualnumberOfCMDCs = 0;
        bool outcome = false;
        if (!_configurationValidator.Validate(configuration, map))
            throw new Exception("Error! Wrong configurations");
        while (!outcome)
        {
            outcome = RunSimulation(map, configuration, commandCenters, prevnumberOfCMDCs, ref actualnumberOfCMDCs,
                prevStepNumbers);
            prevnumberOfCMDCs = actualnumberOfCMDCs;
            prevStepNumbers = simulationContext.StepNumber;
        }
    }

    private bool RunSimulation(Map map, ConfigurationModel configuration, List<Command_Center> commandCenters,
        int prevnumberOfCMDCs,
        ref int actualnumberOfCMDCs,
        int prevStepNumbers)
    {
        simulationContext = MakeNewRover(map, configuration, commandCenters, prevStepNumbers);
        _logger.Log($"\nNew rover built. ID: {simulationContext.Rover.Id}," +
                    $" position: {simulationContext.Rover.CurrentPosition},\n");
        int counterForCmdCBuilding = 0;
        Command_Center commandCenter;
        bool outcome;
        do
        {
            _logger.Log($"\nThe {simulationContext.Rover.Id}'s task: Find new place for next command_center\n");
            counterForCmdCBuilding = 0;
            while (prevnumberOfCMDCs == actualnumberOfCMDCs)
            {
                actualnumberOfCMDCs = commandCenters.Count;
                _explorationSimulationSteps.Steps(simulationContext, configuration);
                if (simulationContext.Outcome != null)
                    return true;
            }

            outcome = (simulationContext.Outcome != null);

            commandCenter = commandCenters[commandCenters.Count() - 1];
            prevnumberOfCMDCs = commandCenters.Count;
            _logger.Log(
                $"\nThe {simulationContext.Rover.Id} has new task: deliver mineral to command_center-{commandCenter.Id}\n");
            do
            {
                counterForCmdCBuilding = MineAndDeliverResource(Resources.Mineral, commandCenter, simulationContext);
                commandCenter.ActivateWhenBuilt();
            } while (!(commandCenter.IsItActive &&
                       commandCenter.DoWeHaveEnoughMineralForRover() ||
                       counterForCmdCBuilding > configuration.CommandCenterSight * 2));
        } while (counterForCmdCBuilding > configuration.CommandCenterSight * 2);

        _logger.Log($"\nBuilding command_center-{commandCenter.Id} is ready at {commandCenter.Position}\n");
        commandCenter.UseMineralsForConstruction(configuration.RoverCost);
        roverStarting = commandCenter.Position;
        var newSimCont = MakeNewRover(map, configuration, commandCenters, simulationContext.StepNumber);
        _logger.Log($"\nNew rover built. ID: {newSimCont.Rover.Id}," +
                    $" position: {newSimCont.Rover.CurrentPosition}," +
                    $"Task: deliver water to command_center-{commandCenter.Id}\n");
        MineAndDeliverResource(Resources.Water, commandCenter, newSimCont);
        do
        {
            MineAndDeliverResource(Resources.Mineral, commandCenter, simulationContext);
            commandCenter.ActivateWhenBuilt();
        } while (commandCenter.DoWeHaveEnoughMineralForRover() &&
                 commandCenter.DoWeHaveSlotForAnotherRover(map));

        return outcome;
    }

    private SimulationContext MakeNewRover(Map map, ConfigurationModel configuration,
        List<Command_Center> commandCenters, int prevStepNumbers)
    {
        var newRover =
            _roverDeployer.DeployMarsRover(_roverFollower.AllMarsRovers.Count, map, roverStarting);
        _roverFollower.AllMarsRovers.Add(newRover);
        SimulationContexts.Add(new SimulationContext(prevStepNumbers, configuration.TimeoutSteps - prevStepNumbers,
            newRover,
            roverStarting, map,
            configuration.NeededResourcesSymbols, null, new HashSet<Coordinate>(), new HashSet<Coordinate>(),
            configuration.CommandCenterSight,
            commandCenters));
        return SimulationContexts[^1];
    }

    private int MineAndDeliverResource(Resources resource, Command_Center commandCenter,
        SimulationContext simulationContext)
    {
        int counter = 0;
        var target = resource == Resources.Mineral ? commandCenter.ClosestMineral : commandCenter.ClosestWater;
        while (_mineOrDeliverSimulator.Finished && counter <= commandCenter.Radius * 2)
        {
            counter++;
            _mineOrDeliverSimulator.MoveSimulator(target, simulationContext);
        }

        counter = 0;
        while (!_mineOrDeliverSimulator.Finished && counter <= commandCenter.Radius * 2)
        {
            counter++;
            _mineOrDeliverSimulator.MoveSimulator(commandCenter.Position, simulationContext);
        }

        if (_mineOrDeliverSimulator.Finished)
        {
            commandCenter.DeliveredResources[resource]++;
        }

        return counter;
    }
}