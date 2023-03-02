using System;
using System.Collections.Generic;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using NUnit.Framework;

namespace CodeCool.MarsExploration.MapExplorerTests;

public class RoverDeployerTest
{
    private readonly MapLoader _mapLoader = new MapLoader();
    private string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
    private ICoordinateCalculator _coordinateCalculator;
    private ConfigurationModel _config1;
    private ConfigurationModel _config2;
    private ConfigurationModel _config3;
    [SetUp]
    public void Setup()
    {
        _coordinateCalculator = new CoordinateCalculator();
        _config1 = new ConfigurationModel($@"{WorkDir}/Resources/exploration-0.map",
            new Coordinate(1, 1), new List<string>() { "*", "%" }, 100);
        _config2 = new ConfigurationModel($@"{WorkDir}/Resources/exploration-1.map",
            new Coordinate(3, 3), new List<string>() { "@", "~" }, 100);
        _config3 = new ConfigurationModel($@"{WorkDir}/Resources/exploration-2.map",
            new Coordinate(6, 6), new List<string>() { "÷", "×" }, 100);
    }
       
    public object[] TestCasesForValidProperties =
    {
        new object[] {true, new Coordinate(1,1), new List<string>() { "*", "%" }, 100 },
        new object[] {true, new Coordinate(1,1), new List<string>() { "*", "%" }, 100 },
        new object[] {true, new Coordinate(1,1), new List<string>() { "*", "%" }, 100 }
    };
    
    [TestCaseSource(nameof(TestCasesForValidProperties))]
    public void TestRoverHasValidProperties(bool expected, Coordinate coordinate, IEnumerable<string> symbols, int timeout)
    {
        IRoverDeployer roverDeployer = new RoverDeployer(_config1, _coordinateCalculator);
        var rover = roverDeployer.DeployMarsRover(1, _mapLoader.Load(_config1.MapFilePath));
        
        Assert.That(expected, Is.EqualTo((rover.Id==$"rover-{}")));
    }
    
    [Test]
    public void TestRoverLandedOnEmptySpace()
    {
        
    }
        
//     public RoverDeployer(ConfigurationModel configuration, ICoordinateCalculator coordinateCalculator)
//     {
//         _configuration = configuration;
//         _coordinateCalculator = coordinateCalculator;
//     }
//
//     public MarsRoverModel DeployMarsRover(int numberOfRoversDeployed, Map map)
//     {
//         var possibleLandingCoordinates = _coordinateCalculator.GetAdjacentCoordinates(_configuration.LandingSpot, map.Dimension, 1);
//         var landingCoordinate = possibleLandingCoordinates.First(coord => map.Representation[coord.X, coord.Y] == null);
//         var numberOfRover = numberOfRoversDeployed+1;
//         var foundResources = new HashSet<(string, Coordinate)>();
//         return new MarsRoverModel($"rover-{numberOfRover}", landingCoordinate, 2, foundResources,
//             new HashSet<Coordinate>());
//     }
//
// }
}