using System.Diagnostics;
using System.Security;
using MailKit;
using NStack;
using PhantomKit.Exceptions;
using PhantomKit.Menus;
using PhantomKit.Models;
using PhantomKit.StatusBars;
using PhantomKit.Windows;
using Spectre.Console;
using Terminal.Gui;

namespace PhantomKit.Helpers;

public sealed class GuiContext : IDisposable
{
    private readonly Dictionary<Guid, IMailService> _connectedAccounts = new();
    private readonly MenuBar _menu;
    private readonly MenuBarItem _miScrollViewCheck;
    private readonly Label _ml;
    private readonly ScrollView _scrollView;
    private readonly Window _window;
    private readonly bool _isBox10X = true;

    public GuiContext(Toplevel top)
    {
        // keep a reference to the settings vault
        this.SettingsVault = AppLoadVaultOrNewVault();

        // set the color scheme/defaults for new gui elements created
        this.SetTheme(theme: this.SettingsVault.Theme);

        // create the window
        this._window = new PhantomMailWindow();
        this._menu = new PhantomMailMainMenu();

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
        this.ShowEntries(container: this._window);

        var count = 0;
        this._ml = new Label(rect: new Rect(x: 3,
                y: 17,
                width: 47,
                height: 1),
            text: "Mouse: ");
        Application.RootMouseEvent += me => this._ml.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags} {count++}";

        this._window.Add(view: this._ml);

