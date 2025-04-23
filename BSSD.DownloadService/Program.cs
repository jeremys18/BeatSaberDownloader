using System.Runtime.Versioning; // Required for SupportedOSPlatform attribute
using App.WindowsService;
using BSSD.DownloadService;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "BeatSaber Song Downloader Service";
});

// Apply platform-specific code to suppress CA1416 warnings
#if WINDOWS
LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
#endif

builder.Services.AddSingleton<JokeService>();
builder.Services.AddHostedService<WindowsBackgroundService>();

var host = builder.Build();
host.Run();
