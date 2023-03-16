using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service.CommandCenterCordinator;
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
    private static char separator = Path.DirectorySeparatorChar;

    private static string _databaseFile =
        $@"{WorkDir.Replace($@"{separator}bin{separator}Debug{separator}net6.0{separator}", "")}{separator}DataBase{separator}SimulationDataBase.db";

    private static JSONHandler _jsonHandler = new JSONHandler();
    private static ConfigurationModel _configuration = _jsonHandler.JSONConverter(WorkDir);
    private static IMapLoader _mapLoader = new MapLoader.MapLoader();
    private static ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private static IRoverDeployer _roverDeployer = new RoverDeployer(_configuration, _coordinateCalculator);
    private static IConfigurationValidator _configurationValidator = new ConfigurationValidator(_coordinateCalculator);
    private static IAnalyzer _successAnalyzer = new SuccessAnalyzer();
    private static IAnalyzer _timeoutAnalyzer = new TimeoutAnalyzer();
    private static IAnalyzer _lackOfResourcesAnalyzer = new LackOfResources();
    private static IAnalyzer _commandCenterAnalyzer = new SuccessOfCommandCentersAnalyzer();
    private static ILogger _logger = _configuration.LoggerType ? new FileLogger() : new ConsolLogger();

    private static ISimulationRepository _simulationRepository =
        new SimulationRepository.SimulationRepository(_databaseFile);

    private static IExploringRoutine _exploringRoutine = new ExploringRoutine();
    private static IRoverFollower _roverFollower = new RoverFollower();
    private static List<Command_Center> _commandCenters = new CenterOfCommandCenters().AllCommandCenters;

    private static IExplorationSimulationSteps _explorationSimulationSteps =
        new ExplorationSimulatorSteps(_coordinateCalculator, _successAnalyzer, _timeoutAnalyzer,
            _lackOfResourcesAnalyzer, _logger, _exploringRoutine, _simulationRepository, _commandCenterAnalyzer);

    private static IMineAndDeliverSimulator _mineOrDeliverSimulator = new MineOrDeliverySimulator(_logger);

    private static IExplorationSimulator _explorationSimulator =
        new ExplorationSimulator(_roverDeployer, _configurationValidator, _explorationSimulationSteps, _roverFollower,
            _mineOrDeliverSimulator, _logger);

    private static IReturnSimulator _returnSimulator = new ReturnSimulator(_logger);

    public static void Main(string[] args)
    {
        File.Delete($@"{WorkDir}/Resources/message.txt");
        var map = _mapLoader.Load(_configuration.MapFilePath);
        try
        {
            _explorationSimulator.ExploringSimulator(map, _configuration, 1, _commandCenters);
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