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
   public Coordinate ClosestMineral { get; }
   public Coordinate ClosestWater { get; }
   public List<MarsRoverModel> Rovers { get; set; }
   private ConfigurationModel _configuration;
   public Dictionary<string, int> DeliveredResources { get; set; } = new Dictionary<string, int>();
   private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
public Coordinate Position { get; }
   public Command_Center(int id, Coordinate position, MarsRoverModel actualRover, ConfigurationModel config, Dictionary<string, Coordinate> closestResources)
   {
      Id = id;
      Position = position;
      Rovers = new List<MarsRoverModel>() {actualRover};
      Radius = config.CommandCenterSight;
      _configuration = config;
      ClosestMineral = closestResources["mineral"];
      ClosestWater = closestResources["water"];
   }

   public void ActivateWhenBuilt()
   {
      if (!IsItActive && DeliveredResources["mineral"] == _configuration.CommandCenterCost)
      {
         DeliveredResources["mineral"] -= _configuration.CommandCenterCost;
         IsItActive = true;
         Console.WriteLine($"building cmdc{Id} is ready at {Position}");
      }
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