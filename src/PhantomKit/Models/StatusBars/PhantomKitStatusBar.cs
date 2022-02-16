using PhantomKit.Helpers;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Models.StatusBars;

public class PhantomKitStatusBar : StatusBar
{
    public PhantomKitStatusBar(GuiCommand guiCommand)
    {
        this.Items = new[]
        {
            new(shortcut: Key.F1,
                title: "~F1~ Help",
                action: () => MessageBox.Query(
                    width: 50,
                    height: 7,
                    title: "Help",
                    message: "Helping",
                    "Ok")),
            new StatusItem(shortcut: Key.CtrlMask | Key.Q,
                title: "~^Q~ Quit",
                action: () =>
                {
                    if (GuiCommand.Quit() && Application.Top is not null) Application.Top.Running = false;
                }),
            new StatusItem(shortcut: Key.F10,
                title: "~F10~ Light/Dark",
                action: () =>
                {
                    if (!guiCommand.DarkMode)
                        GuiCommand.SetTheme(theme: HumanEditableTheme.Themes.Dark,
                            instance: guiCommand);
                    else
                        GuiCommand.SetTheme(theme: HumanEditableTheme.Themes.Blue,
                            instance: guiCommand);

                    guiCommand.DarkMode = !guiCommand.DarkMode;
                }),
            new StatusItem(shortcut: Key.Null,
                title: Constants.ProgramName /* Application.Driver.GetType().Name */,
                action: null),
        };
    }
}