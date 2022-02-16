/* https://github.com/Myfreedom614/UWP-Samples/blob/master/NearestColorUWPApp1/NearestColorUWPApp1/Helper/ColorHelper.cs */

using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace PhantomKit.Helpers;

public class ColorHelper
{
    public static Vector3 GetSystemDrawingColorFromHexString(string hexString)
    {
        if (!Regex.IsMatch(input: hexString,
                pattern: @"[#]([0-9]|[a-f]|[A-F]){6}\b"))
            throw new ArgumentException();
        var red = int.Parse(s: hexString.Substring(startIndex: 1,
                length: 2),
            style: NumberStyles.HexNumber);
        var green = int.Parse(s: hexString.Substring(startIndex: 3,
                length: 2),
            style: NumberStyles.HexNumber);
        var blue = int.Parse(s: hexString.Substring(startIndex: 5,
                length: 2),
            style: NumberStyles.HexNumber);
        return new Vector3(x: red,
            y: green,
            z: blue);
    }

    public static string GetNearestColorName(Vector3 vect)
    {
        var cr = GetClosestColorReference(vect: vect);
        if (cr != null)
            return cr.Name;
        return string.Empty;
    }

    public static ColorReference? GetClosestColorReference(Vector3 vect)
    {
        return GetClosestColor(colorReferences: GetColorReferences(),
            currentColor: vect);
    }

    private static ColorReference? GetClosestColor(ColorReference[] colorReferences, Vector3 currentColor)
    {
        ColorReference? tMin = null;
        var minDist = float.PositiveInfinity;

        foreach (var t in colorReferences)
        {
            var dist = Vector3.Distance(value1: t.Argb,
                value2: currentColor);
            if (dist < minDist)
            {
                tMin = t;
                minDist = dist;
            }
        }

        return tMin;
    }

