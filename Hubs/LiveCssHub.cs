using CssWatcher.Services;
using Microsoft.AspNetCore.SignalR;

namespace CssWatcher.Hubs;

public class LiveCssHub : Hub
{
    private readonly IFilesTrackerService _tracker;

    public LiveCssHub(IFilesTrackerService tracker)
    {
        _tracker = tracker;
    }
    public async Task Subscribe()
    {
        await Clients.All.SendAsync(LiveCssHubMethods.Initialized, _tracker.GetAllClientFilePaths());
    }
}

public static class LiveCssHubMethods
{
    public const string FileChanged = "FileChanged";
    public const string FileRemoved= "FileRemoved";
    public const string FileAdded = "FileAdded";
    public const string Initialized = "Initialized";
}