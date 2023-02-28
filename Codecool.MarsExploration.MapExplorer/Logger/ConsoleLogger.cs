namespace Codecool.MarsExploration.MapExplorer.Logger;

public class ConsolLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
}