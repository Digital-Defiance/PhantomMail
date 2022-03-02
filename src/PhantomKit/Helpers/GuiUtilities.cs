using PhantomKit.Interfaces;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Helpers;

public static class GuiUtilities
{
    public static Toplevel GetNewTopLevel(IGuiCommand guiCommand)
    {
        var newTop = Toplevel.Create();
        newTop.ColorScheme = (guiCommand.DarkMode ? HumanEditableTheme.Themes.Dark : HumanEditableTheme.Themes.Blue)
            .TopLevel
            .ToColorScheme();
        newTop.Add(view: guiCommand.Menu);
        newTop.Add(view: guiCommand.StatusBar);
        newTop.Add(view: guiCommand.Window);
        return newTop;
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