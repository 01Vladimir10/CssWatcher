namespace CssWatcher.Services;

public interface ICssWatcherHandler
{
    public Task OnFiledScanned(List<string> filePaths);
    public Task OnFileChanged(string filePath);
    public Task OnFileDeleted(string filePath);
    public Task OnFileAdded(string filePath);

}