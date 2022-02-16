using System.Text;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PhantomKit.Helpers;
using PhantomMail.CLI.Commands;
using PhantomMail.Menus;
using Serilog;
using Serilog.Extensions.Logging;
using Terminal.Gui;

namespace PhantomMail.CLI;

public static class PhantomMail
{
    public delegate MenuItem MenuItemDelegate(MenuItemDetails menuItem);

    public static IConfiguration? Configuration { get; private set; }


    private static async Task<int> Main(string[] args)
    {
        if (args.Any() && args.Contains(value: "-usc")) Application.UseSystemConsole = true;

        Console.OutputEncoding = Encoding.Default;

        // TODO: combine appsettings.json and settings.vault.json (deprecate appsettings.json)
        Configuration = new ConfigurationBuilder()
            .SetBasePath(basePath: Directory.GetCurrentDirectory())
            .AddJsonFile(path: Utilities.AppSettingsFile,
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
                services.AddLogging(configure: builder =>
                {
                    builder.AddConsole();
                    builder.ClearProviders();
                    builder.AddProvider(provider: new SerilogLoggerProvider(logger: Log.Logger));
                    var minimumLevel = Configuration.GetSection(key: "Serilog:MinimumLevel")?.Value;
                    if (!string.IsNullOrEmpty(value: minimumLevel))
                        builder.SetMinimumLevel(level: Enum.Parse<LogLevel>(value: minimumLevel));
                });
                services.AddSingleton(implementationInstance: Log.Logger);
                services.AddSingleton(implementationInstance: PhysicalConsole.Singleton);
            })
            .UseConsoleLifetime();

        var cancellationTokenSource = new CancellationTokenSource();
        var cancellationToken = cancellationTokenSource.Token;
        using var host = hostBuilder
            .UseCommandLineApplication<PhantomMailGuiCommand>(args: args)
            .Build();
        var result = await host
            .RunCommandLineApplicationAsync(cancellationToken: cancellationToken);
        try
        {
            await host.WaitForShutdownAsync(token: cancellationToken);
        }
        catch (ObjectDisposedException exception)
        {
            Log.Error(exception: exception,
                messageTemplate: "Host was disposed");
        }

        return result;
    }
}