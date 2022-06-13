using PhantomKit.Models;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomMail.CLI.Commands;
using Terminal.Gui;

namespace PhantomMail.CLI.StatusBars;

// ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
public class PhantomMailStatusBar : PhantomKitStatusBar
{
    // ReSharper disable once SuggestBaseTypeForParameterInConstructor
    public PhantomMailStatusBar(PhantomMailGuiCommand guiCommand) : base(guiCommand: guiCommand)
    {
        this.Items = this.BuildItems();
    }

    private new PhantomMailGuiCommand GuiCommand => (PhantomMailGuiCommand)base.GuiCommand;


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
            this.TitleItem,
        };
    }

    protected virtual void VaultSaveClick()
    {
        if (!this.GuiCommand.VaultLoaded) return;
        this.GuiCommand.SaveVaultWithChangePrompt(vaultFileName: this.GuiCommand.VaultFile!);
    }

    protected virtual void VaultLoadClick()
    {
        this.GuiCommand.SelectAndLoadOrCreateVaultWithKeyPrompt();
    }

    public override void LightDarkClick()
    {
        base.LightDarkClick();
        if (this.GuiCommand.VaultLoaded)
            this.GuiCommand.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                this.GuiCommand.DarkMode ? DarkHumanEditableTheme.Name : BlueHumanEditableTheme.Name;
    }
}