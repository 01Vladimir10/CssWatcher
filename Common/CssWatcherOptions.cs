using CommandLine;

namespace CssWatcher.Common;

public class CssWatcherOptions : ICssWatcherOptions
{
    [Option('p',"path", Required = false, HelpText = "The path to the directory to be mounted")]
    public string Path { get; set; } = Directory.GetCurrentDirectory();
    
    [Option('u',"url", Required = false, HelpText = "The url to listen on, i.e. https://localhost:4433")]
    public string Url { get; set; } = "https://localhost:44300";
    
    [Option('e',"extensions", Required = false, HelpText = "The file extensions to watch, i.e. .css, .css.map")]
    public IEnumerable<string> FileExtensions { get; set; } = new List<string> { ".css", ".css.map" };
    
    [Option('v',"verbose", Required = false, HelpText = "Prints all messages to standard output.")]
    public bool Verbose { get; set; } = false;

    [Option('r', "recursive", Required = false, HelpText = "Recursively watch subdirectories.")]
    public bool Recursive { get; set; } = true;

    public override string ToString()
    {
        return $"{nameof(Path)}: {Path}, {nameof(Url)}: {Url}, {nameof(FileExtensions)}: {FileExtensions}, {nameof(Verbose)}: {Verbose}, {nameof(Recursive)}: {Recursive}";
    }
}