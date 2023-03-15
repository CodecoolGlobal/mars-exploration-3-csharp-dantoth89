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
            outcome = RunSimulation(map, configuration, commandCenters, prevnumberOfCMDCs, ref actualnumberOfCMDCs, prevStepNumbers);
            prevnumberOfCMDCs = actualnumberOfCMDCs;
            prevStepNumbers = simulationContext.StepNumber;
        }
        // return simulationContext;
    }

    private bool RunSimulation(Map map, ConfigurationModel configuration, List<Command_Center> commandCenters,
        int prevnumberOfCMDCs,
        ref int actualnumberOfCMDCs,
        int prevStepNumbers)
    {
        simulationContext = MakeNewRover(map, configuration, commandCenters, prevStepNumbers);
        _logger.Log($" ");
        _logger.Log($"New rover built. ID: {simulationContext.Rover.Id}," +
                    $" position: {simulationContext.Rover.CurrentPosition}," +
                    $"Task: Find new place for next command_center");
        _logger.Log($" ");
        while (prevnumberOfCMDCs == actualnumberOfCMDCs)
        {
            actualnumberOfCMDCs = commandCenters.Count;
            _explorationSimulationSteps.Steps(simulationContext, configuration);
        }

        var commandCenter = commandCenters[commandCenters.Count() - 1];
        var outcome = (simulationContext.Outcome != null);
        do
        {
            _logger.Log($" ");
            _logger.Log($"The {simulationContext.Rover.Id} has new task: deliver mineral to command_center-{commandCenter.Id}");
            _logger.Log($" ");
            MineAndDeliverResource(Resources.Mineral, commandCenter, simulationContext);
            commandCenter.ActivateWhenBuilt();
        } while (commandCenter.IsItActive && commandCenter.DoWeHaveEnoughMineralForRover() &&
                 commandCenter.DoWeHaveSlotForAnotherRover(map));
        _logger.Log($" ");
        _logger.Log($"Building command_center-{commandCenter.Id} is ready at {commandCenter.Position}");
        _logger.Log($" ");
        commandCenter.UseMineralsForConstruction(configuration.RoverCost);
        roverStarting = commandCenter.Position;
        var newSimCont = MakeNewRover(map, configuration, commandCenters, simulationContext.StepNumber);
        _logger.Log($" ");
        _logger.Log($"New rover built. ID: {newSimCont.Rover.Id}," +
                    $" position: {newSimCont.Rover.CurrentPosition}," +
                    $"Task: deliver water to command_center-{commandCenter.Id}");
        _logger.Log($" ");
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
        var newSimCont = new SimulationContext(prevStepNumbers, configuration.TimeoutSteps, newRover, roverStarting, map,
            configuration.NeededResourcesSymbols, null, _roverFollower.AllDiscoveredPlaces(), new HashSet<Coordinate>(),
            configuration.CommandCenterSight,
            commandCenters);
        return newSimCont;
    }

    private void MineAndDeliverResource(Resources resource, Command_Center commandCenter,
        SimulationContext simulationContext)
    {
        var target = resource == Resources.Mineral ? commandCenter.ClosestMineral : commandCenter.ClosestWater;
        while (_mineOrDeliverSimulator.Finished)
        {
            _mineOrDeliverSimulator.MoveSimulator(target, simulationContext);
        }

        while (!_mineOrDeliverSimulator.Finished)
        {
            _mineOrDeliverSimulator.MoveSimulator(commandCenter.Position, simulationContext);
        }

        commandCenter.DeliveredResources[resource]++;
    }
}