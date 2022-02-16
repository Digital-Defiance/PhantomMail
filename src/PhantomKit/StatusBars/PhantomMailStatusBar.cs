using PhantomKit.Exceptions;
using PhantomKit.Helpers;
using PhantomKit.Menus;
using PhantomKit.Models;
using Terminal.Gui;

namespace PhantomKit.StatusBars;

public class PhantomMailStatusBar : StatusBar
{
    private readonly GuiContext _guiContext;

    public PhantomMailStatusBar(GuiContext guiContext)
    {
        this._guiContext = guiContext;
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
                action: this.LoadVault),
            new StatusItem(shortcut: Key.F3,
                title: "~F3~ Save",
                action: this.SaveVault),
            new StatusItem(shortcut: Key.CtrlMask | Key.Q,
                title: "~^Q~ Quit",
                action: () =>
                {
                    if (PhantomMailMainMenu.Quit() && Application.Top is not null) Application.Top.Running = false;
                }),
            new StatusItem(shortcut: Key.F10,
                title: "~F10~ Light/Dark",
                action: () =>
                {
                    var themeName =
                        guiContext.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)];
                    if (themeName is string && themeName.Equals(obj: "Default"))
                    {
                        guiContext.SetTheme(theme: new DarkHumanEditableTheme());
                        guiContext.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                            "Dark";
                    }
                    else
                    {
                        guiContext.SetTheme(theme: new DefaultHumanEditableTheme());
                        guiContext.SettingsVault.EncryptableSettings[key: nameof(EncryptableSettingsVault.Theme)] =
                            "Default";
                    }
                }),
            new StatusItem(shortcut: Key.Null,
                title: Constants.ProgramName /* Application.Driver.GetType().Name */,
                action: null),
        };
    }


    private void SaveVault()
    {
        if (this._guiContext.SettingsVault is null)
            throw new VaultNotLoadedException();

        var q = MessageBox.Query(
            width: 50,
            height: 7,
            title: "Save",
            message: !this._guiContext.SettingsVault.HasChanged ? "No changes detected. Save anyway?" : "Save changes?",
            "Yes",
            "Cancel");
        if (q != 0) return;

        var settingsFileName = Utilities.SettingsFile();
        if (!this.SaveToVaultFile(
                settingsFilePath: settingsFileName,
                overwrite: true))
        {
            var errorQuery = MessageBox.ErrorQuery(
                width: 50,
                height: 7,
                title: "Error Saving",
                message: "Unable to save settings to " + settingsFileName,
                "Ok");
        }
    }

    public bool SaveToVaultFile(string settingsFilePath, bool overwrite = false)
    {
        if (this._guiContext.SettingsVault is null) throw new VaultNotLoadedException();

        if (!overwrite && Directory.Exists(path: Path.GetDirectoryName(path: settingsFilePath)) && File.Exists(
                path: settingsFilePath))
            return false;

        this._guiContext.SettingsVault.Save(fileName: settingsFilePath);

        return true;
    }


    private void LoadVault()
    {
        var q = MessageBox.Query(
            width: 50,
            height: 7,
            title: "Load",
            message: this._guiContext.SettingsVault.HasChanged ? "Changes detected. Load without saving?" : "Load",
            "Yes",
            "Cancel");
        if (q != 0) return;

        var settingsFileName = Utilities.SettingsFile();
        if (!this.SaveToVaultFile(
                settingsFilePath: settingsFileName,
                overwrite: true))
        {
            var errorQuery = MessageBox.ErrorQuery(
                width: 50,
                height: 7,
                title: "Error Saving",
                message: "Unable to save settings to " + settingsFileName,
                "Ok");
        }

        MessageBox.Query(
            width: 50,
            height: 7,
            title: "Load",
            message: "Loading",
            "Ok");
    }
}