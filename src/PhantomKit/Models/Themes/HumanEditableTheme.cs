using System.Collections.Immutable;
using System.Text.Json.Serialization;
using Terminal.Gui;

namespace PhantomKit.Models.Themes;

[Serializable]
public record HumanEditableTheme(HumanEditableColorScheme TopLevel, HumanEditableColorScheme Base,
    HumanEditableColorScheme Dialog, HumanEditableColorScheme Menu, HumanEditableColorScheme Error)
{
    [JsonIgnore] public const string Name = "Terminal";

    [JsonIgnore]
    public static readonly ImmutableDictionary<string, HumanEditableTheme> BuiltinThemes =
        new Dictionary<string, HumanEditableTheme>
        {
            {nameof(Themes.Blue), Themes.Blue},
            {nameof(Themes.Dark), Themes.Dark},
        }.ToImmutableDictionary();

    public HumanEditableTheme() : this(
        TopLevel: HumanEditableColorScheme.FromColorScheme(colorScheme: Colors.TopLevel),
        Base: HumanEditableColorScheme.FromColorScheme(colorScheme: Colors.Base),
        Dialog: HumanEditableColorScheme.FromColorScheme(colorScheme: Colors.Dialog),
        Menu: HumanEditableColorScheme.FromColorScheme(colorScheme: Colors.Menu),
        Error: HumanEditableColorScheme.FromColorScheme(colorScheme: Colors.Error))
    {
    }

    /// <summary>
    /// </summary>
    /// <param name="TopLevel"></param>
    /// <param name="Base"></param>
    /// <param name="Dialog"></param>
    /// <param name="Menu"></param>
    /// <param name="Error"></param>
    // ReSharper disable InconsistentNaming
    public HumanEditableTheme(ColorScheme TopLevel, ColorScheme Base, ColorScheme Dialog, ColorScheme Menu,
        ColorScheme Error)
        : this(
            TopLevel: HumanEditableColorScheme.FromColorScheme(colorScheme: TopLevel),
            Base: HumanEditableColorScheme.FromColorScheme(colorScheme: Base),
            Dialog: HumanEditableColorScheme.FromColorScheme(colorScheme: Dialog),
            Menu: HumanEditableColorScheme.FromColorScheme(colorScheme: Menu),
            Error: HumanEditableColorScheme.FromColorScheme(colorScheme: Error))
    {
    }
    // ReSharper restore InconsistentNaming

    [JsonInclude] public HumanEditableColorScheme TopLevel { get; set; } = TopLevel;

    [JsonInclude] public HumanEditableColorScheme Base { get; set; } = Base;

    [JsonInclude] public HumanEditableColorScheme Dialog { get; set; } = Dialog;

    [JsonInclude] public HumanEditableColorScheme Menu { get; set; } = Menu;

    [JsonInclude] public HumanEditableColorScheme Error { get; set; } = Error;

    public static class Themes
    {
        [JsonIgnore] public static HumanEditableTheme Blue => new BlueHumanEditableTheme();
        [JsonIgnore] public static HumanEditableTheme Dark => new DarkHumanEditableTheme();
    }
}