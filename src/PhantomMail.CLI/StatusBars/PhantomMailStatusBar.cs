using PhantomKit.Helpers;
using PhantomKit.Models;
using Terminal.Gui;

namespace PhantomMail.StatusBars;

public class PhantomMailStatusBar : StatusBar
{
    public PhantomMailStatusBar(GuiContext guiContext)
    {
        guiContext = guiContext;
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
                action: guiContext.LoadVault),
            new StatusItem(shortcut: Key.F3,
                title: "~F3~ Save",
                action: guiContext.SaveVault),
            new StatusItem(shortcut: Key.CtrlMask | Key.Q,
                title: "~^Q~ Quit",
                action: () =>
                {
                    if (GuiContext.Quit() && Application.Top is not null) Application.Top.Running = false;
                }),
            new StatusItem(shortcut: Key.F10,
                title: "~F10~ Light/Dark",
                action: () =>
                {
                    var themeName =
                        guiContext.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)];
                    if (themeName is string && themeName.Equals(obj: "Default"))
                    {
                        GuiContext.SetTheme(theme: new DarkHumanEditableTheme(),
                            updateExisting: true,
                            instance: guiContext);
                        guiContext.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                            "Dark";
                    }
                    else
                    {
                        GuiContext.SetTheme(theme: new DefaultHumanEditableTheme(),
                            updateExisting: true,
                            instance: guiContext);
                        guiContext.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                            "Default";
                    }
                }),
            new StatusItem(shortcut: Key.Null,
                title: Constants.ProgramName /* Application.Driver.GetType().Name */,
                action: null),
        };
    }
}