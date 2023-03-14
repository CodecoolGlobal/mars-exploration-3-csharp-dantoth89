using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.ParseJSON;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Returning;
using Codecool.MarsExploration.MapExplorer.SimulationRepository;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer;

class Program
{
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
    private static string _mapFile = $@"{WorkDir}/Resources/exploration-1.map";

    private static string _databaseFile =
        $@"{WorkDir.Replace("/bin/Debug/net6.0/", "")}/Resources/SimulationDataBase.db";
    
    private static JSONHandler _jsonHandler = new JSONHandler();
    private static ConfigurationModel _configuration = _jsonHandler.JSONConverter(WorkDir);
    
    private static SimulationContext _simulationContext;
    private static IMapLoader _mapLoader = new MapLoader.MapLoader();
    private static ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private static IRoverDeployer _roverDeployer = new RoverDeployer(_configuration, _coordinateCalculator);
    private static IConfigurationValidator _configurationValidator = new ConfigurationValidator(_coordinateCalculator);
    private static IAnalyzer _successAnalyzer = new SuccessAnalyzer();
    private static IAnalyzer _timeoutAnalyzer = new TimeoutAnalyzer();
    private static IAnalyzer _lackOfResourcesAnalyzer = new LackOfResources();
    private static ILogger _logger = _configuration.LoggerType ? new FileLogger() : new ConsolLogger();

    private static ISimulationRepository _simulationRepository =
        new SimulationRepository.SimulationRepository(_databaseFile);

    private static IExploringRoutine _exploringRoutine = new ExploringRoutine();

    private static IExplorationSimulationSteps _explorationSimulationSteps =
        new ExplorationSimulatorSteps(_coordinateCalculator, _successAnalyzer, _timeoutAnalyzer,
            _lackOfResourcesAnalyzer, _logger, _exploringRoutine, _simulationRepository);

    private static IExplorationSimulator _explorationSimulator =
        new ExplorationSimulator(_roverDeployer, _configurationValidator, _explorationSimulationSteps);

    private static IReturnSimulator _returnSimulator = new ReturnSimulator(_logger);

    public static void Main(string[] args)
    {
     
        File.Delete($@"{WorkDir}/Resources/message.txt");
        var map = _mapLoader.Load(_configuration.MapFilePath);
        Console.WriteLine(map);
         try
         {
        var simCont =
                _explorationSimulator.ExploringSimulator(_mapLoader.Load(_configuration.MapFilePath), _configuration, 1,
                    _simulationContext);
            if (simCont != null)
            {
                _returnSimulator.ReturningSimulator(simCont);
                foreach (var visited in simCont.VisitedPlaces)
                {
                    map.Representation[visited.X, visited.Y] = "0";
                }

                foreach (var visited in simCont.VisitedForReturn)
                {
                    map.Representation[visited.X, visited.Y] = "O";
                }
            }

            Console.WriteLine(map);
        }
         catch (Exception e)
         {
             Console.ForegroundColor = ConsoleColor.Red;
             Console.WriteLine(e.Message);
             Console.ForegroundColor = ConsoleColor.White;
         }
    }
}