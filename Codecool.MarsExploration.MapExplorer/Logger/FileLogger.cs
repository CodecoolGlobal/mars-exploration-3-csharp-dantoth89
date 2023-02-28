namespace Codecool.MarsExploration.MapExplorer.Logger;

public class FileLogger : ILogger
{
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;

    public void Log(string message)
    {
        var date = DateTime.Now;
        var timeForFile = date.ToString().Replace(". ", "-").Replace(":", "-");
        File.AppendAllText($"{WorkDir}Resources\\message_{timeForFile}.txt", message + Environment.NewLine);
    }
}