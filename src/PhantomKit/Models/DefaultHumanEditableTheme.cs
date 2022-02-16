using System.Text.Json.Serialization;

namespace PhantomKit.Models;

public sealed record DefaultHumanEditableTheme : HumanEditableTheme
{
    [JsonIgnore] public new const string Name = "Default";

    public DefaultHumanEditableTheme() : base(TopLevel:
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
                BackgroundColor = ConsoleColor.DarkBlue,
            },
            Focus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Black,
                BackgroundColor = ConsoleColor.Gray,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.DarkCyan,
                BackgroundColor = ConsoleColor.DarkBlue,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Blue,
                BackgroundColor = ConsoleColor.Gray,
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
                BackgroundColor = ConsoleColor.Black,
            },
            HotNormal: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Yellow,
                BackgroundColor = ConsoleColor.DarkGray,
            },
            HotFocus: new HumanEditableAttribute
            {
                ForegroundColor = ConsoleColor.Yellow,
                BackgroundColor = ConsoleColor.Black,
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