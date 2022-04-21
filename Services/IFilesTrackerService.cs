namespace CssWatcher.Services;

public interface IFilesTrackerService
{
    public void AddFile(string localPath);
    public string GetClientPath(string localPath);
    public void RemoveFile(string localPath);
    public List<string> GetAllClientFilePaths();
}