using PhantomKit.Helpers;
using PhantomKit.Models.Commands;
using Terminal.Gui;

namespace PhantomMail.Windows;

public class PhantomKitWindow : Window
{
    protected readonly HostedGuiCommandBase GuiCommand;

    public PhantomKitWindow(HostedGuiCommandBase guiCommand) : base(title: Constants.ProgramName)
    {
        this.GuiCommand = guiCommand;
        this.X = 0;
        this.Y = 1;

        this.Width = Dim.Fill();
        this.Height = Dim.Fill() - 1; // menu + status are outside of the window, or just one of them?
    }
}