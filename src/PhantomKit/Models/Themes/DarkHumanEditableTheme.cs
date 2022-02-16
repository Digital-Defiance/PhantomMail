using Terminal.Gui;

namespace PhantomKit.Models.Themes;

public sealed record DarkHumanEditableTheme : HumanEditableTheme
{
    public new const string Name = "Dark";

    public DarkHumanEditableTheme() : base(
        TopLevel:
        new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = Color.Green,
                BackgroundColor = Color.Black,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Cyan,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.BrightYellow,
                BackgroundColor = Color.Black,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.Blue,
                BackgroundColor = Color.Cyan,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = Color.DarkGray,
                BackgroundColor = Color.Black,
            }),
        Base: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Black,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Blue,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.DarkGray,
                BackgroundColor = Color.Black,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Cyan,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = Color.DarkGray,
                BackgroundColor = Color.Blue,
            }),
        Menu: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.DarkGray,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Blue,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Blue,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.Blue,
                BackgroundColor = Color.Cyan,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = Color.Gray,
                BackgroundColor = Color.DarkGray,
            }),
        Dialog: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = Color.Black,
                BackgroundColor = Color.Gray,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.DarkGray,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.Blue,
                BackgroundColor = Color.Gray,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.Blue,
                BackgroundColor = Color.DarkGray,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = Color.DarkGray,
                BackgroundColor = Color.Gray,
            }),
        Error: new HumanEditableColorScheme(
            Normal: new HumanEditableAttribute
            {
                ForegroundColor = Color.Red,
                BackgroundColor = Color.White,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = Color.White,
                BackgroundColor = Color.Red,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.Black,
                BackgroundColor = Color.White,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.Black,
                BackgroundColor = Color.Red,
            },
            Disabled: new HumanEditableAttribute
            {
                ForegroundColor = Color.DarkGray,
                BackgroundColor = Color.White,
            })
    )
    {
    }
}