    private static ColorReference[] GetColorReferences()
    {
        return new[]
        {
            new()
            {
                Name = "AliceBlue", Argb = new Vector3(
                    x: 240,
                    y: 248,
                    z: 255),
            },
            new ColorReference
            {
                Name = "LightSalmon", Argb = new Vector3(
                    x: 255,
                    y: 160,
                    z: 122),
            },
            new ColorReference
            {
                Name = "AntiqueWhite", Argb = new Vector3(
                    x: 250,
                    y: 235,
                    z: 215),
            },
            new ColorReference
            {
                Name = "LightSeaGreen", Argb = new Vector3(
                    x: 32,
                    y: 178,
                    z: 170),
            },
            new ColorReference
            {
                Name = "Aqua", Argb = new Vector3(
                    x: 0,
                    y: 255,
                    z: 255),
            },
            new ColorReference
            {
                Name = "LightSkyBlue", Argb = new Vector3(
                    x: 135,
                    y: 206,
                    z: 250),
            },
            new ColorReference
            {
                Name = "Aquamarine", Argb = new Vector3(
                    x: 127,
                    y: 255,
                    z: 212),
            },
            new ColorReference
            {
                Name = "LightSlateGray", Argb = new Vector3(
                    x: 119,
                    y: 136,
                    z: 153),
            },
            new ColorReference
            {
                Name = "Azure", Argb = new Vector3(
                    x: 240,
                    y: 255,
                    z: 255),
            },
            new ColorReference
            {
                Name = "LightSteelBlue", Argb = new Vector3(
                    x: 176,
                    y: 196,
                    z: 222),
            },
            new ColorReference
            {
                Name = "Beige", Argb = new Vector3(
                    x: 245,
                    y: 245,
                    z: 220),
            },
            new ColorReference
            {
                Name = "LightYellow", Argb = new Vector3(
                    x: 255,
                    y: 255,
                    z: 224),
            },
            new ColorReference
            {
                Name = "Bisque", Argb = new Vector3(
                    x: 255,
                    y: 228,
                    z: 196),
            },
            new ColorReference
            {
                Name = "Lime", Argb = new Vector3(
                    x: 0,
                    y: 255,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Black", Argb = new Vector3(
                    x: 0,
                    y: 0,
                    z: 0),
            },
            new ColorReference
            {
                Name = "LimeGreen", Argb = new Vector3(
                    x: 50,
                    y: 205,
                    z: 50),
            },
            new ColorReference
            {
                Name = "BlanchedAlmond", Argb = new Vector3(
                    x: 255,
                    y: 255,
                    z: 205),
            },
            new ColorReference
            {
                Name = "Linen", Argb = new Vector3(
                    x: 250,
                    y: 240,
                    z: 230),
            },
            new ColorReference
            {
                Name = "Blue", Argb = new Vector3(
                    x: 0,
                    y: 0,
                    z: 255),
            },
            new ColorReference
            {
                Name = "Magenta", Argb = new Vector3(
                    x: 255,
                    y: 0,
                    z: 255),
            },
            new ColorReference
            {
                Name = "BlueViolet", Argb = new Vector3(
                    x: 138,
                    y: 43,
                    z: 226),
            },
            new ColorReference
            {
                Name = "Maroon", Argb = new Vector3(
                    x: 128,
                    y: 0,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Brown", Argb = new Vector3(
                    x: 165,
                    y: 42,
                    z: 42),
            },
            new ColorReference
            {
                Name = "MediumAquamarine", Argb = new Vector3(
                    x: 102,
                    y: 205,
                    z: 170),
            },
            new ColorReference
            {
                Name = "BurlyWood", Argb = new Vector3(
                    x: 222,
                    y: 184,
                    z: 135),
            },
            new ColorReference
            {
                Name = "MediumBlue", Argb = new Vector3(
                    x: 0,
                    y: 0,
                    z: 205),
            },
            new ColorReference
            {
                Name = "CadetBlue", Argb = new Vector3(
                    x: 95,
                    y: 158,
                    z: 160),
            },
            new ColorReference
            {
                Name = "MediumOrchid", Argb = new Vector3(
                    x: 186,
                    y: 85,
                    z: 211),
            },
            new ColorReference
            {
                Name = "Chartreuse", Argb = new Vector3(
                    x: 127,
                    y: 255,
                    z: 0),
            },
            new ColorReference
            {
                Name = "MediumPurple", Argb = new Vector3(
                    x: 147,
                    y: 112,
                    z: 219),
            },
            new ColorReference
            {
                Name = "Chocolate", Argb = new Vector3(
                    x: 210,
                    y: 105,
                    z: 30),
            },
            new ColorReference
            {
                Name = "MediumSeaGreen", Argb = new Vector3(
                    x: 60,
                    y: 179,
                    z: 113),
            },
            new ColorReference
            {
                Name = "Coral", Argb = new Vector3(
                    x: 255,
                    y: 127,
                    z: 80),
            },
            new ColorReference
            {
                Name = "MediumSlateBlue", Argb = new Vector3(
                    x: 123,
                    y: 104,
                    z: 238),
            },
            new ColorReference
            {
                Name = "CornflowerBlue", Argb = new Vector3(
                    x: 100,
                    y: 149,
                    z: 237),
            },
            new ColorReference
            {
                Name = "MediumSpringGreen", Argb = new Vector3(
                    x: 0,
                    y: 250,
                    z: 154),
            },
            new ColorReference
            {
                Name = "Cornsilk", Argb = new Vector3(
                    x: 255,
                    y: 248,
                    z: 220),
            },
            new ColorReference
            {
                Name = "MediumTurquoise", Argb = new Vector3(
                    x: 72,
                    y: 209,
                    z: 204),
            },
            new ColorReference
            {
                Name = "Crimson", Argb = new Vector3(
                    x: 220,
                    y: 20,
                    z: 60),
            },
            new ColorReference
            {
                Name = "MediumVioletRed", Argb = new Vector3(
                    x: 199,
                    y: 21,
                    z: 112),
            },
            new ColorReference
            {
                Name = "Cyan", Argb = new Vector3(
                    x: 0,
                    y: 255,
                    z: 255),
            },
            new ColorReference
            {
                Name = "MidnightBlue", Argb = new Vector3(
                    x: 25,
                    y: 25,
                    z: 112),
            },
            new ColorReference
            {
                Name = "DarkBlue", Argb = new Vector3(
                    x: 0,
                    y: 0,
                    z: 139),
            },
            new ColorReference
            {
                Name = "MintCream", Argb = new Vector3(
                    x: 245,
                    y: 255,
                    z: 250),
            },
            new ColorReference
            {
                Name = "DarkCyan", Argb = new Vector3(
                    x: 0,
                    y: 139,
                    z: 139),
            },
            new ColorReference
            {
                Name = "MistyRose", Argb = new Vector3(
                    x: 255,
                    y: 228,
                    z: 225),
            },
            new ColorReference
            {
                Name = "DarkGoldenrod", Argb = new Vector3(
                    x: 184,
                    y: 134,
                    z: 11),
            },
            new ColorReference
            {
                Name = "Moccasin", Argb = new Vector3(
                    x: 255,
                    y: 228,
                    z: 181),
            },
            new ColorReference
            {
                Name = "DarkGray", Argb = new Vector3(
                    x: 169,
                    y: 169,
                    z: 169),
            },
            new ColorReference
            {
                Name = "NavajoWhite", Argb = new Vector3(
                    x: 255,
                    y: 222,
                    z: 173),
            },
            new ColorReference
            {
                Name = "DarkGreen", Argb = new Vector3(
                    x: 0,
                    y: 100,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Navy", Argb = new Vector3(
                    x: 0,
                    y: 0,
                    z: 128),
            },
            new ColorReference
            {
                Name = "DarkKhaki", Argb = new Vector3(
                    x: 189,
                    y: 183,
                    z: 107),
            },
            new ColorReference
            {
                Name = "OldLace", Argb = new Vector3(
                    x: 253,
                    y: 245,
                    z: 230),
            },
            new ColorReference
            {
                Name = "DarkMagena", Argb = new Vector3(
                    x: 139,
                    y: 0,
                    z: 139),
            },
            new ColorReference
            {
                Name = "Olive", Argb = new Vector3(
                    x: 128,
                    y: 128,
                    z: 0),
            },
            new ColorReference
            {
                Name = "DarkOliveGreen", Argb = new Vector3(
                    x: 85,
                    y: 107,
                    z: 47),
            },
            new ColorReference
            {
                Name = "OliveDrab", Argb = new Vector3(
                    x: 107,
                    y: 142,
                    z: 45),
            },
            new ColorReference
            {
                Name = "DarkOrange", Argb = new Vector3(
                    x: 255,
                    y: 140,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Orange", Argb = new Vector3(
                    x: 255,
                    y: 165,
                    z: 0),
            },
            new ColorReference
            {
                Name = "DarkOrchid", Argb = new Vector3(
                    x: 153,
                    y: 50,
                    z: 204),
            },
            new ColorReference
            {
                Name = "OrangeRed", Argb = new Vector3(
                    x: 255,
                    y: 69,
                    z: 0),
            },
            new ColorReference
            {
                Name = "DarkRed", Argb = new Vector3(
                    x: 139,
                    y: 0,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Orchid", Argb = new Vector3(
                    x: 218,
                    y: 112,
                    z: 214),
            },
            new ColorReference
            {
                Name = "DarkSalmon", Argb = new Vector3(
                    x: 233,
                    y: 150,
                    z: 122),
            },
            new ColorReference
            {
                Name = "PaleGoldenrod", Argb = new Vector3(
                    x: 238,
                    y: 232,
                    z: 170),
            },
            new ColorReference
            {
                Name = "DarkSeaGreen", Argb = new Vector3(
                    x: 143,
                    y: 188,
                    z: 143),
            },
            new ColorReference
            {
                Name = "PaleGreen", Argb = new Vector3(
                    x: 152,
                    y: 251,
                    z: 152),
            },
            new ColorReference
            {
                Name = "DarkSlateBlue", Argb = new Vector3(
                    x: 72,
                    y: 61,
                    z: 139),
            },
            new ColorReference
            {
                Name = "PaleTurquoise", Argb = new Vector3(
                    x: 175,
                    y: 238,
                    z: 238),
            },
            new ColorReference
            {
                Name = "DarkSlateGray", Argb = new Vector3(
                    x: 40,
                    y: 79,
                    z: 79),
            },
            new ColorReference
            {
                Name = "PaleVioletRed", Argb = new Vector3(
                    x: 219,
                    y: 112,
                    z: 147),
            },
            new ColorReference
            {
                Name = "DarkTurquoise", Argb = new Vector3(
                    x: 0,
                    y: 206,
                    z: 209),
            },
            new ColorReference
            {
                Name = "PapayaWhip", Argb = new Vector3(
                    x: 255,
                    y: 239,
                    z: 213),
            },
            new ColorReference
            {
                Name = "DarkViolet", Argb = new Vector3(
                    x: 148,
                    y: 0,
                    z: 211),
            },
            new ColorReference
            {
                Name = "PeachPuff", Argb = new Vector3(
                    x: 255,
                    y: 218,
                    z: 155),
            },
            new ColorReference
            {
                Name = "DeepPink", Argb = new Vector3(
                    x: 255,
                    y: 20,
                    z: 147),
            },
            new ColorReference
            {
                Name = "Peru", Argb = new Vector3(
                    x: 205,
                    y: 133,
                    z: 63),
            },
            new ColorReference
            {
                Name = "DeepSkyBlue", Argb = new Vector3(
                    x: 0,
                    y: 191,
                    z: 255),
            },
            new ColorReference
            {
                Name = "Pink", Argb = new Vector3(
                    x: 255,
                    y: 192,
                    z: 203),
            },
            new ColorReference
            {
                Name = "DimGray", Argb = new Vector3(
                    x: 105,
                    y: 105,
                    z: 105),
            },
            new ColorReference
            {
                Name = "Plum", Argb = new Vector3(
                    x: 221,
                    y: 160,
                    z: 221),
            },
            new ColorReference
            {
                Name = "DodgerBlue", Argb = new Vector3(
                    x: 30,
                    y: 144,
                    z: 255),
            },
            new ColorReference
            {
                Name = "PowderBlue", Argb = new Vector3(
                    x: 176,
                    y: 224,
                    z: 230),
            },
            new ColorReference
            {
                Name = "Firebrick", Argb = new Vector3(
                    x: 178,
                    y: 34,
                    z: 34),
            },
            new ColorReference
            {
                Name = "Purple", Argb = new Vector3(
                    x: 128,
                    y: 0,
                    z: 128),
            },
            new ColorReference
            {
                Name = "FloralWhite", Argb = new Vector3(
                    x: 255,
                    y: 250,
                    z: 240),
            },
            new ColorReference
            {
                Name = "Red", Argb = new Vector3(
                    x: 255,
                    y: 0,
                    z: 0),
            },
            new ColorReference
            {
                Name = "ForestGreen", Argb = new Vector3(
                    x: 34,
                    y: 139,
                    z: 34),
            },
            new ColorReference
            {
                Name = "RosyBrown", Argb = new Vector3(
                    x: 188,
                    y: 143,
                    z: 143),
            },
            new ColorReference
            {
                Name = "Fuschia", Argb = new Vector3(
                    x: 255,
                    y: 0,
                    z: 255),
            },
            new ColorReference
            {
                Name = "RoyalBlue", Argb = new Vector3(
                    x: 65,
                    y: 105,
                    z: 225),
            },
            new ColorReference
            {
                Name = "Gainsboro", Argb = new Vector3(
                    x: 220,
                    y: 220,
                    z: 220),
            },
            new ColorReference
            {
                Name = "SaddleBrown", Argb = new Vector3(
                    x: 139,
                    y: 69,
                    z: 19),
            },
            new ColorReference
            {
                Name = "GhostWhite", Argb = new Vector3(
                    x: 248,
                    y: 248,
                    z: 255),
            },
            new ColorReference
            {
                Name = "Salmon", Argb = new Vector3(
                    x: 250,
                    y: 128,
                    z: 114),
            },
            new ColorReference
            {
                Name = "Gold", Argb = new Vector3(
                    x: 255,
                    y: 215,
                    z: 0),
            },
            new ColorReference
            {
                Name = "SandyBrown", Argb = new Vector3(
                    x: 244,
                    y: 164,
                    z: 96),
            },
            new ColorReference
            {
                Name = "Goldenrod", Argb = new Vector3(
                    x: 218,
                    y: 165,
                    z: 32),
            },
            new ColorReference
            {
                Name = "SeaGreen", Argb = new Vector3(
                    x: 46,
                    y: 139,
                    z: 87),
            },
            new ColorReference
            {
                Name = "Gray", Argb = new Vector3(
                    x: 128,
                    y: 128,
                    z: 128),
            },
            new ColorReference
            {
                Name = "Seashell", Argb = new Vector3(
                    x: 255,
                    y: 245,
                    z: 238),
            },
            new ColorReference
            {
                Name = "Green", Argb = new Vector3(
                    x: 0,
                    y: 128,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Sienna", Argb = new Vector3(
                    x: 160,
                    y: 82,
                    z: 45),
            },
            new ColorReference
            {
                Name = "GreenYellow", Argb = new Vector3(
                    x: 173,
                    y: 255,
                    z: 47),
            },
            new ColorReference
            {
                Name = "Silver", Argb = new Vector3(
                    x: 192,
                    y: 192,
                    z: 192),
            },
            new ColorReference
            {
                Name = "Honeydew", Argb = new Vector3(
                    x: 240,
                    y: 255,
                    z: 240),
            },
            new ColorReference
            {
                Name = "SkyBlue", Argb = new Vector3(
                    x: 135,
                    y: 206,
                    z: 235),
            },
            new ColorReference
            {
                Name = "HotPink", Argb = new Vector3(
                    x: 255,
                    y: 105,
                    z: 180),
            },
            new ColorReference
            {
                Name = "SlateBlue", Argb = new Vector3(
                    x: 106,
                    y: 90,
                    z: 205),
            },
            new ColorReference
            {
                Name = "IndianRed", Argb = new Vector3(
                    x: 205,
                    y: 92,
                    z: 92),
            },
            new ColorReference
            {
                Name = "SlateGray", Argb = new Vector3(
                    x: 112,
                    y: 128,
                    z: 144),
            },
            new ColorReference
            {
                Name = "Indigo", Argb = new Vector3(
                    x: 75,
                    y: 0,
                    z: 130),
            },
            new ColorReference
            {
                Name = "Snow", Argb = new Vector3(
                    x: 255,
                    y: 250,
                    z: 250),
            },
            new ColorReference
            {
                Name = "Ivory", Argb = new Vector3(
                    x: 255,
                    y: 240,
                    z: 240),
            },
            new ColorReference
            {
                Name = "SpringGreen", Argb = new Vector3(
                    x: 0,
                    y: 255,
                    z: 127),
            },
            new ColorReference
            {
                Name = "Khaki", Argb = new Vector3(
                    x: 240,
                    y: 230,
                    z: 140),
            },
            new ColorReference
            {
                Name = "SteelBlue", Argb = new Vector3(
                    x: 70,
                    y: 130,
                    z: 180),
            },
            new ColorReference
            {
                Name = "Lavender", Argb = new Vector3(
                    x: 230,
                    y: 230,
                    z: 250),
            },
            new ColorReference
            {
                Name = "Tan", Argb = new Vector3(
                    x: 210,
                    y: 180,
                    z: 140),
            },
            new ColorReference
            {
                Name = "LavenderBlush", Argb = new Vector3(
                    x: 255,
                    y: 240,
                    z: 245),
            },
            new ColorReference
            {
                Name = "Teal", Argb = new Vector3(
                    x: 0,
                    y: 128,
                    z: 128),
            },
            new ColorReference
            {
                Name = "LawnGreen", Argb = new Vector3(
                    x: 124,
                    y: 252,
                    z: 0),
            },
            new ColorReference
            {
                Name = "Thistle", Argb = new Vector3(
                    x: 216,
                    y: 191,
                    z: 216),
            },
            new ColorReference
            {
                Name = "LemonChiffon", Argb = new Vector3(
                    x: 255,
                    y: 250,
                    z: 205),
            },
            new ColorReference
            {
                Name = "Tomato", Argb = new Vector3(
                    x: 253,
                    y: 99,
                    z: 71),
            },
            new ColorReference
            {
                Name = "LightBlue", Argb = new Vector3(
                    x: 173,
                    y: 216,
                    z: 230),
            },
            new ColorReference
            {
                Name = "Turquoise", Argb = new Vector3(
                    x: 64,
                    y: 224,
                    z: 208),
            },
            new ColorReference
            {
                Name = "LightCoral", Argb = new Vector3(
                    x: 240,
                    y: 128,
                    z: 128),
            },
            new ColorReference
            {
                Name = "Violet", Argb = new Vector3(
                    x: 238,
                    y: 130,
                    z: 238),
            },
            new ColorReference
            {
                Name = "LightCyan", Argb = new Vector3(
                    x: 224,
                    y: 255,
                    z: 255),
            },
            new ColorReference
            {
                Name = "Wheat", Argb = new Vector3(
                    x: 245,
                    y: 222,
                    z: 179),
            },
            new ColorReference
            {
                Name = "LightGoldenrodYellow", Argb = new Vector3(
                    x: 250,
                    y: 250,
                    z: 210),
            },
            new ColorReference
            {
                Name = "White", Argb = new Vector3(
                    x: 255,
                    y: 255,
                    z: 255),
            },
            new ColorReference
            {
                Name = "LightGreen", Argb = new Vector3(
                    x: 144,
                    y: 238,
                    z: 144),
            },
            new ColorReference
            {
                Name = "WhiteSmoke", Argb = new Vector3(
                    x: 245,
                    y: 245,
                    z: 245),
            },
            new ColorReference
            {
                Name = "LightGray", Argb = new Vector3(
                    x: 211,
                    y: 211,
                    z: 211),
            },
            new ColorReference
            {
                Name = "Yellow", Argb = new Vector3(
                    x: 255,
                    y: 255,
                    z: 0),
            },
            new ColorReference
            {
                Name = "LightPink", Argb = new Vector3(
                    x: 255,
                    y: 182,
                    z: 193),
            },
            new ColorReference
            {
                Name = "YellowGreen", Argb = new Vector3(
                    x: 154,
                    y: 205,
                    z: 50),
            },
        };
    }
}

public record ColorReference
{
    public string Name { get; init; } = string.Empty;
    public Vector3 Argb { get; init; }
}