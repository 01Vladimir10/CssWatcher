namespace CssWatcher.Common;

public interface ICssWatcherOptions
{
    public string Path { get; set; }
    public string Url { get; set; }
    public IEnumerable<string> FileExtensions { get; set; }
    public bool Verbose { get; set; }
    public bool Recursive { get; set; }
}