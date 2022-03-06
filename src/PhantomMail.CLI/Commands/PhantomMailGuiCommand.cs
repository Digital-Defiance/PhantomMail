using System.Security;
using MailKit;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using NStack;
using PhantomKit.Exceptions;
using PhantomKit.Helpers;
using PhantomKit.Models;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Settings;
using PhantomMail.CLI.Dialogs;
using PhantomMail.CLI.Menus;
using PhantomMail.CLI.StatusBars;
using PhantomMail.Windows;
using Serilog;
using Terminal.Gui;

namespace PhantomMail.CLI.Commands;

[HelpOption(template: "--help")]
// ReSharper disable once ClassNeverInstantiated.Global
// ReSharper is not smart enough to know this is being executed by PhantomMail.CLI lines 65-69 <see cref="PhantomMail.CLI:66"/>
public class PhantomMailGuiCommand : HostedGuiCommandBase
{
    private const bool MouseEnabledValue = false;
    private readonly Dictionary<Guid, IMailService> _connectedAccounts = new();
    private readonly FileDialog _fileDialog;


    // private new readonly PhantomMailStatusBar StatusBar;
    private EncryptableSettingsVault? _vault;
    private string? _vaultFile = Utilities.GetDefaultVaultFile();

    public PhantomMailGuiCommand()
    {
        this.Window = new PhantomKitWindow(guiCommand: this);
        this.StatusBar = new PhantomMailStatusBar(guiCommand: this);
        this.Menu = new PhantomMailMainMenu(guiCommand: this);

        /*
        // keep a reference to the settings vault
        this.SettingsVault = AppLoadVaultOrNewVault();
        */

        this._fileDialog = new FileDialog(
            title: "Select vault",
            prompt: "Open",
            nameFieldLabel: "File",
            message: "Select a vault json file to open",
            allowedTypes: new List<string> {".json"});
        this._fileDialog.CanCreateDirectories = true;
        this._fileDialog.AllowsOtherFileTypes = false;
        // default to the preferred location/file
        this._fileDialog.DirectoryPath =
            ustring.Make(str: Path.GetDirectoryName(path: Utilities.GetDefaultVaultFile()));
        this._fileDialog.FilePath = ustring.Make(str: Path.GetFileName(path: Utilities.GetDefaultVaultFile()));
    }

    public PhantomMailGuiCommand(ILogger logger, IConfiguration configuration, IConsole console)
    {
        this.Window = new PhantomKitWindow(guiCommand: this);
        this.StatusBar = new PhantomMailStatusBar(guiCommand: this);
        this.Menu = new PhantomMailMainMenu(guiCommand: this);

        /*
        // keep a reference to the settings vault
        this.SettingsVault = AppLoadVaultOrNewVault();
        */

        this._fileDialog = new FileDialog(
            title: "Select vault",
            prompt: "Open",
            nameFieldLabel: "File",
            message: "Select a vault json file to open",
            allowedTypes: new List<string> {".json"});
        this._fileDialog.CanCreateDirectories = true;
        this._fileDialog.AllowsOtherFileTypes = false;
        // default to the preferred location/file
        this._fileDialog.DirectoryPath =
            ustring.Make(str: Path.GetDirectoryName(path: Utilities.GetDefaultVaultFile()));
        this._fileDialog.FilePath = ustring.Make(str: Path.GetFileName(path: Utilities.GetDefaultVaultFile()));
    }

    [Option(optionType: CommandOptionType.SingleValue,
        ShortName = "v",
        LongName = "vault",
        Description = "vault file",
        ValueName = "vault file",
        ShowInHelpText = true)]
    public string? VaultFile
    {
        get => this._vaultFile;
        set
        {
            if (this._vault is not null) throw new VaultAlreadyLoadedException();
            this._vaultFile = value;
        }
    }

    public EncryptableSettingsVault SettingsVault => this._vault ?? throw new VaultNotLoadedException();
    public bool VaultLoaded => this._vault is not null;

    public EncryptableSettingsVault SettingsVaultCopy => new(other: this.SettingsVault);

    /// <summary>
    ///     Virtual list of mail accounts extracted from EncryptedSettings
    /// </summary>
    // Naming is on purpose as we use nameof(MailAccounts) and want our key to be "MailAccounts"
    // ReSharper disable once InconsistentNaming
    private IEnumerable<EncryptedMailAccount> MailAccounts => this.SettingsVault is not null
        ? this.SettingsVault.MailAccounts(vaultKey: this.SettingsVault.VaultKey)
        : throw new VaultNotLoadedException();

