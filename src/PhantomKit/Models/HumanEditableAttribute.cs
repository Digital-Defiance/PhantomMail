using System.Text.Json.Serialization;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace PhantomKit.Models;

[Serializable]
public record HumanEditableAttribute
{
    [JsonInclude] public string Foreground { get; set; }

    [JsonIgnore]
    public ConsoleColor ForegroundColor
    {
        get => (ConsoleColor) Enum.Parse(enumType: typeof(ConsoleColor),
            value: this.Foreground,
            ignoreCase: true);
        set => this.Foreground = Enum.GetName(enumType: typeof(ConsoleColor),
            value: value)!;
    }

    [JsonInclude] public string Background { get; set; }

    [JsonIgnore]
    public ConsoleColor BackgroundColor
    {
        get => (ConsoleColor) Enum.Parse(enumType: typeof(ConsoleColor),
            value: this.Background,
            ignoreCase: true);
        set => this.Background = Enum.GetName(enumType: typeof(ConsoleColor),
            value: value)!;
    }

    public static Attribute MakeColor(ConsoleColor f, ConsoleColor b)
    {
        // Encode the colors into the int value.
        return new Attribute(
            value: (int) f | ((int) b << 4),
            foreground: (Color) f,
            background: (Color) b
        );
    }

    public Attribute ToAttribute()
    {
        return MakeColor(
            f: this.ForegroundColor,
            b: this.BackgroundColor);
    }

    public static HumanEditableAttribute FromAttribute(Attribute attribute)
    {
        return new HumanEditableAttribute
        {
            ForegroundColor = (ConsoleColor) attribute.Background,
            BackgroundColor = (ConsoleColor) attribute.Background,
        };
    }
}