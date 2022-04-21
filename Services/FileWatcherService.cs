using CssWatcher.Common;
using Microsoft.Extensions.Options;

namespace CssWatcher.Services;

public class FileWatcherService : IHostedService
{
    private readonly CssWatcherOptions _options;
    private readonly ICssWatcherHandler _handler;
    private readonly Dictionary<string, DateTime> _lastModified = new();
    private FileSystemWatcher? _watcher;

    public FileWatcherService(IOptions<CssWatcherOptions> options, ICssWatcherHandler handler)
    {
        _options = options.Value;
        _handler = handler;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine();
        Console.WriteLine("Watching files in folder: {0}",_options.Path);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine();
        Console.WriteLine("\t--------------------------------------------------------------");
        Console.WriteLine("\t\tAdd the following snippet to your html file: ");
        Console.WriteLine($"\t\t<script src=\"{_options.Url}/client\"></script>");
        Console.WriteLine("\t--------------------------------------------------------------");
        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.White;
        
        var filters = _options.FileExtensions.Select(e => $"*{e}").ToList();
        _watcher = new FileSystemWatcher(_options.Path);
        filters.ForEach(_watcher.Filters.Add);
        _watcher.IncludeSubdirectories = _options.Recursive;
        _watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;
        _watcher.Created += OnFileCreated;
        _watcher.Deleted += OnFileDeleted;
        _watcher.Renamed += OnFileRenamed;
        _watcher.Changed += OnFileChanged;
        _watcher.EnableRaisingEvents = true;

        return ScanDirectory();
    }

    private async Task ScanDirectory()
    {
        var files = Directory.GetFiles(_options.Path, "*.*", _options.Recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly);
        var filesToWatch = files.Where(f => _options.FileExtensions.Contains(Path.GetExtension(f))).ToList();
        await _handler.OnFiledScanned(filesToWatch);
        Logger.LogInformation($"Watching {filesToWatch.Count} files: \n\t- {string.Join("\n\t- ", filesToWatch)}");
    }
    
    
    private async void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        if (_lastModified.ContainsKey(e.FullPath) && DateTime.Now - _lastModified[e.FullPath] < TimeSpan.FromMilliseconds(500))
        {
            return;
        }
        _lastModified[e.FullPath] = DateTime.Now;
        Logger.LogVerbose($"File changed {e.Name}");
        await _handler.OnFileChanged(e.FullPath);
    }

    private async void OnFileRenamed(object sender, RenamedEventArgs e)
    {
        Logger.LogVerbose($"File changed: {e.OldName} -> {e.Name}");
        await _handler.OnFileDeleted(e.OldFullPath);
        await _handler.OnFileAdded(e.FullPath);
    }

    private async void OnFileDeleted(object sender, FileSystemEventArgs e)
    {
        Logger.LogVerbose($"File deleted: {e.Name}");
        await _handler.OnFileDeleted(e.FullPath);
    }

    private async void OnFileCreated(object sender, FileSystemEventArgs e)
    {
        Logger.LogVerbose($"File created: {e.Name}");
        await _handler.OnFileAdded(e.FullPath);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _watcher?.Dispose();
        return Task.CompletedTask;
    }
}