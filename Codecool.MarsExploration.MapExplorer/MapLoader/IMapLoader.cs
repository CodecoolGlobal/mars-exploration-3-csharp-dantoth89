using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.MapLoader;

public interface IMapLoader
{
    //Constructor with Configuration record
    Map Load(string mapFile);
    
    //TESTS
}
