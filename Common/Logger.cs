namespace CssWatcher.Common;

public static class Logger
{
    public static void LogInformation(string format, params object[] args)
    {
        Console.WriteLine($"{DateTime.Now:t}:[INFO]: {format}", args);
    }
    public static void LogVerbose(string format, params object[] args)
    {
        Console.WriteLine($"{DateTime.Now:t}:[INFO]: {format}", args);
    }
}