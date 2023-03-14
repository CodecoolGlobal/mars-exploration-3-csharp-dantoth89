using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;

public class Command_Center:ICommand_Center
{
   private int _id;
   private Coordinate _position;
   private int _radius;
   private bool _isItActive;
   private List<MarsRoverModel> _rovers;
   private Dictionary<string, int> _deliveredResources;
   private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
public Coordinate Position { get; }
   public Command_Center(int id, Coordinate position, int radius, bool isItActive, List<MarsRoverModel> rovers, Dictionary<string, int> deliveredResources)
   {
      _id = id;
      Position = position;
      _radius = radius;
      _isItActive = isItActive;
      _rovers = rovers;
      _deliveredResources = deliveredResources;
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