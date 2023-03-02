using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
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
  

    public ExplorationSimulator(IRoverDeployer roverDeployer,
        IConfigurationValidator configurationValidator,
        IExplorationSimulationSteps explorationSimulationSteps)
    {
        _roverDeployer = roverDeployer;
        _configurationValidator = configurationValidator;
        _explorationSimulationSteps = explorationSimulationSteps;
        
    }
    public SimulationContext ExploringSimulator(Map map, ConfigurationModel configuration, int numberToRun, SimulationContext simulationContext)
    {
        if (_configurationValidator.Validate(configuration,map))
        {
            int count = 0;
            while (count < numberToRun)
            {
                var rover = _roverDeployer.DeployMarsRover(count, map);
                simulationContext = new SimulationContext(0, configuration.TimeoutSteps,
                    rover, configuration.LandingSpot, map, configuration.NeededResourcesSymbols, null, new List<Coordinate>());
                count++;
                while (simulationContext.Outcome==null)
                {
                    _explorationSimulationSteps.Steps(simulationContext);
                }
            }
        }

        return simulationContext;
    }
}