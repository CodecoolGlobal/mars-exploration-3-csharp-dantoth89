using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;

public class Command_Center:ICommand_Center
{
   public int Id { get; }
   public string Symbol { get; } = "@";
   public int Radius { get; }
   public bool IsItActive { get; set; } = false;
   public List<MarsRoverModel> Rovers { get; set; }
   public Dictionary<string, int> DeliveredResources { get; set; } = new Dictionary<string, int>();
   private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
public Coordinate Position { get; }
   public Command_Center(int id, Coordinate position, MarsRoverModel actualRover, ConfigurationModel config)
   {
      Id = id;
      Position = position;
      Rovers = new List<MarsRoverModel>() {actualRover};
      Radius = config.CommandCenterSight;

   }
 
   
   public bool DoWeHaveEnoughMineralForRover()
   {
      
         throw new NotImplementedException();
      
   }

   public bool DoWeHaveSlotForAnotherRover()
   {
      throw new NotImplementedException();
   }

   public void UseMineralsForConstruction()
   {
      throw new NotImplementedException();
   }
}