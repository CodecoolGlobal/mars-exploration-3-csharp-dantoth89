using Codecool.MarsExploration.MapExplorer.Configuration.Model;
using Codecool.MarsExploration.MapExplorer.Configuration.Service;
using Codecool.MarsExploration.MapExplorer.Logger;
using Codecool.MarsExploration.MapExplorer.MapLoader;
using Codecool.MarsExploration.MapExplorer.MarsRover.Service;
using Codecool.MarsExploration.MapExplorer.Simulation.Model;
using Codecool.MarsExploration.MapExplorer.Simulation.Service.Analyzer;
using Codecool.MarsExploration.MapGenerator.MapElements.Model;

namespace Codecool.MarsExploration.MapExplorer.Simulation.Service.Routine.Returning;

public class ReturnSimulator : IReturnSimulator
{
    private ILogger _logger;

    public ReturnSimulator(ILogger logger)
    {
        _logger = logger;
    }
    public void ReturningSimulator(SimulationContext simulationContext)
    {
        Console.WriteLine(simulationContext.Rover.Id);
        simulationContext.Rover.CurrentPosition = simulationContext.LocationOfSpaceship;
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

// teleport back...