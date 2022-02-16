using System.Text;
using PhantomKit.Helpers;
using Terminal.Gui;

namespace PhantomMail;

public static class PhantomMail
{
    private static void Main(string[] args)
    {
        if (args.Any() && args.Contains(value: "-usc")) Application.UseSystemConsole = true;

        Console.OutputEncoding = Encoding.Default;

        Application.Init();
        using var guiContext = new GuiContext(
            top: Application.Top);

        Application.Run();
        /* main loop */
        Application.Shutdown();
    }
}