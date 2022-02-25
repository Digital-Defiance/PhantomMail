using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PhantomMail.Menus;
using PhantomMail.Windows;
using Terminal.Gui;

namespace PhantomMail;

public static class PhantomMail
{
    public delegate MenuItem MenuItemDelegate(MenuItemDetails menuItem);

    public static Action Running = PhantomMailWindow.Editor;

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        return Host.CreateDefaultBuilder(args: args)
            .ConfigureServices(configureDelegate: (hostContext, services) => { });
    }

    private static async Task Main(string[] args)
    {
        if (args.Any() && args.Contains(value: "-usc")) Application.UseSystemConsole = true;

        Console.OutputEncoding = Encoding.Default;

        await Host.CreateDefaultBuilder(args: args)
            .ConfigureServices(configureDelegate: (hostContext, services) =>
            {
                services.AddLogging();
                services.AddSingleton(implementationFactory: serviceProvider =>
                {
                    var guiContext = new GuiContext(
                        serviceProvider: serviceProvider);
                    return guiContext;
                });
            })
            .RunConsoleAsync();


        Running.Invoke();

        /* main loop */
        Application.Shutdown();
    }
}