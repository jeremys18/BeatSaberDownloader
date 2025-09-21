using BeatSaberDownloader.DownloadService;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(@"G:\BeatSaber\Logs\SongDownloaderSrv.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "BeatSaber Song Downloader Service";
});
builder.Services.AddHostedService<Worker>();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Logging.AddConsole();

var host = builder.Build();
host.Run();
