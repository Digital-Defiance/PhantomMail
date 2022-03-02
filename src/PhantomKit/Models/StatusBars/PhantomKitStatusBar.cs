using PhantomKit.Helpers;
using PhantomKit.Interfaces;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Models.StatusBars;

public class PhantomKitStatusBar : StatusBar
{
    private readonly HostedGuiCommandBase _command;

    public PhantomKitStatusBar(Type guiCommandType)
    {
        this._command = IGuiCommand.GetInstance(guiCommandType: guiCommandType);
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
                action: this.QuitClick),
            new StatusItem(shortcut: Key.F10,
                title: "~F10~ Light/Dark",
                action: this.LightDarkClick),
            new StatusItem(shortcut: Key.Null,
                title: Constants.ProgramName /* Application.Driver.GetType().Name */,
                action: null),
        };
    }


    public virtual void LightDarkClick()
    {
        this._command.SetTheme(theme: !this._command.DarkMode
            ? HumanEditableTheme.Themes.Dark
            : HumanEditableTheme.Themes.Blue);

        this._command.DarkMode = !this._command.DarkMode;
    }

    public void QuitClick()
    {
        if (GuiUtilities.QuitPrompt() && Application.Top is not null) Application.Top.Running = false;
    }
}