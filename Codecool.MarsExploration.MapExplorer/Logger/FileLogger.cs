namespace Codecool.MarsExploration.MapExplorer.Logger;

public class FileLogger : ILogger
{
    private static readonly string WorkDir = AppDomain.CurrentDomain.BaseDirectory;

    public void Log(string message)
    {
        File.AppendAllText(@$"{WorkDir}Resources/message.txt", message + Environment.NewLine);
    }
}