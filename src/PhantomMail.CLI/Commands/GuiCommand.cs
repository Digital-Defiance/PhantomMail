using System.Diagnostics;
using System.Globalization;
using System.Security;
using MailKit;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using NStack;
using PhantomKit.Exceptions;
using PhantomKit.Helpers;
using PhantomKit.Models;
using PhantomMail.Menus;
using PhantomMail.StatusBars;
using PhantomMail.Views;
using PhantomMail.Windows;
using Terminal.Gui;

namespace PhantomMail;

[HelpOption(template: "--help")]
public class GuiCommand
{
    private static GuiCommand? _singleton;

    private readonly IConfiguration _configuration;

    // ---- PASTED
    private readonly Dictionary<Guid, IMailService> _connectedAccounts = new();
    private readonly IConsole _console;
    private readonly bool _isBox10X = true;
    private readonly Serilog.ILogger _logger;
    private readonly ScrollView _scrollView;
    private readonly VaultPrompt _vaultPrompt;
    private readonly Window _window;
    private string? _settingsFile;

    private EncryptableSettingsVault _vault;
    public MenuBar Menu;
    public CheckBox MenuAutoMouseNav;
    public CheckBox MenuKeysStyle;
    public Label Ml;
    public Label Ml2;

    public GuiCommand(Serilog.ILogger logger, IConfiguration configuration, IConsole console)
    {
        if (_singleton != null)
            throw new InvalidOperationException(message: "Only one instance of GuiCommand can be created");

        _singleton = this;
        this._logger = logger;
        this._configuration = configuration;
        this._console = console;
        this._vaultPrompt = new VaultPrompt(
            height: 40,
            width: 400,
            border: new Border {BorderThickness = new Thickness {Top = 1, Bottom = 1, Left = 1, Right = 1}});

        Application.Init();

        /*
        // keep a reference to the settings vault
        this.SettingsVault = AppLoadVaultOrNewVault();

        this.SetTheme(theme: this.SettingsVault.Theme);
        */
        // set the color scheme/defaults for new gui elements created
        SetTheme(theme: HumanEditableTheme.Dark,
            updateExisting: true,
            instance: this);

        // create the window
        this._window = new PhantomMailWindow();
        this.Menu = new PhantomMailMainMenu();

        this._scrollView = new ScrollView(frame: new Rect(x: 50,
            y: 10,
            width: 20,
            height: 8))
        {
            ContentSize = new Size(width: 100,
                height: 100),
            ContentOffset = new Point(x: -1,
                y: -1),
            ShowVerticalScrollIndicator = true,
            ShowHorizontalScrollIndicator = true,
        };

        this.MenuKeysStyle = new CheckBox(x: 3,
            y: 25,
            s: "UseKeysUpDownAsKeysLeftRight",
            is_checked: true);
        this.MenuKeysStyle.Toggled += MenuKeysStyle_Toggled;
        this.MenuAutoMouseNav = new CheckBox(x: 40,
            y: 25,
            s: "UseMenuAutoNavigation",
            is_checked: true);
        this.MenuAutoMouseNav.Toggled += MenuAutoMouseNav_Toggled;

        var count = 0;
        this.Ml = new Label(rect: new Rect(x: 3,
                y: 17,
                width: 47,
                height: 1),
            text: "Mouse: ");
        Application.RootMouseEvent += me => this.Ml.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags} {count++}";

        this._window.Add(view: this.Ml);

