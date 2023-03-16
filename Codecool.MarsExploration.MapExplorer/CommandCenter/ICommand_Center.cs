using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;

public interface ICommand_Center
{
    public void ActivateWhenBuilt();

    public bool DoWeHaveEnoughMineralForRover();

    public bool DoWeHaveSlotForAnotherRover(Map map);

    public void UseMineralsForConstruction(int cost);
}