    public override bool MouseEnabled { get; init; } = MouseEnabledValue;

    public override void Dispose()
    {
        foreach (var (guid, connection) in this._connectedAccounts)
            this.DisconnectAccount(accountId: guid,
                quit: true,
                connectedAccount: connection);
        this.SettingsVault.Dispose();
        this._fileDialog?.Dispose();
        //guiCommand.SuspendUi();
        // guiCommand.Running = false;

        this.Window?.Dispose();
        this.Menu?.Dispose();
    }

    public void LoadSavedTheme()
    {
        if (this.VaultLoaded)
        {
            var theme = this._vault!.Theme;
            GuiUtilities.SetColorsFromTheme(
                theme: theme);
            this.UpdateTheme(theme: theme);
        }
    }

    private void ShowFileDialog()
    {
        var top = GuiUtilities.GetNewTopLevel(clearExistingTop: Application.Top);
        top.Add(view: this._fileDialog);
        Application.Run(view: top);
        top.Remove(view: this._fileDialog);
    }

    private SecureString? ShowVaultPrompt(bool verify)
    {
        var top = GuiUtilities.GetNewTopLevel(clearExistingTop: Application.Top);
        var vaultPrompt = new VaultPromptDialog(withVerify: verify);
        top.Add(view: vaultPrompt);
        Application.Run(view: top);
        top.Remove(view: vaultPrompt);
        return vaultPrompt.VaultKeySecureString;
    }

    public void SelectAndLoadOrCreateVaultWithKeyPrompt()
    {
        this.ShowFileDialog();

        if (this._fileDialog.Canceled)
            return;

        var vaultFile = this._fileDialog.FilePath;
        if (vaultFile is null || vaultFile.IsEmpty)
            return;

        var vaultFileName = vaultFile.ToString();
        var newFile = !File.Exists(path: vaultFileName);
        if (newFile)
        {
            var n = MessageBox.Query(
                width: 50,
                height: 7,
                title: ustring.Make(str: "Create new Vault?"),
                message: ustring.Make(str: $"The vault {vaultFileName} does not exist. Create it?"),
                "Yes",
                "No");
            if (n != 0) return;
        }

        this._vaultFile = vaultFileName;

        using var vaultKey = this.ShowVaultPrompt(verify: newFile);

        // canceled?
        if (vaultKey is null)
            return;

        if (newFile)
        {
            try
            {
                this.CreateVault(
                    vaultKey: vaultKey,
                    replaceLoadedVault: false,
                    overwriteExisting: false);


                MessageBox.Query(
                    width: 50,
                    height: 7,
                    title: ustring.Make(str: "Vault created"),
                    message: ustring.Make(str: "Vault created successfully"),
                    "Ok");
            }
            catch (Exception ex)
            {
                MessageBox.Query(
                    width: 50,
                    height: 7,
                    title: ustring.Make(str: "Vault creation failed"),
                    message: ustring.Make(str: $"Vault creation failed: {ex.Message}. Please try again."),
                    "Ok");
            }
        }
        else
        {
            if (!this.LoadVaultWithChangePrompt(vaultKey: vaultKey))
            {
                MessageBox.ErrorQuery(
                    width: 50,
                    height: 7,
                    title: ustring.Make(str: "Unable to load vault"),
                    message: ustring.Make(str: "Unable to load vault. Please check your key and try again."),
                    "Ok");
                return;
            }

            MessageBox.Query(
                width: 50,
                height: 7,
                title: ustring.Make(str: "Vault loaded"),
                message: ustring.Make(str: "Vault loaded successfully"),
                "Ok");
        }
    }

    public void LoadVault(SecureString vaultKey, bool replaceLoadedVault)
    {
        if (!replaceLoadedVault && this._vault is not null)
            throw new VaultAlreadyLoadedException();

        if (this._vaultFile is null || string.IsNullOrEmpty(value: this._vaultFile))
            throw new InvalidOperationException(message: "Settings file not set");

        if (!File.Exists(path: this._vaultFile))
            throw new VaultFileNotFoundException(fileName: this._vaultFile);

        this._vault = EncryptableSettingsVault.Load(vaultKey: vaultKey,
            fileName: this._vaultFile);

        this.StatusBar.SetStatus(title: "[vault loaded]");

        this.LoadSavedTheme();
    }

