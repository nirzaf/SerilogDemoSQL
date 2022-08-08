using System.Globalization;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args); 

// Add services to the container.
var logger = new LoggerConfiguration()
  .MinimumLevel.Information()
  .ReadFrom.Configuration(builder.Configuration)
  .WriteTo.SQLite("serilog.db")
  .WriteTo.MSSqlServer(
    connectionString: "Server= ;Database=LogDb;Integrated Security=true;",
    sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEvents" })
  .WriteTo.Console()
  .WriteTo.File("../logs/webapi-.txt",
    rollingInterval: RollingInterval.Day,
    formatProvider: new DateTimeFormatInfo(),
    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}",
    restrictedToMinimumLevel: LogEventLevel.Information,
    retainedFileCountLimit: 30,
    flushToDiskInterval: TimeSpan.FromSeconds(5),
    rollOnFileSizeLimit: true)
  .Enrich.FromLogContext()
  .CreateLogger();
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();