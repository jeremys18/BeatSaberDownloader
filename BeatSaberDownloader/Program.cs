using Microsoft.Extensions.Primitives;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Diagnostics;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("LoggingDatabase");

// Configure sink options for MSSqlServer sink
var sinkOptions = new MSSqlServerSinkOptions
{
    SchemaName="Server",
    TableName = "Logs",
    AutoCreateSqlTable = true
};

// Column options can be customized if needed
var columnOptions = new ColumnOptions
{
    TimeStamp = { ColumnName = "LoggedAt", ConvertToUtc = true },
};

// Create Serilog logger
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("Logs/Server.log", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Information)
    .WriteTo.MSSqlServer(connectionString, sinkOptions: sinkOptions, columnOptions: columnOptions, restrictedToMinimumLevel: LogEventLevel.Information)
    .CreateLogger();

// Replace default logging with Serilog
builder.Host.UseSerilog();


// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddMediatR(cfg => {
    cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzkwMDM1MjAwIiwiaWF0IjoiMTc1ODU4MjQwOCIsImFjY291bnRfaWQiOiIwMTk5NzNhZGEyODU3MzY5YjJiYWVkOTUyMDc0MDNhNyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazVzdHcweWg3emR2bm5nOXExMnEza215Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.N-f0QHQYEFJUtflqT_Nqy9AT_RStQGwbIvtgvet3vYqYYgHXv3PV4g7HNEHRpmMCUgwVk7CsNTqAg1LXW-fspVzUvxVEINKic1kESiMQspPzcETZ-Wa9L-O3-f4AP807H4sMdDzzqZcakJl8vTBlvNwpnZ8CB0zukSMFARpQyhj16okg06elHr7YJyJAA2OV7WdgNpAJekXMa3iGzUVxXb-oFxJDS4VSE5xrbvurWoJI2gj4vrpURlykxSCwEJVaj-I_NewzMSoRllMMUDYI-VAH9a3w0G2gUGljUfRq_5c3eeSfSX7hr39plXRhePcoGLyd2G1RPgwD4IACCahMuw";
 });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Logging middleware: logs method, path, client IP and response status code for every incoming request
app.Use(async (context, next) =>
{
    // Resolve an ILogger from DI so logs go through Serilog and configured sinks
    var logger = context.RequestServices.GetRequiredService<ILoggerFactory>().CreateLogger("RequestLogger");

    var path = context.Request.Path + context.Request.QueryString;
    // Prefer X-Forwarded-For if present (may contain a list)
    string clientIp = "unknown";
    if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var xff) && !StringValues.IsNullOrEmpty(xff))
    {
        clientIp = xff.ToString().Split(',')[0].Trim();
    }
    else
    {
        clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    logger.LogInformation("Request {Method} {Path} from {ClientIP}...", context.Request.Method, path, clientIp);

    var sw = Stopwatch.StartNew();
    await next();
    sw.Stop();

    var statusCode = context.Response?.StatusCode ?? 0;
    logger.LogInformation("Responded {StatusCode} in {ElapsedMs}ms.", statusCode, sw.ElapsedMilliseconds);
});

app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5000");

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

try
{
    Log.Information("Starting web host");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
