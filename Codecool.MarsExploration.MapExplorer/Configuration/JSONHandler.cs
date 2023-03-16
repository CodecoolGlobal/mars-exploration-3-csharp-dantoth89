using System.Text.Json;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.ParseJSON;

public class JSONHandler
{
    private JSONConfig source;

    private void JSONReader()
    {
        string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
        var separator = Path.DirectorySeparatorChar;
        using (StreamReader r =
               new StreamReader(
                   $@"{WorkDir.Replace($@"{separator}bin{separator}Debug{separator}net6.0{separator}",
                       "")}{separator}Configuration{separator}Config.json"))
        {
            string json = r.ReadToEnd();
            source = JsonSerializer.Deserialize<JSONConfig>(json);
        }
    }

    public ConfigurationModel JSONConverter(string mapWorkDir)
    {
        JSONReader();
        var neededResource = source.NeededResourceSymbol.ToCharArray().Select(c => c.ToString()).ToList();

        var newConfig = new ConfigurationModel(
            mapWorkDir + source.FilePath,
            new Coordinate(source.LandingSpotX, source.LandingSpotY),
            neededResourcesSymbols:neededResource,
            timeoutSteps:source.TimeOutSteps,
            roverCost:source.RoverCost,
            roverSight:source.RoverSight,
            commandCenterCost:source.CommandCenterCost,
            commandCenterSight: source.CommandCenterSight,
            loggerType:source.FileLogger
        );
        return newConfig;
    }
}