using CommandLine;
using CssWatcher.Common;
using CssWatcher.Hubs;
using CssWatcher.Services;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
ICssWatcherOptions appOptions = new CssWatcherOptions();

// Add services to the container.
Parser.Default.ParseArguments<CssWatcherOptions>(args)
    .WithParsed(options => builder.Services.Configure<ICssWatcherOptions>(o =>
    {
        o.Path = options.Path;
        o.Recursive = options.Recursive;
        o.Url = options.Url;
        o.FileExtensions = options.FileExtensions;
        o.Verbose = options.Verbose;
        appOptions = options;
    }));
builder.Services.AddLogging(l => l.ClearProviders());
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IFilesTrackerService, FilesTrackerService>();
builder.Services.AddTransient<ICssWatcherHandler, CssWatcherHandler>();
builder.Services.AddHostedService<FileWatcherService>();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        policyBuilder =>
        {
            policyBuilder.WithOrigins("https://example.com")
                .AllowAnyHeader()
                .SetIsOriginAllowed(_ => true)
                .WithMethods("GET", "POST")
                .AllowCredentials();
        });
});

builder.Services.AddSignalR();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.Urls.Clear();
app.Urls.Add(appOptions.Url);
Console.WriteLine(appOptions.Path);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(appOptions.Path),
    RequestPath = "/css"
});
app.UseHttpsRedirection();

app.UseAuthorization();
app.UseCors();
app.MapHub<LiveCssHub>("/liveCss");

app.MapControllers();

app.Run();
