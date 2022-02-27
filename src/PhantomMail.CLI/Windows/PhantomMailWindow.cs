using PhantomKit.Helpers;
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
}