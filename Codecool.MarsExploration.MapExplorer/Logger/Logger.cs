namespace Codecool.MarsExploration.MapExplorer.Logger;

public class Logger : ILogger
{
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;
 
    public void Log(string message)
    {
        Console.WriteLine(message);
    }
    
    public void Log(string message, string logName)
    {
        var date = DateTime.Now;
        var timeForFile = date.ToString().Replace(". ", "-").Replace(":", "-");
        
        var filePath = $@"{WorkDir}\Resources\{logName}_{timeForFile}.txt";
        File.WriteAllText(filePath, message);
    }

}