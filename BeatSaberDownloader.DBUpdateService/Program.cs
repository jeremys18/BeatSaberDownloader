using BeatSaberDownloader.DBUpdateService;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Microsoft.EntityFrameworkCore;
using BeatSaberDownloader.Data.DBContext;

var connectionString = Environment.GetEnvironmentVariable("DBUPDATE_SERVICE_LOG_DB") ?? "Server=.;Database=BeatSaberDownloader;Trusted_Connection=True;TrustServerCertificate=True;";

var sinkOptions = new MSSqlServerSinkOptions
{
    SchemaName = "UpdateService",
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
    .WriteTo.File(@"G:\BeatSaber\Logs\DBUpdateService.log", rollingInterval: RollingInterval.Day)
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: sinkOptions,
        columnOptions: columnOptions,
        restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);
// Ensure appsettings.json is included as a configuration source
builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

// Register DbContext using connection string from configuration
var beatSaverConn = builder.Configuration.GetConnectionString("BeatSaver") ?? "Server=.;Database=BeatSaver;Trusted_Connection=True;TrustServerCertificate=True;";
builder.Services.AddDbContext<BeatSaverContext>(options => options.UseSqlServer(beatSaverConn));

builder.Services.AddWindowsService(options =>
{
    options.ServiceName = "BeatSaber DB Update Service";
});
builder.Services.AddHostedService<Worker>();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Logging.AddConsole();

var host = builder.Build();
host.Run();
