using System.Text.Json.Serialization;
using Terminal.Gui;

namespace PhantomKit.Models.Themes;

public sealed record BlueHumanEditableTheme : HumanEditableTheme
{
    [JsonIgnore] public new const string Name = "Blue";

    public BlueHumanEditableTheme() : base(TopLevel:
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
                BackgroundColor = Color.Blue,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = Color.Black,
                BackgroundColor = Color.Gray,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.Cyan,
                BackgroundColor = Color.Blue,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.Blue,
                BackgroundColor = Color.Gray,
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
                BackgroundColor = Color.Black,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = Color.BrightYellow,
                BackgroundColor = Color.DarkGray,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = Color.BrightYellow,
                BackgroundColor = Color.Black,
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