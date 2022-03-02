using PhantomKit.Helpers;
using PhantomKit.Interfaces;
using PhantomKit.Models;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomMail.CLI.Commands;
using Terminal.Gui;

namespace PhantomMail.CLI.StatusBars;

public class PhantomMailStatusBar : PhantomKitStatusBar
{
    private readonly PhantomMailGuiCommand _command;

    public PhantomMailStatusBar(Type guiCommandType) : base(guiCommandType: guiCommandType)
    {
        this._command =
            (IGuiCommand.GetInstance(guiCommandType: typeof(PhantomMailGuiCommand)) as PhantomMailGuiCommand)!;
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
                action: this.VaultLoadClick),
            new StatusItem(shortcut: Key.F3,
                title: "~F3~ Save",
                action: this.VaultSaveClick),
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

    public void VaultSaveClick()
    {
        if (!this._command.VaultLoaded) return;
        this._command.SaveVaultWithChangePrompt(vaultFileName: this._command.VaultFile!);
    }

    public void VaultLoadClick()
    {
        this._command.SelectAndLoadOrCreateVaultWithKeyPrompt();
    }

    public override void LightDarkClick()
    {
        base.LightDarkClick();
        if (this._command.VaultLoaded)
            this._command.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                this._command.DarkMode ? DarkHumanEditableTheme.Name : BlueHumanEditableTheme.Name;
    }
}