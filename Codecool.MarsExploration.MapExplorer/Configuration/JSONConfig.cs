namespace Codecool.MarsExploration.MapExplorer.ParseJSON;

public class JSONConfig
{
    public string FilePath{get; set;}
    public int LandingSpotX{get; set;}
    public int LandingSpotY{get; set;}
    public string NeededResourceSymbol{get; set;}
    public int TimeOutSteps{get; set;}
    public int RoverCost{get; set;}
    public int RoverSight{get; set;}
    public int CommandCenterCost { get; }
    public int CommandCenterSight{get; set;}
    public bool FileLogger{get; set;}
}