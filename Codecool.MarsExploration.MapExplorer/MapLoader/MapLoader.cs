using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.MapLoader;

public class MapLoader : IMapLoader
{
    public Map Load(string mapFile)
    {
        var lines = File.ReadAllLines(mapFile);
        string[,] mapArray = new string[lines.Length, lines.Length];
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].ToCharArray();
            for (int j = 0; j < line.Length; j++)
            {
                mapArray[i, j] = line[j].ToString();
            }
        }
        return new Map(mapArray, true);
    }
}