    public void CreateVault(SecureString vaultKey, bool replaceLoadedVault, bool overwriteExisting)
    {
        if (!replaceLoadedVault && this._vault is not null)
            throw new VaultAlreadyLoadedException();

        if (this._vaultFile is null || string.IsNullOrEmpty(value: this._vaultFile))
            throw new InvalidOperationException(message: "Settings file not set");

        if (File.Exists(path: this._vaultFile) && !overwriteExisting)
            throw new InvalidOperationException(
                message: $"Settings file already exists and overwrite false: {this._vaultFile}");

        this._vault = new EncryptableSettingsVault(vaultKey: vaultKey);
        this.StatusBar.SetStatus(title: "[new vault created]");
        this._vault.Save(fileName: this._vaultFile);
    }

    public void CloseVault(bool saveIfChanged, bool cancelIfChanged = false)
    {
        if (this._vault is null)
            throw new VaultNotLoadedException();

        if (this._vault.HasChanged && !saveIfChanged && cancelIfChanged)
            throw new VaultChangedException();

        if (this._vault.HasChanged && saveIfChanged)
            this._vault.Save();
    }

    /*public static EncryptableSettingsVault AppLoadNewVault(string settingsFilePath, out SecureString vaultKey)
    {
        vaultKey = VaultPromptToSecurePassword(isNew: true);
        // vault does not keep the key by default
        var vault = new EncryptableSettingsVault(vaultKey: vaultKey);
        try
        {
            vault.Save(fileName: settingsFilePath);
            return vault;
        }
        catch (Exception e)
        {
            throw new VaultNotLoadedException(innerException: e);
        }
    }*/

    public bool SaveToVaultFile(string fileName, bool overwrite = false)
    {
        if (this.SettingsVault is null) throw new VaultNotLoadedException();

        if (!overwrite && Directory.Exists(path: Path.GetDirectoryName(path: fileName)) && File.Exists(
                path: fileName))
            return false;

        this.SettingsVault.Save(fileName: fileName);

        return true;
    }

    public void SaveVaultWithChangePrompt(string vaultFileName)
    {
        if (this.SettingsVault is null)
            throw new VaultNotLoadedException();

        var q = MessageBox.Query(
            width: 50,
            height: 7,
            title: "Save",
            message: !this.SettingsVault.HasChanged ? "No changes detected. Save anyway?" : "Save changes?",
            "Yes",
            "Cancel");
        if (q != 0) return;

        if (!this.SaveToVaultFile(
                fileName: vaultFileName,
                overwrite: true))
        {
            var errorQuery = MessageBox.ErrorQuery(
                width: 50,
                height: 7,
                title: "Error Saving",
                message: "Unable to save vault to " + vaultFileName,
                "Ok");
        }
    }

