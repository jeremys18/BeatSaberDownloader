using BeatSaberDownloader.UpdateWatchService;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var connectionString = Environment.GetEnvironmentVariable("DBUPDATEWATCH_SERVICE_LOG_DB") ?? "Server=.;Database=BeatSaberDownloader;Trusted_Connection=True;TrustServerCertificate=True;";

var sinkOptions = new MSSqlServerSinkOptions
{
    SchemaName = "UpdateWatchService",
    TableName = "Logs",
    AutoCreateSqlTable = true
};

var columnOptions = new ColumnOptions
{
    TimeStamp = { ColumnName = "LoggedAt", ConvertToUtc = true }
};

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.File(@"G:\BeatSaber\Logs\UpdateService.log", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: sinkOptions,
        columnOptions: columnOptions,
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "BeatSaber Update Watch Service";
});
builder.Services.AddHostedService<Worker>();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Logging.AddConsole();

var host = builder.Build();
host.Run();
