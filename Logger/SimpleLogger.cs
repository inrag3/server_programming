namespace Logger;

public class SimpleLogger : ILogger
{
    private readonly object _lock = new();
    private readonly string _path;
    public SimpleLogger(string path = "server.log")
    {
        _path = path;
    }
    public void Log(string level, string message)
    {
        var entry = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {level} | {message}";

        lock (_lock)
        {
            File.AppendAllText(_path, entry + Environment.NewLine);
        }

        Console.WriteLine(entry);
    }
    
    public void Info(string message) => Log("[INFO]", message);
    public void Error(string message) => Log("[ERROR]", message);
}