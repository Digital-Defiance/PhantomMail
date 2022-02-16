using PhantomKit.Helpers;
using Terminal.Gui;

namespace PhantomKit.Menus;

public class PhantomMailMainMenu : MenuBar
{
    public PhantomMailMainMenu()
    {
        this.Menus = new[]
        {
            new MenuBarItem(title: "_File",
                children: new[]
                {
                    new(title: "_New",
                        help: "Creates new file",
                        action: NewFile),
                    new MenuItem(title: "_Open",
                        help: "",
                        action: null),
                    new MenuItem(title: "_Close",
                        help: "",
                        action: Close),
                    new MenuItem(title: "_Quit",
                        help: "",
                        action: () =>
                        {
                            if (Quit() && Application.Top is not null) Application.Top.Running = false;
                        }),
                }),
            new MenuBarItem(title: "_Edit",
                children: new[]
                {
                    new MenuItem(title: "_Copy",
                        help: "",
                        action: null),
                    new MenuItem(title: "C_ut",
                        help: "",
                        action: null),
                    new MenuItem(title: "_Paste",
                        help: "",
                        action: null),
                }),
        };
    }

    public static void NewFile()
    {
        var okButton = new Button(text: "Ok",
            is_default: true);
        okButton.Clicked += () => Application.RequestStop();
        var cancelButton = new Button(text: "Cancel");
        cancelButton.Clicked += () => Application.RequestStop();

        var d = new Dialog(
            title: "New File",
            width: 50,
            height: 20,
            okButton,
            cancelButton);

        var ml2 = new Label(x: 1,
            y: 1,
            text: "Mouse Debug Line");
        d.Add(view: ml2);
        Application.Run(view: d);
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

    public static void Close()
    {
        MessageBox.ErrorQuery(
            width: 50,
            height: 7,
            title: "Error",
            message: "There is nothing to close",
            "Ok");
    }
}