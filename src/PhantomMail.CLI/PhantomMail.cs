using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhantomMail.Menus;
using Serilog;
using Serilog.Extensions.Logging;
using Terminal.Gui;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace PhantomMail;

public static class PhantomMail
{
    public delegate MenuItem MenuItemDelegate(MenuItemDetails menuItem);

    public static IConfiguration? Configuration { get; private set; }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args: args)
            .ConfigureServices(configureDelegate: (hostContext, services) => { });
    }

    private static async Task<int> Main(string[] args)
    {
        if (args.Any() && args.Contains(value: "-usc")) Application.UseSystemConsole = true;

        Console.OutputEncoding = Encoding.Default;

        Configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: Directory.GetCurrentDirectory())
            .AddJsonFile(path: AppDomain.CurrentDomain.BaseDirectory + "\\appsettings.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();

        var logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration: Configuration)
            .Enrich.FromLogContext()
            .CreateLogger();
        Log.Logger = logger;

        var hostBuilder = Host.CreateDefaultBuilder(args: args)
            .ConfigureServices(configureDelegate: (hostContext, services) =>
            {
                services.AddSingleton(implementationInstance: Configuration);
                services.AddScoped<IConfiguration>(implementationFactory: provider => Configuration);
                services.AddLogging(configure: config =>
                {
                    config.ClearProviders();
                    config.AddProvider(provider: new SerilogLoggerProvider(logger: Log.Logger));
                    var minimumLevel = Configuration.GetSection(key: "Serilog:MinimumLevel")?.Value;
                    if (!string.IsNullOrEmpty(value: minimumLevel))
                        config.SetMinimumLevel(level: Enum.Parse<LogLevel>(value: minimumLevel));
                });
                services.AddSingleton<Serilog.ILogger>(Log.Logger);
                services.AddSingleton<IConsole>(PhysicalConsole.Singleton);
                /*services.AddSingleton<GuiCommand>(new GuiCommand(
                    logger: Log.Logger,
                    configuration: Configuration,
                    console: PhysicalConsole.Singleton));*/
            })
            .UseConsoleLifetime();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        using var host = hostBuilder
            .UseCommandLineApplication<GuiCommand>(args)
            .Build();
        var result = await host
            .RunCommandLineApplicationAsync(cancellationToken);
        await host.WaitForShutdownAsync(token: cancellationToken);
        return result;
    }
}