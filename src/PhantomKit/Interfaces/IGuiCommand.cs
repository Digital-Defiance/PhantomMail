using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Menus;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomMail.Windows;
using Serilog;

namespace PhantomKit.Interfaces;

public interface IGuiCommand : IDisposable
{
    /// <summary>
    ///     Contains a dictionary of GuiCommandBase instances, keyed by the Type.
    ///     For instance we can only have one instance of the SendEmailGuiCommand.
    ///     It is assumed that GuiCommand instances compete for a physical console window.
    ///     The dictionary allows switching between commands (applications), and lets commands communicate with each other
    ///     as well as enforces the singleton pattern.
    /// </summary>
    private static readonly Dictionary<Type, HostedGuiCommandBase> GuiCommandSingletons = new();

    /// <summary>
    ///     Singleton accessor for the instance of the implementing type.
    /// </summary>
    public HostedGuiCommandBase Instance { get; init; }

    /// <summary>
    ///     Whether this command (application) is currently running.
    /// </summary>
    public bool Running { get; }

    bool MouseEnabled { get; init; }
    IConfiguration Configuration { get; }
    IConsole Console { get; }
    ILogger Logger { get; }
    PhantomKitMainMenu Menu { get; }
    PhantomKitStatusBar StatusBar { get; }
    PhantomKitWindow? Window { get; }
    bool DarkMode { get; set; }

    internal static bool SingletonMade(Type t)
    {
        return GuiCommandSingletons.ContainsKey(key: t);
    }

    internal static bool SingletonMade<T>()
    {
        return GuiCommandSingletons.ContainsKey(key: typeof(T));
    }

    internal static void SetSingleton<T>(T instance)
        where T : HostedGuiCommandBase
    {
        if (GuiCommandSingletons.ContainsKey(key: typeof(T)))
            throw new Exception(message: $"Singleton of type {typeof(T).Name} already exists");
        GuiCommandSingletons.Add(key: typeof(T),
            value: instance);
    }

    internal static void SetSingleton(HostedGuiCommandBase instance)
    {
        if (GuiCommandSingletons.ContainsKey(key: instance.GetType()))
            throw new Exception(message: $"Singleton of type {instance.GetType()} already exists");
        GuiCommandSingletons.Add(key: instance.GetType(),
            value: instance);
    }

    public static HostedGuiCommandBase GetInstance<T>()
        where T : HostedGuiCommandBase, IGuiCommand
    {
        if (GuiCommandSingletons.ContainsKey(key: typeof(T))) return GuiCommandSingletons[key: typeof(T)];
        var singleton = Activator.CreateInstance<T>();
        GuiCommandSingletons.Add(key: typeof(T),
            value: singleton);
        return singleton;
    }

    public static HostedGuiCommandBase GetInstance(Type guiCommandType)
    {
        if (GuiCommandSingletons.ContainsKey(key: guiCommandType))
            return GuiCommandSingletons[key: guiCommandType];

        var singleton = Activator.CreateInstance(type: guiCommandType);

        if (singleton is HostedGuiCommandBase hostedGuiCommandBase)
            return hostedGuiCommandBase;

        throw new Exception(message: $"Singleton of type {guiCommandType.Name} is not a HostedGuiCommandBase");
    }

    public static T GetTypedInstance<T>()
        where T : HostedGuiCommandBase
    {
        if (!typeof(T).IsAssignableTo(targetType: typeof(HostedGuiCommandBase)))
            throw new Exception(message: $"Singleton of type {typeof(T).Name} is not a HostedGuiCommandBase");
        var command = GetInstance(guiCommandType: typeof(T));
        return (T) Convert.ChangeType(value: command,
            conversionType: typeof(T));
    }

    public void SetRunning(bool value);
    int OnExecute(CommandLineApplication app);
    void OnException(Exception ex);
    void OutputToConsole(string data);
    void OutputError(string message);
    void UpdateTheme(HumanEditableTheme theme);
    void Copy();
    void Cut();
    void Paste();
    void AskQuit();
    void SetTheme(HumanEditableTheme theme);
}