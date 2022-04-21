using System.Reflection;
using Microsoft.AspNetCore.Mvc;

namespace CssWatcher.Controllers;

[ApiController]
public class CssWatcherController : ControllerBase
{
    [HttpGet("/client")]
    public async Task<ContentResult> GetClient()
    {
        var rootPath = GetRootPath();
        var library = await ReadFileContent(Path.Combine(rootPath, "Scripts", "signal.js"));
        var client = await ReadFileContent(Path.Combine(rootPath, "Scripts", "client.js"));
        client = client.Replace("[END_POINT]", $"{Request.Scheme}://{Request.Host}");
        return Content($"{library}\n{client}", "application/javascript");
    }
    private static async Task<string> ReadFileContent(string path)
    {
        var stream = new FileStream(path, FileMode.Open, 
            FileAccess.Read, FileShare.ReadWrite);
        var reader = new StreamReader(stream);  
        return await reader.ReadToEndAsync();
    }
    private static string GetRootPath()
    {
        var assembly = Assembly.GetExecutingAssembly();
        return assembly.Location.Replace($"{assembly.GetName().Name ?? ""}.dll", "");
    }
}