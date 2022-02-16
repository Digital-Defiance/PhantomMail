using PhantomKit.Helpers;
using Terminal.Gui;

namespace PhantomKit.Windows;

public class PhantomMailWindow : Window
{
    public PhantomMailWindow() : base(title: Constants.ProgramName)
    {
        this.X = 0;
        this.Y = 1;
        this.Width = Dim.Fill();
        this.Height = Dim.Fill() - 1;
    }
}