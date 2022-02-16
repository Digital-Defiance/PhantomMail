using System.Diagnostics;
using System.Globalization;
using System.Security;
using MailKit;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using NStack;
using PhantomKit.Exceptions;
using PhantomKit.Helpers;
using PhantomKit.Interfaces;
using PhantomKit.Models;
using PhantomKit.Models.Commands;
using PhantomKit.Models.Settings;
using PhantomKit.Models.Themes;
using PhantomMail.CLI.Dialogs;
using PhantomMail.CLI.StatusBars;
using PhantomMail.Menus;
using PhantomMail.Windows;
using Serilog;
using Terminal.Gui;

namespace PhantomMail.CLI.Commands;

[HelpOption(template: "--help")]
public class PhantomMailGuiCommand : GuiCommand, IGuiCommand
{
    private static PhantomMailGuiCommand? _singleton;

    private readonly Dictionary<Guid, IMailService> _connectedAccounts = new();
    private readonly FileDialog _fileDialog;
    private readonly VaultPromptDialog _vaultPromptDialog;
    private EncryptableSettingsVault? _vault;
    private string? _vaultFile = Utilities.GetDefaultVaultFile();

    public PhantomMailGuiCommand(ILogger logger, IConfiguration configuration, IConsole console) : base(logger: logger,
        configuration: configuration,
        console: console)
    {
        if (_singleton != null)
            throw new InvalidOperationException(message: "Only one instance of GuiCommand can be created");

        _singleton = this;

        this._vaultPromptDialog = new VaultPromptDialog();

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

        Application.Top.Remove(view: this.StatusBar);
        this.StatusBar = new PhantomMailStatusBar(guiCommand: this);
        Application.Top.Add(view: this.StatusBar);
    }

    public new static PhantomMailGuiCommand Singleton
        => _singleton ?? throw new InvalidOperationException(message: "GuiCommand has not been initialized");


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

    public override int OnExecute(CommandLineApplication app)
    {
        if (Debugger.IsAttached)
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(name: "en-US");

        Application.Init();
        Application.HeightAsBuffer = true;
        SetTheme(
            theme: HumanEditableTheme.Themes.Dark,
            instance: this);

        var newTop = Application.Top;
        newTop.Clear();
        this.Menu = new PhantomKitMainMenu();
        newTop.Add(view: this.Menu);
        newTop.Add(view: new PhantomMailStatusBar(guiCommand: this));
        var win = new PhantomKitWindow();
        win.KeyPress += Win_KeyPress;
        newTop.Add(view: win);

        this.SelectAndLoadOrCreateVaultWithKeyPrompt(destination: newTop);

        Application.Run(view: newTop);
        return 0;
    }

    public new void Dispose()
    {
        foreach (var (guid, connection) in this._connectedAccounts)
            this.DisconnectAccount(accountId: guid,
                quit: true);
        this.SettingsVault.Dispose();
        base.Dispose();
    }

    public void LoadSavedTheme()
    {
        SetTheme(
            theme: this._vault!.Theme,
            instance: this);
    }

    private void ShowFileDialog(Toplevel newTop)
    {
        newTop.Add(view: this._fileDialog);
        Application.Run(view: newTop);
        newTop.Remove(view: this._fileDialog);
    }

    private void ShowVaultPrompt(Toplevel newTop, bool verify)
    {
        if (this._vaultPromptDialog.VaultKeySecureString is { } secureString) return;
        this._vaultPromptDialog.SetPasswordLabel(verify: verify);
        newTop.Add(view: this._vaultPromptDialog);
        Application.Run(view: newTop);
        newTop.Remove(view: this._vaultPromptDialog);
    }

    public void SelectAndLoadOrCreateVaultWithKeyPrompt(Toplevel destination)
    {
        this.ShowFileDialog(newTop: destination);

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

        this.ShowVaultPrompt(newTop: destination,
            verify: false);

        using var vaultKey = this._vaultPromptDialog.VaultKeySecureString;
        // canceled?
        if (vaultKey is null)
            return;

        if (newFile)
        {
            // verify the vault key
            this.ShowVaultPrompt(newTop: destination,
                verify: true);

            using var vaultKeyVerify = this._vaultPromptDialog.VaultKeySecureString;
            if (vaultKeyVerify is null)
                return;

            // compare the two secure strings
            if (!vaultKey.SecureStringEquals(s2: vaultKeyVerify))
            {
                MessageBox.Query(
                    width: 50,
                    height: 7,
                    title: ustring.Make(str: "Vault key mismatch"),
                    message: ustring.Make(str: "Vault key and verification do not match. Please try again."),
                    "Ok");
                return;
            }

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
        var q = MessageBox.Query(
            width: 50,
            height: 7,
            title: "Load",
            message: this.SettingsVault.HasChanged ? "Changes detected. Load without saving?" : "Load",
            "Yes",
            "Cancel");
        if (q != 0) return false;

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

        var errorQuery = MessageBox.ErrorQuery(
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
    /// <returns>A boolean indicating whether the account was connected previously</returns>
    public bool DisconnectAccount(Guid accountId, bool quit)
    {
        if (!this._connectedAccounts.ContainsKey(key: accountId))
            return false;
        var connection = this._connectedAccounts[key: accountId];
        connection.Disconnect(quit: true);
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