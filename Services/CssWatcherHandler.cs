using CssWatcher.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace CssWatcher.Services;

public class CssWatcherHandler : ICssWatcherHandler
{
    private readonly IHubContext<LiveCssHub> _hub;
    private readonly IFilesTrackerService _tracker;
    
    public CssWatcherHandler(IHubContext<LiveCssHub> hub, IFilesTrackerService tracker)
    {
        _hub = hub;
        _tracker = tracker;
    }

    public async Task OnFiledScanned(List<string> filePaths)
    {
        filePaths.ForEach(p => _tracker.AddFile(p));
        await _hub.Clients.All.SendAsync(LiveCssHubMethods.Initialized, _tracker.GetAllClientFilePaths());
    }

    public async Task OnFileChanged(string filePath)
    {
        var clientFilePath = _tracker.GetClientPath(filePath);
        await _hub.Clients.All.SendAsync(LiveCssHubMethods.FileChanged, clientFilePath);
    }

    public async Task OnFileDeleted(string filePath)
    {
        var clientFilePath = _tracker.GetClientPath(filePath);
        _tracker.RemoveFile(filePath);
        await _hub.Clients.All.SendAsync(LiveCssHubMethods.FileRemoved, clientFilePath);
    }

    public async Task OnFileAdded(string filePath)
    {
        _tracker.AddFile(filePath);
        var clientFilePath = _tracker.GetClientPath(filePath);
        await _hub.Clients.All.SendAsync(LiveCssHubMethods.FileAdded, clientFilePath);
    }
}