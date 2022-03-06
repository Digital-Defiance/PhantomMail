using System.Diagnostics;
using System.Globalization;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PhantomKit.Helpers;
using PhantomKit.Interfaces;
using PhantomKit.Models.Menus;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomMail.Windows;
using Serilog;
using Terminal.Gui;

namespace PhantomKit.Models.Commands;

/// <summary>
///     Abstract, mainly example class
/// </summary>
[HelpOption(template: "--help")]
public abstract class HostedGuiCommandBase : IGuiCommand
{
    public const bool DefaultDarkMode = true;

    /// <summary>
    ///     Must be assigned before using with parameterless constructor
    /// </summary>
    public static IServiceProvider? ServiceProvider;

    protected readonly bool IsBox10X = true;

    public HostedGuiCommandBase()
    {
        // assuming we are in an IHostedService and Host was set before instantiating this class
        var logger = (ServiceProvider!.GetService(serviceType: typeof(ILogger)) as ILogger)!;
        var configuration = (ServiceProvider.GetService(serviceType: typeof(IConfiguration)) as IConfiguration)!;
        var console = (ServiceProvider.GetService(serviceType: typeof(IConsole)) as IConsole)!;

        if (IGuiCommand.SingletonMade(t: this.GetType()))
            throw new Exception(message: $"{this.GetType()} already made");
        IGuiCommand.SetSingleton(instance: this);
        this.Instance = this;
        this.Logger = logger;
        this.Configuration = configuration;
        this.Console = console;
        Application.Init();
        Application.HeightAsBuffer = true;
        this.Window = new PhantomKitWindow(guiCommand: this);
        this.Menu = new PhantomKitMainMenu(guiCommand: this);
        this.StatusBar = new PhantomKitStatusBar(guiCommand: this);
        var theme = this.DarkMode
            ? HumanEditableTheme.Themes.Dark
            : HumanEditableTheme.Themes.Blue;
        GuiUtilities.SetColorsFromTheme(theme: theme);
        this.UpdateTheme(theme: theme);
    }

    public HostedGuiCommandBase Instance { get; init; }
    public bool Running { get; private set; }

    public void SetRunning(bool value)
    {
        this.Running = value;
    }

    public abstract bool MouseEnabled { get; init; }
    public IConfiguration Configuration { get; protected init; }

    public IConsole Console { get; protected init; }
    public ILogger Logger { get; protected init; }

    public PhantomKitMainMenu Menu { get; protected init; }
    public PhantomKitStatusBar StatusBar { get; protected init; }
    public PhantomKitWindow? Window { get; protected init; }
    public bool DarkMode { get; set; } = DefaultDarkMode;

    public int OnExecute(CommandLineApplication app)
    {
        if (Debugger.IsAttached)
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(name: "en-US");

        var theme = this.DarkMode ? HumanEditableTheme.Themes.Dark : HumanEditableTheme.Themes.Blue;
        GuiUtilities.SetColorsFromTheme(
            theme: theme);
        this.UpdateTheme(theme: theme);
        this.StatusBar.SetStatus(title: "[no vault]");

        GuiUtilities.SetupTopLevel(
            guiCommand: this,
            top: Application.Top);
        Application.Run();
        return 0;
    }

    public void OnException(Exception ex)
    {
        this.OutputError(message: ex.Message);
        // ReSharper disable StructuredMessageTemplateProblem
        this.Logger.Error(exception: ex,
            messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        this.Logger.Debug(exception: ex,
            messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        // ReSharper restore StructuredMessageTemplateProblem
    }

    public void OutputToConsole(string data)
    {
        this.Console.BackgroundColor = ConsoleColor.Black;
        this.Console.ForegroundColor = ConsoleColor.White;
        this.Console.Out.Write(value: data);
        this.Console.ResetColor();
    }

    public void OutputError(string message)
    {
        this.Console.BackgroundColor = ConsoleColor.Red;
        this.Console.ForegroundColor = ConsoleColor.White;
        this.Console.Error.WriteLine(value: message);
        this.Console.ResetColor();
    }

    public void UpdateTheme(HumanEditableTheme theme)
    {
        // set the color scheme for the existing elements
        // TODO: this is only true at the moment. SetTheme will be called from a menu later.
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        this.Window!.ColorScheme = theme.Base.ToColorScheme();
        this.Menu.ColorScheme = theme.Menu.ToColorScheme();
        // ReSharper enable ConditionIsAlwaysTrueOrFalse
    }


    public void SuspendUi()
    {
        if (Application.Top?.Running != true) return;
        Application.Top.Remove(view: this.Window);
        Application.Shutdown();
        System.Console.BackgroundColor = ConsoleColor.Black;
        System.Console.Clear();
        this.SetRunning(value: false);
    }

    public void ResumeUi()
    {
        Application.Init();
        if (this.Window != null)
        {
            this.Window.ColorScheme =
                (this.DarkMode ? HumanEditableTheme.Themes.Dark : HumanEditableTheme.Themes.Blue).Base
                .ToColorScheme();
            if (Application.Top.Subviews.Count < 1) Application.Top.Add(view: this.Window);
        }

        Application.Run();
        this.SetRunning(value: false);
    }

    public void Copy()
    {
        var textField = (this.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        if (textField.SelectedLength != 0) textField.Copy();
    }

    public void Cut()
    {
        var textField = (this.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        if (textField.SelectedLength != 0) textField.Cut();
    }

    public void Paste()
    {
        var textField = (this.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        textField?.Paste();
    }

    public void AskQuit()
    {
        if (GuiUtilities.QuitPrompt())
            this.Quit();
    }

    public void Quit(int exitCode = 0)
    {
        this.SuspendUi();
        this.Dispose();
        Environment.Exit(exitCode: exitCode);
    }

    public void Dispose()
    {
        this.Menu?.Dispose();
        this.Window?.Dispose();
    }
}