using System.Diagnostics;
using System.Globalization;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PhantomKit.Helpers;
using PhantomKit.Interfaces;
using PhantomKit.Models.StatusBars;
using PhantomKit.Models.Themes;
using PhantomKit.Models.Views;
using PhantomMail.Menus;
using PhantomMail.Windows;
using Serilog;
using Terminal.Gui;

namespace PhantomKit.Models.Commands;

[HelpOption(template: "--help")]
public abstract class GuiCommand : IGuiCommand
{
    private static GuiCommand? _singleton;

    protected readonly IConfiguration Configuration;

    protected readonly IConsole Console;

    protected readonly bool IsBox10X = true;
    protected readonly ILogger Logger;
    protected readonly ScrollView ScrollView;
    protected readonly Window Window;

    protected MenuBar Menu;
    protected CheckBox MenuAutoMouseNav;
    protected CheckBox MenuKeysStyle;
    protected Label? Ml;
    protected Label? Ml2;
    protected StatusBar StatusBar;

    public GuiCommand(ILogger logger, IConfiguration configuration, IConsole console)
    {
        if (_singleton != null)
            throw new InvalidOperationException(message: "Only one instance of GuiCommand can be created");

        _singleton = this;
        this.Logger = logger;
        this.Configuration = configuration;
        this.Console = console;

        Application.Init();

        // create the window
        this.Window = new PhantomKitWindow();
        this.Menu = new PhantomKitMainMenu();

        this.ScrollView = new ScrollView(frame: new Rect(x: 50,
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

        const bool enableMouse = false;
        this.MenuAutoMouseNav = new CheckBox(x: 40,
            y: 25,
            s: "UseMenuAutoNavigation",
            is_checked: enableMouse);
        // ReSharper disable HeuristicUnreachableCode
        if (enableMouse)
        {
            this.MenuAutoMouseNav.Toggled += MenuAutoMouseNav_Toggled;

            var count = 0;
            this.Ml = new Label(rect: new Rect(x: 3,
                    y: 17,
                    width: 47,
                    height: 1),
                text: "Mouse: ");

            Application.RootMouseEvent += me => this.Ml!.Text = $"Mouse: ({me.X},{me.Y}) - {me.Flags} {count++}";

            this.Window.Add(view: this.Ml);
        }
        // ReSharper restore HeuristicUnreachableCode

        this.StatusBar = new PhantomKitStatusBar(guiCommand: this);
        Application.Top.Add(this.Window,
            this.Menu,
            this.StatusBar);
    }

    public static GuiCommand Singleton
        => _singleton ?? throw new InvalidOperationException(message: "GuiCommand has not been initialized");

    public bool DarkMode { get; set; } = true;


    public Toplevel RebuildWindow()
    {
        var newTop = Application.Top;
        newTop.Clear();
        this.Menu = new PhantomKitMainMenu();
        newTop.Add(view: this.Menu);
        newTop.Add(view: new PhantomKitStatusBar(guiCommand: this));
        var win = new PhantomKitWindow();
        win.KeyPress += Win_KeyPress;
        newTop.Add(view: win);
        return newTop;
    }

    public virtual int OnExecute(CommandLineApplication app)
    {
        if (Debugger.IsAttached)
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(name: "en-US");

        Application.Init();
        Application.HeightAsBuffer = true;
        Application.Run(view: this.RebuildWindow());
        return 0;
    }

    public void OnException(Exception ex)
    {
        this.OutputError(message: ex.Message);
        // ReSharper disable StructuredMessageTemplateProblem
        this.Logger.Error(exception: ex,
            messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        this.Logger.Debug(exception: ex,
            messageTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}");
        // ReSharper restore StructuredMessageTemplateProblem
    }

    public void OutputToConsole(string data)
    {
        this.Console.BackgroundColor = ConsoleColor.Black;
        this.Console.ForegroundColor = ConsoleColor.White;
        this.Console.Out.Write(value: data);
        this.Console.ResetColor();
    }

    public void OutputError(string message)
    {
        this.Console.BackgroundColor = ConsoleColor.Red;
        this.Console.ForegroundColor = ConsoleColor.White;
        this.Console.Error.WriteLine(value: message);
        this.Console.ResetColor();
    }

    public void Dispose()
    {
        this.Ml?.Dispose();
        this.Ml2?.Dispose();
        this.ScrollView.Dispose();
        // window seems to already be disposed
        //this._window.Dispose();
    }

    public void UpdateTheme(HumanEditableTheme theme)
    {
        // set the color scheme for the existing elements
        // TODO: this is only true at the moment. SetTheme will be called from a menu later.
        // ReSharper disable ConditionIsAlwaysTrueOrFalse
        if (this.Window is not null)
            this.Window.ColorScheme = theme.Base.ToColorScheme();
        if (this.Menu is not null)
            this.Menu.ColorScheme = theme.Menu.ToColorScheme();
        if (this.ScrollView is not null)
            this.ScrollView.ColorScheme = theme.Base.ToColorScheme();
        // ReSharper enable ConditionIsAlwaysTrueOrFalse
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

    public static void SetTheme(HumanEditableTheme theme, GuiCommand? instance = null)
    {
        // load color schemes into the Colors palette which is used to make new elements
        Colors.TopLevel = theme.TopLevel.ToColorScheme();
        Colors.Base = theme.Base.ToColorScheme();
        Colors.Menu = theme.Menu.ToColorScheme();
        Colors.Error = theme.Error.ToColorScheme();
        Colors.Dialog = theme.Dialog.ToColorScheme();
        instance?.UpdateTheme(theme: theme);
    }

    public void AddScrollViewChild()
    {
        if (this.IsBox10X)
            this.ScrollView.Add(view: new Box10X(x: 0,
                y: 0));
        else
            this.ScrollView.Add(view: new Filler(rect: new Rect(x: 0,
                y: 0,
                width: 40,
                height: 40)));

        this.ScrollView.ContentOffset = Point.Empty;
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

    #region KeyDown / KeyPress / KeyUp Demo

    protected static void OnKeyDownPressUpDemo()
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
}