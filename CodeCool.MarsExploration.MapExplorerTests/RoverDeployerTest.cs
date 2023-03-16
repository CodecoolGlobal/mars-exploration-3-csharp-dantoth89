using System;
using System.Collections.Generic;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using NUnit.Framework;

namespace CodeCool.MarsExploration.MapExplorerTests;

public class RoverDeployerTest
{
    private readonly MapLoader _mapLoader = new MapLoader();
    private static string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private IRoverDeployer _roverDeployer;

    
    public static object[] ConfigCases =
    {
        new object[] {new ConfigurationModel($@"{WorkDir}/Resources/exploration-0.map",
            new Coordinate(1, 1), new List<string>() { "*", "%" }, 100, 2,2,3,20,true)},
        new object[] {new ConfigurationModel($@"{WorkDir}/Resources/exploration-1.map",
            new Coordinate(3, 3), new List<string>() { "@", "~" }, 100,2,2,3,20,true)},
        new object[] {new ConfigurationModel($@"{WorkDir}/Resources/exploration-2.map",
                new Coordinate(16, 16), new List<string>() { "÷", "×" }, 100,2,2,3,20,true)}
    };


    [TestCaseSource(nameof(ConfigCases))]
    public void TestRoverExists(ConfigurationModel config)
    {
        _roverDeployer = new RoverDeployer(config, _coordinateCalculator);
        var rover = _roverDeployer.DeployMarsRover(1, _mapLoader.Load(config.MapFilePath), config.LandingSpot);
        Assert.That(rover.GetType(), Is.EqualTo(typeof(MarsRoverModel)));
    }

    [TestCaseSource(nameof(ConfigCases))]
    public void TestRoverLandedOnEmptySpace(ConfigurationModel config)
    {
        _roverDeployer = new RoverDeployer(config, _coordinateCalculator);
        var map = _mapLoader.Load(config.MapFilePath);
        var rover = _roverDeployer.DeployMarsRover(1, map, config.LandingSpot);
        var landingCoordinate = rover.CurrentPosition;
        Assert.That(map.Representation[landingCoordinate.X, landingCoordinate.Y], Is.Null);
    }
}
 