using System.Text.Json;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;

namespace Codecool.MarsExploration.MapExplorer.ParseJSON;

public class JSONHandler
{
    private JSONConfig source;

    private void JSONReader()
    {
        using (StreamReader r = new StreamReader("Config.json"))
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
            neededResource,
            source.TimeOutSteps,
            source.RoverCost,
            source.RoverSight,
            source.CommandCenterSight,
            source.FileLogger
        );
        return newConfig;
    }
}