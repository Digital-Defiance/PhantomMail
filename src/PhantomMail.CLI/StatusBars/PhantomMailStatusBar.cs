using PhantomKit.Helpers;
using PhantomKit.Models;
using PhantomKit.Models.Commands;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomMail.CLI.Commands;
using Terminal.Gui;

namespace PhantomMail.CLI.StatusBars;

public class PhantomMailStatusBar : PhantomKitStatusBar
{
    public PhantomMailStatusBar(PhantomMailGuiCommand guiCommand) : base(guiCommand: guiCommand)
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
                action: () => guiCommand.SelectAndLoadOrCreateVaultWithKeyPrompt(
                    destination: guiCommand.RebuildWindow())),
            new StatusItem(shortcut: Key.F3,
                title: "~F3~ Save",
                action: () =>
                {
                    if (!guiCommand.VaultLoaded) return;
                    guiCommand.SaveVaultWithChangePrompt(vaultFileName: guiCommand.VaultFile!);
                }),
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
                    {
                        GuiCommand.SetTheme(theme: HumanEditableTheme.Themes.Dark,
                            instance: guiCommand);
                        if (guiCommand.VaultLoaded)
                            guiCommand.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                                DarkHumanEditableTheme.Name;
                    }
                    else
                    {
                        GuiCommand.SetTheme(theme: HumanEditableTheme.Themes.Blue,
                            instance: guiCommand);
                        if (guiCommand.VaultLoaded)
                            guiCommand.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                                BlueHumanEditableTheme.Name;
                    }

                    guiCommand.DarkMode = !guiCommand.DarkMode;
                }),
            new StatusItem(shortcut: Key.Null,
                title: Constants.ProgramName /* Application.Driver.GetType().Name */,
                action: null),
        };
    }
}