        Application.Top.Add(this._window,
            this.Menu,
            new PhantomMailStatusBar(guiCommand: this));
    }

    public static GuiCommand Singleton
        => _singleton ?? throw new InvalidOperationException(message: "GuiCommand has not been initialized");


    [Option(optionType: CommandOptionType.SingleValue,
        ShortName = "v",
        LongName = "vault",
        Description = "vault file",
        ValueName = "vault file",
        ShowInHelpText = true)]
    public string VaultFile { get; set; } = Utilities.GetSettingsFile();

    public string? SettingsFile
    {
        get => this._settingsFile;
        set
        {
            if (this._vault is not null) throw new VaultAlreadyLoadedException();

            this._settingsFile = value;
        }
    }

    public EncryptableSettingsVault SettingsVault => this._vault ?? throw new VaultNotLoadedException();

    public EncryptableSettingsVault SettingsVaultCopy => new(other: this.SettingsVault);

    /// <summary>
    ///     Virtual list of mail accounts extracted from EncryptedSettings
    /// </summary>
    // Naming is on purpose as we use nameof(MailAccounts) and want our key to be "MailAccounts"
    // ReSharper disable once InconsistentNaming
    private IEnumerable<EncryptedMailAccount> MailAccounts => this.SettingsVault is not null
        ? this.SettingsVault.MailAccounts(vaultKey: this.SettingsVault.VaultKey)
        : throw new VaultNotLoadedException();

    protected virtual async Task<int> OnExecute(CommandLineApplication app)
    {
        if (Debugger.IsAttached)
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(name: "en-US");

        Application.Init();
        Application.HeightAsBuffer = true;

        var newTop = Application.Top;
        newTop.Clear();
        this.Menu = new PhantomMailMainMenu();
        newTop.Add(view: this.Menu);

        var win = new PhantomMailWindow();
        var vaultFileDialog = new FileDialog(
            title: "Select vault",
            prompt: "Open",
            nameFieldLabel: "File",
            message: "Select a vault json file to open",
            allowedTypes: new List<string> {".json"});
        vaultFileDialog.DirectoryPath = ustring.Make(str: Path.GetDirectoryName(path: Utilities.GetSettingsFile()));
        vaultFileDialog.FilePath = ustring.Make(str: Path.GetFileName(path: Utilities.GetSettingsFile()));

        newTop.Add(view: vaultFileDialog);
        // run the vault file selection dialog
        Application.Run(view: newTop);

        newTop.Remove(view: vaultFileDialog);

        var vaultFile = vaultFileDialog.FilePath;
        if (vaultFileDialog.Canceled || vaultFile is null || vaultFile.IsEmpty) return -1;

        var newFile = !File.Exists(path: vaultFile.ToString());
        if (!newFile)
            // TODO: prompt to create
            throw new NotImplementedException();

        win.KeyPress += Win_KeyPress;
        newTop.Add(view: new PhantomMailStatusBar(guiCommand: this));
        await this.ShowVaultPrompt(newTop: newTop);
        newTop.Add(view: win);

        /*
var text = new TextView
{
    X = 0, Y = 0, Width = Dim.Fill(),
    Height = Dim.Fill() - 2, // -2 for the menu bar & status bar?
};
text.Text = "TODO: emails and such";
win.Add(view: text);
*/

        Application.Run(view: newTop);
        return 0;
    }

    protected void OnException(Exception ex)
    {
        this.OutputError(message: ex.Message);
        // ReSharper disable StructuredMessageTemplateProblem
        this._logger.Error(exception: ex, messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        this._logger.Debug(exception: ex, messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        // ReSharper restore StructuredMessageTemplateProblem
    }

    protected void OutputToConsole(string data)
    {
        this._console.BackgroundColor = ConsoleColor.Black;
        this._console.ForegroundColor = ConsoleColor.White;
        this._console.Out.Write(value: data);
        this._console.ResetColor();
    }

    protected void OutputError(string message)
    {
        this._console.BackgroundColor = ConsoleColor.Red;
        this._console.ForegroundColor = ConsoleColor.White;
        this._console.Error.WriteLine(value: message);
        this._console.ResetColor();
    }

    public static void Win_KeyPress(View.KeyEventEventArgs e)
    {
        var guiCommand = Singleton;
        switch (ShortcutHelper.GetModifiersKey(kb: e.KeyEvent))
        {
            case Key.CtrlMask | Key.T:
                if (guiCommand.Menu.IsMenuOpen)
                    guiCommand.Menu.CloseMenu();
                else
                    guiCommand.Menu.OpenMenu();
                e.Handled = true;
                break;
        }
    }

    public void LoadVault(SecureString vaultKey, bool replaceLoadedVault)
    {
        if (!replaceLoadedVault && this._vault is not null)
            throw new VaultAlreadyLoadedException();

        if (this._settingsFile is null || string.IsNullOrEmpty(value: this._settingsFile))
            throw new InvalidOperationException(message: "Settings file not set");

        if (!File.Exists(path: this._settingsFile))
            throw new FileNotFoundException(message: "Settings file not found",
                fileName: this._settingsFile);

        this._vault = EncryptableSettingsVault.Load(vaultKey: vaultKey,
            fileName: this._settingsFile);
    }

    public void CreateVault(SecureString vaultKey, bool replaceLoadedVault, bool overwriteExisting)
    {
        if (!replaceLoadedVault && this._vault is not null)
            throw new VaultAlreadyLoadedException();

        if (this._settingsFile is null || string.IsNullOrEmpty(value: this._settingsFile))
            throw new InvalidOperationException(message: "Settings file not set");

        if (File.Exists(path: this._settingsFile) && !overwriteExisting)
            throw new InvalidOperationException(
                message: $"Settings file already exists and overwrite false: {this._settingsFile}");

        this._vault = new EncryptableSettingsVault(vaultKey: vaultKey);
        this._vault.Save(fileName: this._settingsFile);
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

    public void Dispose()
    {
        foreach (var (guid, connection) in this._connectedAccounts)
            this.DisconnectAccount(accountId: guid,
                quit: true);

        this.Ml.Dispose();
        this._scrollView.Dispose();
        this.SettingsVault.Dispose();
        // window seems to already be disposed
        //this._window.Dispose();
    }

    public static void MenuKeysStyle_Toggled(bool e)
    {
        Singleton.Menu.UseKeysUpDownAsKeysLeftRight = Singleton.MenuKeysStyle.Checked;
    }

    public static void MenuAutoMouseNav_Toggled(bool e)
    {
        Singleton.Menu.WantMousePositionReports = Singleton.MenuAutoMouseNav.Checked;
    }

    public static bool Quit()
    {
        var n = MessageBox.Query(
            width: 50,
            height: 7,
            title: $"Quit {Constants.ProgramName}",
            message: $"Are you sure you want to quit {Constants.ProgramName}?",
            "Yes",
            "No");
        return n == 0;
    }

    public static void Help()
    {
        MessageBox.Query(width: 50,
            height: 7,
            title: "Help",
            message: "This is a small help\nBe kind.",
            "Ok");
    }


    public static void ShowTextAlignments()
    {
        var container = new Window(title: "Show Text Alignments - Press Esc to return")
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };
        container.KeyUp += e =>
        {
            if (e.KeyEvent.Key == Key.Esc)
                container.Running = false;
        };

        var i = 0;
        var txt = "Hello world, how are you doing today?";
        container.Add(
            new Label(text: $"{i + 1}-{txt}") {TextAlignment = TextAlignment.Left, Y = 3, Width = Dim.Fill()},
            new Label(text: $"{i + 2}-{txt}") {TextAlignment = TextAlignment.Right, Y = 5, Width = Dim.Fill()},
            new Label(text: $"{i + 3}-{txt}") {TextAlignment = TextAlignment.Centered, Y = 7, Width = Dim.Fill()},
            new Label(text: $"{i + 4}-{txt}") {TextAlignment = TextAlignment.Justified, Y = 9, Width = Dim.Fill()}
        );

        Application.Run(view: container);
    }

    public void UpdateTheme(HumanEditableTheme theme)
    {
        // set the color scheme for the existing elements
        // TODO: this is only true at the moment. SetTheme will be called from a menu later.
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (this._window is not null)
            this._window.ColorScheme = theme.Base.ToColorScheme();
        if (this.Menu is not null)
            this.Menu.ColorScheme = theme.Menu.ToColorScheme();
        if (this._scrollView is not null)
            this._scrollView.ColorScheme = theme.Base.ToColorScheme();
        // ReSharper enable ConditionIsAlwaysTrueOrFalse
    }

    public static void SetTheme(HumanEditableTheme theme, bool updateExisting = false, GuiCommand? instance = null)
    {
        // load color schemes into the Colors palette which is used to make new elements
        Colors.TopLevel = theme.TopLevel.ToColorScheme();
        Colors.Base = theme.Base.ToColorScheme();
        Colors.Menu = theme.Menu.ToColorScheme();
        Colors.Error = theme.Error.ToColorScheme();
        Colors.Dialog = theme.Dialog.ToColorScheme();
        if (updateExisting)
            (instance ?? Singleton).UpdateTheme(theme: theme);
    }

    private void AddScrollViewChild()
    {
        if (this._isBox10X)
            this._scrollView.Add(view: new Box10X(x: 0,
                y: 0));
        else
            this._scrollView.Add(view: new Filler(rect: new Rect(x: 0,
                y: 0,
                width: 40,
                height: 40)));

        this._scrollView.ContentOffset = Point.Empty;
    }

    public async Task<bool> ShowVaultPrompt(Toplevel newTop)
    {
        newTop.Add(view: this._vaultPrompt);
        var result = await this._vaultPrompt.Run(top: newTop);
        newTop.Remove(view: this._vaultPrompt);
        return result;
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

    public bool SaveToVaultFile(string settingsFilePath, bool overwrite = false)
    {
        if (this.SettingsVault is null) throw new VaultNotLoadedException();

        if (!overwrite && Directory.Exists(path: Path.GetDirectoryName(path: settingsFilePath)) && File.Exists(
                path: settingsFilePath))
            return false;

        this.SettingsVault.Save(fileName: settingsFilePath);

        return true;
    }

    public void SaveVault()
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

        var settingsFileName = Utilities.GetSettingsFile();
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

    public void Copy()
    {
        var textField = (this.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        if (textField != null && textField.SelectedLength != 0) textField.Copy();
    }

    public void Cut()
    {
        var textField = (this.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        if (textField != null && textField.SelectedLength != 0) textField.Cut();
    }

    public void Paste()
    {
        var textField = (this.Menu.LastFocused as TextField ?? Application.Top.MostFocused as TextField) ??
                        throw new InvalidOperationException();
        textField?.Paste();
    }

    public void LoadVault()
    {
        var q = MessageBox.Query(
            width: 50,
            height: 7,
            title: "Load",
            message: this.SettingsVault.HasChanged ? "Changes detected. Load without saving?" : "Load",
            "Yes",
            "Cancel");
        if (q != 0) return;

        var settingsFileName = Utilities.GetSettingsFile();
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
        this.SettingsVault[key: nameof(this.MailAccounts)] = EncryptedObjectSetting.Create(
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
        this.SettingsVault[key: nameof(this.MailAccounts)] = EncryptedObjectSetting.Create(
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
        var mailService = MailClient.ConnectMailService(unlockedMailAccount: account.Unlock(vaultKey: vaultKey));
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

    #region KeyDown / KeyPress / KeyUp Demo

    private static void OnKeyDownPressUpDemo()
    {
        var close = new Button(text: "Close");
        close.Clicked += () => { Application.RequestStop(); };
        var container = new Dialog(title: "KeyDown & KeyPress & KeyUp demo",
            width: 80,
            height: 20,
            close)
        {
            Width = Dim.Fill(),
            Height = Dim.Fill(),
        };

        var list = new List<string>();
        var listView = new ListView(source: list)
        {
            X = 0,
            Y = 0,
            Width = Dim.Fill() - 1,
            Height = Dim.Fill() - 2,
        };
        listView.ColorScheme = Colors.TopLevel;
        container.Add(view: listView);

        void KeyDownPressUp(KeyEvent keyEvent, string upDown)
        {
            const int ident = -5;
            switch (upDown)
            {
                case "Down":
                case "Up":
                case "Press":
                    var msg = $"Key{upDown,ident}: ";
                    if ((keyEvent.Key & Key.ShiftMask) != 0)
                        msg += "Shift ";
                    if ((keyEvent.Key & Key.CtrlMask) != 0)
                        msg += "Ctrl ";
                    if ((keyEvent.Key & Key.AltMask) != 0)
                        msg += "Alt ";
                    msg +=
                        $"{(((uint) keyEvent.KeyValue & (uint) Key.CharMask) > 26 ? $"{(char) keyEvent.KeyValue}" : $"{keyEvent.Key}")}";
                    list.Add(item: msg);
                    //list.Add(item: keyEvent.ToString());

                    break;

                default:
                    if ((keyEvent.Key & Key.ShiftMask) != 0)
                        list.Add(item: $"Key{upDown,ident}: Shift ");
                    else if ((keyEvent.Key & Key.CtrlMask) != 0)
                        list.Add(item: $"Key{upDown,ident}: Ctrl ");
                    else if ((keyEvent.Key & Key.AltMask) != 0)
                        list.Add(item: $"Key{upDown,ident}: Alt ");
                    else
                        list.Add(
                            item:
                            $"Key{upDown,ident}: {(((uint) keyEvent.KeyValue & (uint) Key.CharMask) > 26 ? $"{(char) keyEvent.KeyValue}" : $"{keyEvent.Key}")}");

                    break;
            }

            listView.MoveDown();
        }

        container.KeyDown += e => KeyDownPressUp(keyEvent: e.KeyEvent,
            upDown: "Down");
        container.KeyPress += e => KeyDownPressUp(keyEvent: e.KeyEvent,
            upDown: "Press");
        container.KeyUp += e => KeyDownPressUp(keyEvent: e.KeyEvent,
            upDown: "Up");
        Application.Run(view: container);
    }

    #endregion

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
    // -- end paste --
}