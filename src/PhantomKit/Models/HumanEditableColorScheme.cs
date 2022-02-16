using System.Text.Json.Serialization;
using Terminal.Gui;

namespace PhantomKit.Models;

[Serializable]
public record HumanEditableColorScheme(HumanEditableAttribute Normal, HumanEditableAttribute Focus,
    HumanEditableAttribute HotNormal, HumanEditableAttribute HotFocus,
    HumanEditableAttribute Disabled) : IEquatable<ColorScheme>
{
    /// <summary>
    ///     The default color for text, when the view is not focused.
    /// </summary>
    [JsonInclude]
    public HumanEditableAttribute Normal { get; set; } = Normal;

    /// <summary>The color for text when the view has the focus.</summary>
    [JsonInclude]
    public HumanEditableAttribute Focus { get; set; } = Focus;

    /// <summary>The color for the hotkey when a view is not focused</summary>
    [JsonInclude]
    public HumanEditableAttribute HotNormal { get; set; } = HotNormal;

    /// <summary>The color for the hotkey when the view is focused.</summary>
    [JsonInclude]
    public HumanEditableAttribute HotFocus { get; set; } = HotFocus;

    /// <summary>The default color for text, when the view is disabled.</summary>
    [JsonInclude]
    public HumanEditableAttribute Disabled { get; set; } = Disabled;

    public virtual bool Equals(ColorScheme? other)
    {
        return this.ToColorScheme().Equals(other: other);
    }

    public ColorScheme ToColorScheme()
    {
        return new ColorScheme
        {
            Disabled = this.Disabled.ToAttribute(),
            Normal = this.Normal.ToAttribute(),
            Focus = this.Focus.ToAttribute(),
            HotFocus = this.HotFocus.ToAttribute(),
            HotNormal = this.HotNormal.ToAttribute(),
        };
    }

    public static HumanEditableColorScheme FromColorScheme(ColorScheme colorScheme)
    {
        return new HumanEditableColorScheme(
            Normal: HumanEditableAttribute.FromAttribute(attribute: colorScheme.Normal),
            Focus: HumanEditableAttribute.FromAttribute(attribute: colorScheme.Focus),
            HotNormal: HumanEditableAttribute.FromAttribute(attribute: colorScheme.HotNormal),
            HotFocus: HumanEditableAttribute.FromAttribute(attribute: colorScheme.HotFocus),
            Disabled: HumanEditableAttribute.FromAttribute(attribute: colorScheme.Disabled)
        );
    }

    public virtual int GetHashCode(object other)
    {
        if (other is HumanEditableColorScheme scheme) return scheme.ToColorScheme().GetHashCode();

        return 0;
    }

    public override int GetHashCode()
    {
        return this.ToColorScheme().GetHashCode();
    }
}