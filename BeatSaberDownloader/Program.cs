using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

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

app.Urls.Clear();
app.Urls.Add("http://0.0.0.0:5000");

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
