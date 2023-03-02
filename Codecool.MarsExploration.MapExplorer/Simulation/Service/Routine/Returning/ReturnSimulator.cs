using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;
using Codecool.MarsExploration.MapGenerator.Calculators.Model;
using Codecool.MarsExploration.MapGenerator.Calculators.Service;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Returning;

public class ReturnSimulator : IReturnSimulator
{
    
    private ILogger _logger;
    private IReturningRoutine _returningRoutine = new ReturningRoutine();
    private ICoordinateCalculator _coordinateCalculator = new CoordinateCalculator();
    public ReturnSimulator(ILogger logger)
    {
        _logger = logger;
    }
    public void ReturningSimulator(SimulationContext simulationContext)
    {
        Coordinate nextPlace = new Coordinate(-1,-1);
       while (!_coordinateCalculator.GetAdjacentCoordinates(simulationContext.LocationOfSpaceship, simulationContext.Map.Dimension,1).Contains(nextPlace))
       {
            var adjacentCoordinates = _coordinateCalculator.GetAdjacentCoordinates(simulationContext.Rover.CurrentPosition, simulationContext.Map.Dimension, 1);
            var possibleCoordinates = adjacentCoordinates.Where(coord => simulationContext.Map.Representation[coord.X, coord.Y] == null);
               nextPlace =_returningRoutine.ReturnMovement(simulationContext, possibleCoordinates);
            simulationContext.Rover.CurrentPosition = nextPlace;
            simulationContext.VisitedPlaces.Add(nextPlace);
        }
        WhatDidTheRoverFind(simulationContext);
    }

    private void WhatDidTheRoverFind(SimulationContext simulationContext)
    {
        int numberOfWaters = 0;
        int numberOfMinerals = 0;
        foreach (var (foundResourceSymbol, foundResourceCoord) in simulationContext.Rover.FoundResources)
        {
            if (foundResourceSymbol == "*")
                numberOfWaters++;
            if (foundResourceSymbol == "%")
                numberOfMinerals++;
        }

        _logger.Log($"The {simulationContext.Rover.Id} is back. Found: {numberOfWaters} water and {numberOfMinerals} minerals");
    }
}

