using NStack;
using PhantomKit.Helpers;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Themes;
using Terminal.Gui;

namespace PhantomKit.Models.StatusBars;

public class PhantomKitStatusBar : StatusBar
{
    public readonly HostedGuiCommandBase GuiCommand;

    protected readonly StatusItem TitleItem;

    public PhantomKitStatusBar(HostedGuiCommandBase guiCommand)
    {
        this.GuiCommand = guiCommand;
        this.TitleItem = new StatusItem(shortcut: Key.Null,
            title: Constants.ProgramName /* Application.Driver.GetType().Name */,
            action: null);
        this.Items = this.BuildItems();
    }

    public StatusItem StatusItemAt(int index)
    {
        if (index < 0 || index > this.Items.Length) throw new ArgumentOutOfRangeException(paramName: nameof(index));
        return this.Items[index];
    }

    public void SetStatus(string title, bool prependProgram = true)
    {
        this.TitleItem.Title = ustring.Make(str: prependProgram
            ? string.Concat(arg0: Constants.ProgramName,
                arg1: ' ',
                arg2: title)
            : title);
    }

    private StatusItem[] BuildItems()
    {
        return new[]
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
            this.TitleItem,
        };
    }


    public virtual void LightDarkClick()
    {
        this.GuiCommand.DarkMode = !this.GuiCommand.DarkMode;
        var theme = this.GuiCommand.DarkMode
            ? HumanEditableTheme.Themes.Dark
            : HumanEditableTheme.Themes.Blue;
        GuiUtilities.SetColorsFromTheme(theme: theme);
        this.GuiCommand.UpdateTheme(theme: theme);
    }

    public virtual void QuitClick()
    {
        if (GuiUtilities.QuitPrompt() && Application.Top is not null) Application.Top.Running = false;
    }
}