        top.Add(this._window,
            this._menu,
            new PhantomMailStatusBar(guiContext: this));
    }

    public EncryptableSettingsVault SettingsVault { get; }

    public EncryptableSettingsVault SettingsVaultCopy => new(other: this.SettingsVault);


    /// <summary>
    ///     Virtual list of mail accounts extracted from EncryptedSettings
    /// </summary>
    // Naming is on purpose as we use nameof(MailAccounts) and want our key to be "MailAccounts"
    // ReSharper disable once InconsistentNaming
    private IEnumerable<EncryptedMailAccount> MailAccounts => this.SettingsVault is not null
        ? this.SettingsVault.MailAccounts(vaultKey: this.SettingsVault.VaultKey)
        : throw new VaultNotLoadedException();

    public void Dispose()
    {
        Spectre.PrintStatus(message: "exiting...",
            colorWrapMessage: true);
        Spectre.PrintStatus(message: "disconnecting from all connected accounts",
            colorWrapMessage: true);
        foreach (var (guid, connection) in this._connectedAccounts)
            this.DisconnectAccount(accountId: guid,
                quit: true);
        Spectre.PrintStatus(message: "disconnection complete",
            colorWrapMessage: true);

        Spectre.PrintStatus(message: string.Concat(
                str0: "press ",
                str1: Spectre.SpectreTag(
                    tag: "blue",
                    content: "any key"),
                str2: " to exit"),
            colorWrapMessage: false);

        this._ml.Dispose();
        this._scrollView.Dispose();
        this.SettingsVault.Dispose();
        // window seems to already be disposed
        //this._window.Dispose();
    }

    public void SetTheme(HumanEditableTheme theme)
    {
        // load color schemes into the Colors palette which is used to make new elements
        Colors.TopLevel = theme.TopLevel.ToColorScheme();
        Colors.Base = theme.Base.ToColorScheme();
        Colors.Menu = theme.Menu.ToColorScheme();
        Colors.Error = theme.Error.ToColorScheme();
        Colors.Dialog = theme.Dialog.ToColorScheme();

        // set the color scheme for the existing elements
        // TODO: this is only true at the moment. SetTheme will be called from a menu later.
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (this._window is not null)
            this._window.ColorScheme = theme.Base.ToColorScheme();
        if (this._menu is not null)
            this._menu.ColorScheme = theme.Menu.ToColorScheme();
        if (this._scrollView is not null)
            this._scrollView.ColorScheme = theme.Base.ToColorScheme();
        // ReSharper enable ConditionIsAlwaysTrueOrFalse
    }

    private void ShowEntries(View container)
    {
        this.AddScrollViewChild();

        // This is just to debug the visuals of the scrollview when small
        var scrollView2 = new ScrollView(frame: new Rect(x: 72,
            y: 10,
            width: 3,
            height: 3))
        {
            ContentSize = new Size(width: 100,
                height: 100),
            ShowVerticalScrollIndicator = true,
            ShowHorizontalScrollIndicator = true,
        };
        scrollView2.Add(view: new Box10X(x: 0,
            y: 0));
        var progress = new ProgressBar(rect: new Rect(x: 68,
            y: 1,
            width: 10,
            height: 1));

        bool Timer(MainLoop _)
        {
            progress.Pulse();
            return true;
        }

        Application.MainLoop.AddTimeout(time: TimeSpan.FromMilliseconds(value: 300),
            callback: Timer);

        // A little convoluted, this is because I am using this to test the
        // layout based on referencing elements of another view:

        var login = new Label(text: "Login: ") {X = 3, Y = 6};
        var password = new Label(text: "Password: ")
        {
            X = Pos.Left(view: login),
            Y = Pos.Bottom(view: login) + 1,
        };
        var loginText = new TextField(text: "")
        {
            X = Pos.Right(view: password),
            Y = Pos.Top(view: login),
            Width = 40,
        };
        var passText = new TextField(text: "")
        {
            Secret = true,
            X = Pos.Left(view: loginText),
            Y = Pos.Top(view: password),
            Width = Dim.Width(view: loginText),
        };

        // Add some content
        container.Add(
            login,
            loginText,
            password,
            passText,
            new FrameView(frame: new Rect(x: 3,
                    y: 10,
                    width: 25,
                    height: 6),
                title: "Options",
                views: new View[]
                {
                    new CheckBox(x: 1,
                        y: 0,
                        s: "Remember me"),
                    new RadioGroup(x: 1,
                        y: 2,
                        radioLabels: new ustring[] {"_Personal", "_Company"}),
                }
            ),
            new ListView(rect: new Rect(x: 60,
                    y: 6,
                    width: 16,
                    height: 4),
                source: new[]
                {
                    "First row",
                    "<>",
                    "This is a very long row that should overflow what is shown",
                    "4th",
                    "There is an empty slot on the second row",
                    "Whoa",
                    "This is so cool",
                }),
            this._scrollView,
            scrollView2,
            new Button(text: "Ok") {X = 3, Y = 19},
            new Button(text: "Cancel") {X = 10, Y = 19},
            progress,
            new Label(text: "Press F9 (on Unix ESC+9 is an alias) to activate the menubar") {X = 3, Y = 22}
        );
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


    public static EncryptableSettingsVault AppLoadVaultOrNewVault(string? settingsDirectory = null,
        string? settingsFile = null)
    {
        AnsiConsole.MarkupLine(
            value: string.Concat(
                str0: Spectre.SpectreTag(tag: Constants.WelcomeColor,
                    content: "Welcome to"),
                str1: Spectre.PrettyProgramName));
        AnsiConsole.MarkupLine(value: string.Empty);

        Spectre.PrintStatus(message: "checking for configuration file",
            colorWrapMessage: true);

        var settingsFilePath = Utilities.SettingsFile(
            directoryName: settingsDirectory,
            fileName: settingsFile);

        if (Directory.Exists(path: Path.GetDirectoryName(path: settingsFilePath)) && File.Exists(
                path: settingsFilePath))
            return AppLoadExistingVault(settingsFilePath: settingsFilePath);

        var vault = AppLoadNewVault(settingsFilePath: settingsFilePath,
            vaultKey: out var vaultKey);
        Debug.Assert(condition: vault is not null);
        Debug.Assert(condition: vault.VaultKey is null);
        vault.SetVaultKey(vaultKey: vaultKey);

        var mailAccounts = vault!.MailAccounts().ToArray();

        if (!mailAccounts.Any())
            Spectre.PrintStatus(message: "no mail accounts found.",
                colorWrapMessage: true);
        // TODO: prompt for account creation
        return vault;
    }

    public static EncryptableSettingsVault AppLoadNewVault(string settingsFilePath, out SecureString vaultKey)
    {
        Spectre.PrintStatus(message: "starting new account wizard",
            colorWrapMessage: true);
        vaultKey = VaultPromptToSecurePassword(isNew: true);
        // vault does not keep the key by default
        var vault = new EncryptableSettingsVault(vaultKey: vaultKey);
        Spectre.PrintStatus(message: "default configuration created, saving...",
            colorWrapMessage: true);
        try
        {
            vault.Save(fileName: settingsFilePath);
            Spectre.PrintSuccess(message: string.Concat(str0: "new configuration file created: ",
                str1: settingsFilePath));
            return vault;
        }
        catch (Exception e)
        {
            Spectre.PrintError(message: string.Concat(str0: "failed to create new configuration file: ",
                str1: settingsFilePath));
            throw new VaultNotLoadedException(innerException: e);
        }
    }

    public static EncryptableSettingsVault AppLoadExistingVault(string settingsFilePath)
    {
        if (Directory.Exists(path: Path.GetDirectoryName(path: settingsFilePath)) && File.Exists(
                path: settingsFilePath))
        {
            Exception? reThrow = null;
            Spectre.PrintSuccess(message: string.Concat(str0: "configuration file found: ",
                str1: settingsFilePath));
            var vaultKey = VaultPromptToSecurePassword(isNew: false);
            try
            {
                var vault = EncryptableSettingsVault.Load(vaultKey: vaultKey);
                Spectre.PrintStatus(message: "configuration file loaded",
                    colorWrapMessage: true);
                return vault;
            }
            catch (PhantomKitException pkEx)
            {
                reThrow = pkEx;
                Spectre.PrintError(pkEx: pkEx);
            }
            catch (Exception e)
            {
                reThrow = e;
                Spectre.PrintError(e: e);
            }

            throw new InvalidSettingsException(innerException: reThrow);
        }

        Spectre.PrintError(message: string.Concat(str0: "configuration not file found: ",
            str1: settingsFilePath));
        throw new InvalidSettingsException();
    }

    private static SecureString VaultPromptToSecurePassword(bool isNew)
    {
        var passwordPrompt = new TextPrompt<string>(prompt: isNew ? "New vault password" : "Existing vault password");
        passwordPrompt
            .PromptStyle(style: Constants.ErrorColor)
            .Secret();

        var insecurePassword = AnsiConsole.Prompt(prompt: passwordPrompt);

        var securePassword = new SecureString();
        foreach (var c in insecurePassword)
            securePassword.AppendChar(c: c);
        securePassword.MakeReadOnly();
        return securePassword;
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

    private class Box10X : View
    {
        public Box10X(int x, int y) : base(frame: new Rect(x: x,
            y: y,
            width: 10,
            height: 10))
        {
        }

        public override void Redraw(Rect region)
        {
            Driver.SetAttribute(c: this.ColorScheme.Focus);

            for (var y = 0; y < 10; y++)
            {
                this.Move(col: 0,
                    row: y);
                for (var x = 0; x < 10; x++) Driver.AddRune(rune: (Rune) ('0' + (x + y) % 10));
            }
        }
    }

    private class Filler : View
    {
        public Filler(Rect rect) : base(frame: rect)
        {
        }

        public override void Redraw(Rect region)
        {
            Driver.SetAttribute(c: this.ColorScheme.Focus);
            var f = this.Frame;

            for (var y = 0; y < f.Width; y++)
            {
                this.Move(col: 0,
                    row: y);
                for (var x = 0; x < f.Height; x++)
                {
                    var r = (x % 3) switch
                    {
                        0 => '.',
                        1 => 'o',
                        _ => 'O',
                    };
                    Driver.AddRune(rune: r);
                }
            }
        }
    }
}