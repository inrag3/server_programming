namespace Logger;

public interface ILogger
{
    public void Log(string level, string message);
    public void Info(string message);
    public void Error(string message);
}