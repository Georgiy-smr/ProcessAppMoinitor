using MonitorApp;
using MonitorApp.Settings;
using Serilog;


Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();


var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        services.Configure<AppStartSettings>(
            context.Configuration.GetSection(nameof(AppStartSettings)));
        services.AddSerilog(Log.Logger);
        services.AddHostedService<Worker>();
    })
    .UseWindowsService()
    .Build();

try
{
    host.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

//var builder = Host
//        .CreateApplicationBuilder(args)
//    ;



//builder.Services.AddHostedService<Worker>();
//builder.Services.AddSerilog(Log.Logger);
//builder.Services.Configure<AppStartSettings>(
//    builder.Configuration.GetSection(nameof(AppStartSettings)));




//var host = builder.Build();
//host.Run();
