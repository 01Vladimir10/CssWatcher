using CssWatcher.Common;
using Microsoft.Extensions.Options;

namespace CssWatcher.Services;

public class FilesTrackerService : IFilesTrackerService
{
    private readonly Dictionary<string, string> _files = new();
    private readonly CssWatcherOptions _options;
    private const string StaticFilesPath = "css";
    private string RootPath => $"{_options.Url}/{StaticFilesPath}/";

    public FilesTrackerService(IOptions<CssWatcherOptions> options)
    {
        _options = options.Value;
    }

    public void AddFile(string localPath)
    {
        var clientPath = localPath.Replace(_options.Path, RootPath);
        clientPath = clientPath.Replace("\\", "/");
        clientPath = clientPath.Replace("//", "/");
        _files[localPath] = clientPath;
    }

    public string GetClientPath(string localPath)
    {
        return _files.ContainsKey(localPath) ? _files[localPath] : "";
    }

    public void RemoveFile(string localPath)
    {
        _files.Remove(localPath);
    }

    public List<string> GetAllClientFilePaths()
    {
        return _files.Values.ToList();
    }
}