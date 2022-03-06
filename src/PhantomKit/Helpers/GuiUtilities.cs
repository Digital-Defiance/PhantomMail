using PhantomKit.Interfaces;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Helpers;

public static class GuiUtilities
{
    public static void SetupTopLevel(IGuiCommand guiCommand, Toplevel top)
    {
        top.ColorScheme = (guiCommand.DarkMode ? HumanEditableTheme.Themes.Dark : HumanEditableTheme.Themes.Blue)
            .TopLevel
            .ToColorScheme();
        top.Add(view: guiCommand.Menu);
        top.Add(view: guiCommand.StatusBar);
        top.Add(view: guiCommand.Window);
    }

    public static Toplevel GetNewTopLevel(Toplevel? clearExistingTop = null)
    {
        if (clearExistingTop is null)
            return Toplevel.Create();

        clearExistingTop.Clear();
        return clearExistingTop;
    }

    public static void SetColorsFromTheme(HumanEditableTheme theme)
    {
        Colors.TopLevel = theme.TopLevel.ToColorScheme();
        Colors.Base = theme.Base.ToColorScheme();
        Colors.Menu = theme.Menu.ToColorScheme();
        Colors.Error = theme.Error.ToColorScheme();
        Colors.Dialog = theme.Dialog.ToColorScheme();
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static void HelpPrompt()
    {
        MessageBox.Query(width: 50,
            height: 7,
            title: "Help",
            message: "This is a small help\nBe kind.",
            "Ok");
    }

    /// <summary>
    /// </summary>
    /// <returns></returns>
    public static bool QuitPrompt()
    {
        var n = MessageBox.Query(
            width: 50,
            height: 7,
            title: $"Quit {Constants.ProgramName}",
            message: $"Are you sure you want to quit {Constants.ProgramName}?",
            "Yes",
            "No");
        return n == 0;
    }

    public static void QuitClick()
    {
        if (QuitPrompt() && Application.Top is not null) Application.Top.Running = false;
    }
}