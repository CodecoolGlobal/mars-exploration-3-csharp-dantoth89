using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service.CommandCenterCordinator;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MapLoader;
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

    public ExplorationSimulator(IRoverDeployer roverDeployer,
        IConfigurationValidator configurationValidator,
        IExplorationSimulationSteps explorationSimulationSteps,
        IRoverFollower roverFollower,
        IMineAndDeliverSimulator mineOrDeliverSimulator)
    {
        _roverDeployer = roverDeployer;
        _configurationValidator = configurationValidator;
        _explorationSimulationSteps = explorationSimulationSteps;
        _roverFollower = roverFollower;
        _mineOrDeliverSimulator = mineOrDeliverSimulator;
    }

    public void ExploringSimulator(Map map, ConfigurationModel configuration, int numberToRun,
        List<Command_Center> commandCenters)
    {
        int prevnumberOfCMDCs = commandCenters.Count;
        int actualnumberOfCMDCs = 0;
        SimulationContext simulationContext = null;
        bool outcome = false;
        if (!_configurationValidator.Validate(configuration, map))
            throw new Exception("Error! Wrong configurations");
        while (!outcome)
        {
            outcome = RunSimulation(map, configuration, commandCenters, prevnumberOfCMDCs, ref actualnumberOfCMDCs);
            prevnumberOfCMDCs = actualnumberOfCMDCs;
        }

        // return simulationContext;
    }

    private bool RunSimulation(Map map, ConfigurationModel configuration, List<Command_Center> commandCenters, int prevnumberOfCMDCs,
        ref int actualnumberOfCMDCs)
    {
        SimulationContext simulationContext;
        bool outcome;
        var rover = _roverDeployer.DeployMarsRover(_roverFollower.AllMarsRovers.Count, map,
            configuration.LandingSpot);
        _roverFollower.AllMarsRovers.Add(rover);
        simulationContext = new SimulationContext(0, configuration.TimeoutSteps,
            rover, configuration.LandingSpot, map, configuration.NeededResourcesSymbols, null,
            new HashSet<Coordinate>(), new HashSet<Coordinate>(), configuration.CommandCenterSight, commandCenters);
        while (prevnumberOfCMDCs == actualnumberOfCMDCs)
        {
            actualnumberOfCMDCs = commandCenters.Count;
            _explorationSimulationSteps.Steps(simulationContext, configuration);
        }
        var commandCenter = commandCenters[commandCenters.Count() - 1];
        outcome = (simulationContext.Outcome != null);
        MineAndDeliverResource(commandCenter.ClosestMineral, commandCenter, simulationContext);
        // if possible, build commandcenteer (turn active)
        // NEXT if possible build next rover 
        // NEXT NextRover goes for water with MineAndDeliverResource(commandCenter, simulationContext);
        // if possible start simulator again, but starting point is command center position
        return outcome;
    }

    private void MineAndDeliverResource(Coordinate closestResource, Command_Center commandCenter, SimulationContext simulationContext)
    {
        while (_mineOrDeliverSimulator.Finished)
        {
            _mineOrDeliverSimulator.MoveSimulator(closestResource, simulationContext);
        }
        while (!_mineOrDeliverSimulator.Finished)
        {
            _mineOrDeliverSimulator.MoveSimulator(commandCenter.Position, simulationContext);
        }
    }
}