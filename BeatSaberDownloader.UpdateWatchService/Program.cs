using BeatSaberDownloader.UpdateWatchService;
using Serilog;
using Serilog.Extensions.Logging; 

Log.Logger = new LoggerConfiguration()
    .WriteTo.File(@"c:\BeatSaber\Logs\UpdateService.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

// Replace builder.Logging.AddSerilog(); with the following:
builder.Logging.AddProvider(new SerilogLoggerProvider(Log.Logger, dispose: false));

builder.Services.AddHostedService<Worker>();

var host = builder.Build();
host.Run();
