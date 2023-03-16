using Codecool.MarsExploration.MapExplorer.Configuration.CommandCenter.Service;
using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Exploration;
using Codecool.MarsExploration.MapExplorer.MarsRover.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Exploring;

public class PlacingCommandCenter
{
    private ExplorationSimulatorSteps _explorationSimulatorSteps;
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    private Coordinate _possibleMineral;
    private Coordinate _possibleWater;

    public PlacingCommandCenter(ExplorationSimulatorSteps explorationSimulatorSteps)
    {
        _explorationSimulatorSteps = explorationSimulatorSteps;
    }

    private bool NewResourceFound()
    {
        var isTrue = _explorationSimulatorSteps.NumberOfResources > _explorationSimulatorSteps.PrevNumberOfResources;
        _explorationSimulatorSteps.PrevNumberOfResources = _explorationSimulatorSteps.NumberOfResources;
        return isTrue;
    }

    private IEnumerable<Coordinate> ScanForPlaceForCommandCenter(SimulationContext simulationContext)
    {
        var minerals = simulationContext.Rover.FoundResources.Where(res => res.foundResourceSymbol == "%");
        var waters = simulationContext.Rover.FoundResources.Where(res => res.foundResourceSymbol == "*");
        List<List<Coordinate>> listOfAreaLists = new List<List<Coordinate>>();
        foreach (var mineral in minerals)
        {
            var water= waters.MinBy(water => Math.Abs(water.foundResourceCoordinate.X - mineral.foundResourceCoordinate.X) +
                                  Math.Abs(water.foundResourceCoordinate.Y - mineral.foundResourceCoordinate.Y));
            if (AreBothCoordinatesInSightRange(simulationContext, mineral, water))
            {
                _possibleMineral = mineral.foundResourceCoordinate;
                _possibleWater = water.foundResourceCoordinate;
                listOfAreaLists.Add(GetAreaOfResource(simulationContext, mineral)
                    .Intersect(GetAreaOfResource(simulationContext, water)).ToList());
            }
        }
        return listOfAreaLists.Count()==0? new List<Coordinate>(): listOfAreaLists.MaxBy(list=> list.Count);
    }
    private static bool AreBothCoordinatesInSightRange(SimulationContext simulationContext,
        (string foundResourceSymbol, Coordinate foundResourceCoordinate) mineral,
        (string foundResourceSymbol, Coordinate foundResourceCoordinate) water)
    {
        return Math.Abs(mineral.foundResourceCoordinate.X - water.foundResourceCoordinate.X) <=
               simulationContext.CommandCenterSight * 2
               &&
               Math.Abs(mineral.foundResourceCoordinate.Y - water.foundResourceCoordinate.Y) <=
               simulationContext.CommandCenterSight * 2;
    }

    private List<Coordinate> GetAreaOfResource(SimulationContext simulationContext,
        (string foundResourceSymbol, Coordinate foundResourceCoordinate) resource)
    {
        var rangeOfResource = new List<Coordinate>();
        for (int i = 0; i < simulationContext.CommandCenterSight; i++)
        {
            rangeOfResource.AddRange(_coordinateCalculator.GetAdjacentCoordinates(
                resource.foundResourceCoordinate, simulationContext.Map.Dimension, i));
        }

        return rangeOfResource;
    }

    private bool IsThereAnotherCommandCenter(IEnumerable<Coordinate> possiblePlacesForCMDC,
        List<Command_Center> commandCenters)
    {
        if (possiblePlacesForCMDC.Any() && commandCenters.Any())
            return possiblePlacesForCMDC.Intersect
            (commandCenters.Select
                (CMDC => CMDC.Position).ToList()).Any();
        else return false;
    }

    private Coordinate? FindPlaceCommandCenter(SimulationContext simulationContext, List<Command_Center> commandCenters)
    {
        Coordinate? newPlace = null;
        if (NewResourceFound())
        {
            var areaWithResources = ScanForPlaceForCommandCenter(simulationContext).ToList();
            if (!IsThereAnotherCommandCenter(areaWithResources, commandCenters) &&
                areaWithResources.Any(place => simulationContext.Map.Representation[place.X, place.Y] == null))
            {
                var newEmptyPlaces = areaWithResources.Where(
                    place => simulationContext.Map.Representation[place.X, place.Y] == null);
                newPlace = FindPlaceClosestToMiddle(newEmptyPlaces);
            }
        }

        return newPlace;
    }

    private Coordinate FindPlaceClosestToMiddle(IEnumerable<Coordinate> newEmptyPlaces)
    {
        Coordinate newPlace;
        int averageX = (int)newEmptyPlaces.Average(newEmptyPlace => newEmptyPlace.X);
        int averageY = (int)newEmptyPlaces.Average(newEmptyPlace => newEmptyPlace.Y);
        int newX = newEmptyPlaces.MinBy(newEmptyPlace => Math.Abs(newEmptyPlace.X - averageX)).X;
        int newY = newEmptyPlaces.MinBy(newEmptyPlace => Math.Abs(newEmptyPlace.Y - averageY)).Y;
        newPlace = new Coordinate(newX, newY);
        return newPlace;
    }

    public Command_Center? PlaceCommandCenter(SimulationContext simulationContext, List<Command_Center> commandCenters,
        ConfigurationModel config)
    {
        Command_Center? cmdCenter = null;
        var position = FindPlaceCommandCenter(simulationContext, commandCenters);
        if (position != null)
        {
            cmdCenter = new Command_Center(commandCenters.Count + 1,
                position,
                simulationContext.Rover, config,
                new Dictionary<Resources, Coordinate>() { { Resources.Mineral, _possibleMineral }, { Resources.Water, _possibleWater } });
           Console.WriteLine($"mineral : {_possibleMineral}, water: {_possibleWater}, cmdC: {cmdCenter.Position}");
            return cmdCenter;
        }
        return cmdCenter;
    }
}