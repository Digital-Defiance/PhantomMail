namespace PhantomKit.Models;

public sealed record DarkHumanEditableTheme : HumanEditableTheme
{
    public new const string Name = "Dark";

    public DarkHumanEditableTheme() : base(
        TopLevel:
        new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Green,
                BackgroundColor = ConsoleColor.Black,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkCyan,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkYellow,
                BackgroundColor = ConsoleColor.Black,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkBlue,
                BackgroundColor = ConsoleColor.DarkCyan,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkGray,
                BackgroundColor = ConsoleColor.Black,
            }),
        Base: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.Black,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.Blue,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkGray,
                BackgroundColor = ConsoleColor.Black,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.Cyan,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkGray,
                BackgroundColor = ConsoleColor.DarkBlue,
            }),
        Menu: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkGray,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.Blue,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.Blue,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkBlue,
                BackgroundColor = ConsoleColor.DarkCyan,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Gray,
                BackgroundColor = ConsoleColor.DarkGray,
            }),
        Dialog: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Black,
                BackgroundColor = ConsoleColor.Gray,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkGray,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkBlue,
                BackgroundColor = ConsoleColor.Gray,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkBlue,
                BackgroundColor = ConsoleColor.DarkGray,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkGray,
                BackgroundColor = ConsoleColor.Gray,
            }),
        Error: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkRed,
                BackgroundColor = ConsoleColor.White,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.White,
                BackgroundColor = ConsoleColor.DarkRed,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Black,
                BackgroundColor = ConsoleColor.White,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Black,
                BackgroundColor = ConsoleColor.DarkRed,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkGray,
                BackgroundColor = ConsoleColor.White,
            })
    )
    {
    }
}