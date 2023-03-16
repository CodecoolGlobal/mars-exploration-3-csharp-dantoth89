using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;
using Moq;
using NUnit.Framework;

namespace CodeCool.MarsExploration.MapExplorerTests;

public class ConfigurationValidatorTests
{
    private static readonly CoordinateCalculator CoordinateCalculator = new();
    private IConfigurationValidator _configurationValidator = new ConfigurationValidator(CoordinateCalculator);

    public static object[] OccupationCases =
    {
        new object[] { new Coordinate(1, 1), false },
        new object[] { new Coordinate(1, 2), true },
    };

    public static object[] ResourceCases =
    {
        new object[] { new[] { "#", "&" }, true },
        new object[] { new string[] {}, false },
    };

    private string[,] _mapRepresentation;

    [SetUp]
    public void SetUp()
    {
        _mapRepresentation = new string[3, 3];
    }

    [Test]
    [TestCase("", false)]
    [TestCase("filePath", true)]
    public void FilePathIsNotEmptyTest(string filePath, bool expected)
    {
        var configurationModel = new ConfigurationModel(filePath, new Coordinate(1, 1), new[] { "#", "&" }, 10, 2,2,3,20,true);
        string[,] mapRepresentation = new string[3, 3];
        var map = new Map(_mapRepresentation);
        var isValid = _configurationValidator.Validate(configurationModel, map);

        Assert.That(isValid, Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(OccupationCases))]
    public void LandingSpotOccupationTest(Coordinate landingSpot, bool expected)
    {
        var configurationModel = new ConfigurationModel("filePath", landingSpot, new[] { "#", "&" }, 10,2,2,3,20,true);
        _mapRepresentation[1, 1] = "#";
        var map = new Map(_mapRepresentation);
        var isValid = _configurationValidator.Validate(configurationModel, map);
        
        Assert.That(isValid, Is.EqualTo(expected));
    }

    [Test]
    [TestCaseSource(nameof(ResourceCases))]
    public void IsThereAnyResourcesToLookForTest(string[] resourcesToLookFor, bool expected)
    {
        var configurationModel = new ConfigurationModel("filePath", new Coordinate(1, 1), resourcesToLookFor, 10,2,2,3,20,true);
        var map = new Map(_mapRepresentation);
        var isValid = _configurationValidator.Validate(configurationModel, map);
        
        Assert.That(isValid, Is.EqualTo(expected));
    }

    [Test]
    [TestCase(5, true)]
    [TestCase(0, false)]
    public void TimeoutStepsTest(int timeoutSteps, bool expected)
    {
        var configurationModel = new ConfigurationModel("filePath", new Coordinate(1, 1), new[] { "#", "&" }, timeoutSteps,2,2,3,20,true);
        var map = new Map(_mapRepresentation);
        var isValid = _configurationValidator.Validate(configurationModel, map);
        
        Assert.That(isValid, Is.EqualTo(expected));
    }

    [Test]
    public void LandingSpotDoNotHasFreeAdjacentCoordinateTest()
    {
        var configurationModel = new ConfigurationModel("filePath", new Coordinate(1, 1), new[] { "#", "&" }, 10,2,2,3,20,true);
        for (int i = 0; i < _mapRepresentation.GetLength(0); i++)
        {
            for (int j = 0; j < _mapRepresentation.GetLength(1); j++)
            {
                if(!(i == 1 && j == 1)) _mapRepresentation[i, j] = "#";
            }
        }
        var map = new Map(_mapRepresentation);
        var isValid = _configurationValidator.Validate(configurationModel, map);
        
        Assert.That(isValid, Is.False);
    }
    
    [Test]
    public void LandingSpotHasFreeAdjacentCoordinateTest()
    {
        var configurationModel = new ConfigurationModel("filePath", new Coordinate(1, 1), new[] { "#", "&" }, 10,2,2,3,20,true);
        for (int i = 0; i < _mapRepresentation.GetLength(0); i+=2)
        {
            for (int j = 0; j < _mapRepresentation.GetLength(1); j+=2)
            {
                _mapRepresentation[i, j] = "#";
            }
        }
        var map = new Map(_mapRepresentation);
        var isValid = _configurationValidator.Validate(configurationModel, map);
        
        Assert.That(isValid, Is.True);
    }
}