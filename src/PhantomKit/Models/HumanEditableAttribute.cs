using System.Text.Json.Serialization;
using Terminal.Gui;
using Attribute = Terminal.Gui.Attribute;

namespace PhantomKit.Models;

[Serializable]
public record HumanEditableAttribute
{
    [JsonInclude] public string Foreground { get; set; } = Color.White.ToString();

    [JsonIgnore]
    public Color ForegroundColor
    {
        get => (Color) Enum.Parse(enumType: typeof(Color),
            value: this.Foreground,
            ignoreCase: true);
        set => this.Foreground = Enum.GetName(enumType: typeof(Color),
            value: value)!;
    }

    [JsonInclude] public string Background { get; set; } = Color.Blue.ToString();

    [JsonIgnore]
    public Color BackgroundColor
    {
        get => (Color) Enum.Parse(enumType: typeof(Color),
            value: this.Background,
            ignoreCase: true);
        set => this.Background = Enum.GetName(enumType: typeof(Color),
            value: value)!;
    }

    public static Attribute MakeColor(Color f, Color b)
    {
        // Encode the colors into the int value.
        return new Attribute(
            value: (int) f | ((int) b << 4),
            foreground: f,
            background: b
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
            ForegroundColor = attribute.Foreground,
            BackgroundColor = attribute.Background,
        };
    }
}