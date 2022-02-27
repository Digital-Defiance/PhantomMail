using PhantomKit.Helpers;
using PhantomKit.Models;
using Terminal.Gui;

namespace PhantomMail.StatusBars;

public class PhantomMailStatusBar : StatusBar
{
    public PhantomMailStatusBar(GuiCommand guiCommand)
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
            new StatusItem(shortcut: Key.F2,
                title: "~F2~ Load",
                action: guiCommand.LoadVault),
            new StatusItem(shortcut: Key.F3,
                title: "~F3~ Save",
                action: guiCommand.SaveVault),
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
                    var themeName =
                        guiCommand.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)];
                    if (themeName is string && themeName.Equals(obj: "Default"))
                    {
                        GuiCommand.SetTheme(theme: new DarkHumanEditableTheme(),
                            updateExisting: true,
                            instance: guiCommand);
                        guiCommand.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                            "Dark";
                    }
                    else
                    {
                        GuiCommand.SetTheme(theme: new DefaultHumanEditableTheme(),
                            updateExisting: true,
                            instance: guiCommand);
                        guiCommand.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                            "Default";
                    }
                }),
            new StatusItem(shortcut: Key.Null,
                title: Constants.ProgramName /* Application.Driver.GetType().Name */,
                action: null),
        };
    }
}