    public bool LoadVaultWithChangePrompt(SecureString vaultKey)
    {
        if (this.VaultLoaded)
        {
            var q = MessageBox.Query(
                width: 50,
                height: 7,
                title: "Load Vault",
                message: this.SettingsVault.HasChanged
                    ? "Changes detected. Load without saving?"
                    : "Load and replace existing (saved) vault?",
                "Yes",
                "Cancel");
            if (q != 0) return false;
        }

        try
        {
            this.LoadVault(
                vaultKey: vaultKey,
                replaceLoadedVault: true);
            return true;
        }
        catch (PhantomKitException phantomKitException)
        {
            this.Logger.Information(exception: phantomKitException,
                messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        }
        catch (Exception e)
        {
            this.Logger.Error(exception: e,
                messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] UNEXPECTED: {Message:lj}{NewLine}{Exception}");
        }

        MessageBox.ErrorQuery(
            width: 50,
            height: 7,
            title: "Error Saving",
            message: "Unable to load vault from " + this._vaultFile,
            "Ok");
        return false;
    }

    /// <summary>
    /// </summary>
    /// <param name="newAccount"></param>
    /// <returns></returns>
    /// <exception cref="AccountExistsException"></exception>
    public void AddAccount(EncryptedMailAccount newAccount)
    {
        if (this.SettingsVault is null)
            throw new VaultNotLoadedException();

        var vaultKey = this.SettingsVault.VaultKey;
        if (vaultKey is null)
            throw new VaultKeyNotSetException();

        var existingAccount = this.MailAccounts.Select(selector: mailAccount => mailAccount.Id == newAccount.Id)
            .ToArray();
        if (existingAccount.Any()) throw new AccountExistsException(encryptedMailAccount: newAccount);

        // re-create the mail-accounts object with the new account and re-encrypt
        // explicitly create as encrypted object rather than going through the setter's default condition, for readability
        this.SettingsVault[key: nameof(this.MailAccounts)] = EncryptedObjectSetting.CreateEncrypted(
            vaultKey: vaultKey,
            value: new MailAccounts(accounts: this.MailAccounts.Append(element: newAccount)));
    }

    /// <summary>
    ///     Remove an account from the list of mail accounts
    /// </summary>
    /// <param name="account">Account to remove</param>
    /// <returns></returns>
    /// <exception cref="AccountNotFoundException"></exception>
    public void RemoveAccount(EncryptedMailAccount account)
    {
        if (this.SettingsVault is null)
            throw new VaultNotLoadedException();

        var vaultKey = this.SettingsVault.VaultKey;
        if (vaultKey is null)
            throw new VaultKeyNotSetException();

        var existingAccount = this.MailAccounts.Select(selector: mailAccount => mailAccount.Id == account.Id)
            .ToArray();
        if (!existingAccount.Any()) throw new AccountNotFoundException(encryptedMailAccount: account);

        // re-create the mail-accounts object with the account removed and re-encrypt
        // explicitly create as encrypted object rather than going through the setter's default condition, for readability
        this.SettingsVault[key: nameof(this.MailAccounts)] = EncryptedObjectSetting.CreateEncrypted(
            vaultKey: vaultKey,
            value: new MailAccounts(
                accounts: this.MailAccounts.Where(predicate: mailAccount => mailAccount.Id != account.Id)));
    }

    /// <summary>
    ///     Make a connection to the specified account
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public IMailService ConnectAccount(EncryptedMailAccount account)
    {
        if (this.SettingsVault is null)
            throw new VaultNotLoadedException();

        var vaultKey = this.SettingsVault.VaultKey;
        if (vaultKey is null)
            throw new VaultKeyNotSetException();

        if (this._connectedAccounts.ContainsKey(key: account.Id)) return this._connectedAccounts[key: account.Id];
        var mailService = MailUtilities.ConnectMailService(unlockedMailAccount: account.Unlock(vaultKey: vaultKey));
        this._connectedAccounts.Add(key: account.Id,
            value: mailService);
        return mailService;
    }

    /// <summary>
    ///     Returns whether the specified account is connected
    /// </summary>
    /// <param name="account"></param>
    /// <returns></returns>
    public bool AccountConnected(EncryptedMailAccount account)
    {
        return this._connectedAccounts.ContainsKey(key: account.Id);
    }

    /// <summary>
    ///     Disconnect the specified account and returns whether it was connected
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="quit"></param>
    /// <param name="connectedAccount"></param>
    /// <returns>A boolean indicating whether the account was connected previously</returns>
    public bool DisconnectAccount(Guid accountId, bool quit, IMailService? connectedAccount = null)
    {
        if (!this._connectedAccounts.ContainsKey(key: accountId))
            return false;

        var connection = this._connectedAccounts[key: accountId];

        if (connectedAccount is not null)
        {
            if (connectedAccount.IsConnected)
                connectedAccount.Disconnect(quit: quit);

            if (quit)
                connectedAccount.Dispose();
        }

        if (connection.IsConnected)
            connection.Disconnect(quit: quit);

        if (quit)
            connection.Dispose();
        else
            this._connectedAccounts.Remove(key: accountId);

        return true;
    }

    /*public void LostCode()
    {
        try
        {
            this.Vault = EncryptableSettingsVault.Load(
                vaultKey: this.VaultKeySecureString!,
                fileName: this.SettingsFile);
            this.VaultPasswordCorrect = true;
            this.Resolution = ResolutionType.OkPasswordCorrect;
            return true;
        }
        catch (VaultKeyIncorrectException vaultKeyIncorrectException)
        {
            this.Resolution = ResolutionType.OkPasswordIncorrect;
            this.VaultPasswordCorrect = false;
            return false;
        }
        catch (Exception e)
        {
            this.Resolution = ResolutionType.OkInvalid;
            return false;
        }
        
        this._vaultPrompt = new VaultPrompt(
            settingsFile: this._settingsFile,
            height: 40,
            width: 200,
            border: new Border
            {
                BorderThickness = new Thickness {Left = 1, Right = 1, Top = 1, Bottom = 1},
            });
    }*/
}