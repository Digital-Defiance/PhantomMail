using System.Diagnostics;
using System.Globalization;
using McMaster.Extensions.CommandLineUtils;
using PhantomKit.Helpers;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Extensions;

public static class GuiCommandExtensions
{
    public static int OnExecute(this HostedGuiCommandBase hostedGuiCommand, CommandLineApplication app)
    {
        if (Debugger.IsAttached)
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(name: "en-US");

        hostedGuiCommand.SetTheme(
            theme: hostedGuiCommand.DarkMode ? HumanEditableTheme.Themes.Dark : HumanEditableTheme.Themes.Blue);
        ;

        hostedGuiCommand.StatusBar.SetStatus(title: "[no vault]");

        var newTop = GuiUtilities.GetNewTopLevel(newTop: Application.Top);
        GuiUtilities.SetupTopLevel(guiCommand: hostedGuiCommand,
            newTop: newTop);
        Application.Run(view: newTop);
        return 0;
    }

    public static void OnException(this HostedGuiCommandBase hostedGuiCommand, Exception ex)
    {
        hostedGuiCommand.OutputError(message: ex.Message);
        // ReSharper disable StructuredMessageTemplateProblem
        hostedGuiCommand.Logger.Error(exception: ex,
            messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        hostedGuiCommand.Logger.Debug(exception: ex,
            messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        // ReSharper restore StructuredMessageTemplateProblem
    }

    public static void OutputToConsole(this HostedGuiCommandBase hostedGuiCommand, string data)
    {
        hostedGuiCommand.Console.BackgroundColor = ConsoleColor.Black;
        hostedGuiCommand.Console.ForegroundColor = ConsoleColor.White;
        hostedGuiCommand.Console.Out.Write(value: data);
        hostedGuiCommand.Console.ResetColor();
    }

    public static void OutputError(this HostedGuiCommandBase hostedGuiCommand, string message)
    {
        hostedGuiCommand.Console.BackgroundColor = ConsoleColor.Red;
        hostedGuiCommand.Console.ForegroundColor = ConsoleColor.White;
        hostedGuiCommand.Console.Error.WriteLine(value: message);
        hostedGuiCommand.Console.ResetColor();
    }

    public static void UpdateTheme(this HostedGuiCommandBase hostedGuiCommand, HumanEditableTheme theme)
    {
        // set the color scheme for the existing elements
        // TODO: this is only true at the moment. SetTheme will be called from a menu later.
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        hostedGuiCommand.Window!.ColorScheme = theme.Base.ToColorScheme();
        hostedGuiCommand.Menu.ColorScheme = theme.Menu.ToColorScheme();
        // ReSharper enable ConditionIsAlwaysTrueOrFalse
    }

    public static void SetTheme(this HostedGuiCommandBase hostedGuiCommand, HumanEditableTheme theme)
    {
        Colors.TopLevel = theme.TopLevel.ToColorScheme();
        Colors.Base = theme.Base.ToColorScheme();
        Colors.Menu = theme.Menu.ToColorScheme();
        Colors.Error = theme.Error.ToColorScheme();
        Colors.Dialog = theme.Dialog.ToColorScheme();
        hostedGuiCommand.UpdateTheme(theme: theme);
    }

    public static void SuspendUi(this HostedGuiCommandBase hostedGuiCommand)
    {
        if (Application.Top?.Running != true) return;
        Application.Top.Remove(view: hostedGuiCommand.Window);
        Application.Shutdown();
        Console.BackgroundColor = ConsoleColor.Black;
        Console.Clear();
        hostedGuiCommand.SetRunning(value: false);
    }

    public static void ResumeUi(this HostedGuiCommandBase hostedGuiCommand)
    {
        Application.Init();
        if (hostedGuiCommand.Window != null)
        {
            hostedGuiCommand.Window.ColorScheme =
                (hostedGuiCommand.DarkMode ? HumanEditableTheme.Themes.Dark : HumanEditableTheme.Themes.Blue).Base
                .ToColorScheme();
            if (Application.Top.Subviews.Count < 1) Application.Top.Add(view: hostedGuiCommand.Window);
        }

        Application.Run();
        hostedGuiCommand.SetRunning(value: false);
    }

    public static void Copy(this HostedGuiCommandBase hostedGuiCommand)
    {
        var textField = (hostedGuiCommand.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        if (textField.SelectedLength != 0) textField.Copy();
    }

    public static void Cut(this HostedGuiCommandBase hostedGuiCommand)
    {
        var textField = (hostedGuiCommand.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        if (textField.SelectedLength != 0) textField.Cut();
    }

    public static void Paste(this HostedGuiCommandBase hostedGuiCommand)
    {
        var textField = (hostedGuiCommand.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        textField?.Paste();
    }

    public static void AskQuit(this HostedGuiCommandBase hostedGuiCommand)
    {
        if (GuiUtilities.QuitPrompt())
            hostedGuiCommand.Quit();
    }

    public static void Quit(this HostedGuiCommandBase hostedGuiCommand, int exitCode = 0)
    {
        hostedGuiCommand.SuspendUi();
        hostedGuiCommand.Dispose();
        Environment.Exit(exitCode: exitCode);
    }
}