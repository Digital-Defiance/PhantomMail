using System.Diagnostics;
using System.Globalization;
using PhantomKit.Helpers;
using PhantomMail.Menus;
using PhantomMail.StatusBars;
using Terminal.Gui;

namespace PhantomMail.Windows;

public class PhantomMailWindow : Window
{
    public PhantomMailWindow() : base(title: Constants.ProgramName)
    {
        this.X = 0;
        this.Y = 1;

        this.Width = Dim.Fill();
        this.Height = Dim.Fill() - 1; // menu + status are outside of the window, or just one of them?
    }

    public static void Win_KeyPress(KeyEventEventArgs e)
    {
        var guiContext = GuiContext.Instance;
        switch (ShortcutHelper.GetModifiersKey(kb: e.KeyEvent))
        {
            case Key.CtrlMask | Key.T:
                if (guiContext.Menu.IsMenuOpen)
                    guiContext.Menu.CloseMenu();
                else
                    guiContext.Menu.OpenMenu();
                e.Handled = true;
                break;
        }
    }

    public static void Editor()
    {
        if (Debugger.IsAttached)
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo(name: "en-US");

        var guiContext = GuiContext.Instance;
        Application.Init();
        Application.HeightAsBuffer = true;

        var newTop = Application.Top;
        newTop.Clear();
        guiContext.Menu = new PhantomMailMainMenu();
        newTop.Add(view: guiContext.Menu);

        var win = new PhantomMailWindow();


        win.KeyPress += Win_KeyPress;
        newTop.Add(view: win);
        newTop.Add(view: new PhantomMailStatusBar(guiContext: guiContext));
        guiContext.ShowVaultPrompt();
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
    }
}