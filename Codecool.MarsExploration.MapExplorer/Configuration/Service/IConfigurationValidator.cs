using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.Service;

public interface IConfigurationValidator
{
    bool Validate(ConfigurationModel configuration, Map map);
}