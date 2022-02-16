/* https://github.com/Myfreedom614/UWP-Samples/blob/master/NearestColorUWPApp1/NearestColorUWPApp1/Helper/ColorHelper.cs */

using System.Globalization;
using System.Numerics;
using System.Text.RegularExpressions;

namespace PhantomKit.Helpers;

public class ColorHelper
{
    public static Dictionary<string, (int number, byte r, byte g, byte b, ConsoleColor?
        consoleColor)> ColorNames =
        new()
        {
            {"black", (number: 0, r: 0, g: 0, b: 0, consoleColor: ConsoleColor.Black)},
            {"maroon", (number: 1, r: 128, g: 0, b: 0, consoleColor: ConsoleColor.DarkRed)},
            {"green", (number: 2, r: 0, g: 128, b: 0, consoleColor: ConsoleColor.DarkGreen)},
            {"olive", (number: 3, r: 128, g: 128, b: 0, consoleColor: ConsoleColor.DarkYellow)},
            {"navy", (number: 4, r: 0, g: 0, b: 128, consoleColor: ConsoleColor.DarkBlue)},
            {"purple", (number: 5, r: 128, g: 0, b: 128, consoleColor: ConsoleColor.DarkMagenta)},
            {"teal", (number: 6, r: 0, g: 128, b: 128, consoleColor: ConsoleColor.DarkCyan)},
            {"silver", (number: 7, r: 192, g: 192, b: 192, consoleColor: ConsoleColor.Gray)},
            {"gray", (number: 8, r: 128, g: 128, b: 128, consoleColor: ConsoleColor.DarkGray)},
            {"red", (number: 9, r: 255, g: 0, b: 0, consoleColor: ConsoleColor.Red)},
            {"lime", (number: 10, r: 0, g: 255, b: 0, consoleColor: ConsoleColor.Green)},
            {"yellow", (number: 11, r: 255, g: 255, b: 0, consoleColor: ConsoleColor.Yellow)},
            {"blue", (number: 12, r: 0, g: 0, b: 255, consoleColor: ConsoleColor.Blue)},
            {"fuchsia", (number: 13, r: 255, g: 0, b: 255, consoleColor: ConsoleColor.Magenta)},
            {"aqua", (number: 14, r: 0, g: 255, b: 255, consoleColor: ConsoleColor.Cyan)},
            {"white", (number: 15, r: 255, g: 255, b: 255, consoleColor: ConsoleColor.White)},
            {"grey0", (number: 16, r: 0, g: 0, b: 0, ConsoleColor.Black)},
            // ReSharper disable once StringLiteralTypo
            {"navyblue", (number: 17, r: 0, g: 0, b: 95, null)},
            {"darkblue", (number: 18, r: 0, g: 0, b: 135, null)},
            {"blue3", (number: 19, 0, 0, 135, null)},
            {"blue1", (number: 21, 0, 0, 255, null)},
            {"darkgreen", (number: 22, r: 0, g: 95, b: 0, consoleColor: null)},
            {"deepskyblue4", (number: 23, r: 0, g: 95, b: 95, consoleColor: null)},
            {"deepskyblue4_1", (number: 24, r: 0, g: 95, b: 135, consoleColor: null)},
            {"deepskyblue4_2", (number: 25, r: 0, g: 95, b: 175, consoleColor: null)},
            {"dodgerblue3", (number: 26, r: 0, g: 95, b: 215, consoleColor: null)},
            {"dodgerblue2", (number: 27, r: 0, g: 95, b: 255, consoleColor: null)},
            {"green4", (number: 28, r: 0, g: 135, b: 0, consoleColor: null)},
            {"springgreen4", (number: 29, r: 0, g: 135, b: 95, consoleColor: null)},
            {"turquoise4", (number: 30, r: 0, g: 135, b: 135, consoleColor: null)},
            {"deepskyblue3", (number: 31, r: 0, g: 135, b: 175, consoleColor: null)},
            {"deepskyblue3_1", (number: 32, r: 0, g: 135, b: 215, consoleColor: null)},
            {"dodgerblue1", (number: 33, r: 0, g: 135, b: 255, consoleColor: null)},
            {"green3", (number: 34, r: 0, g: 175, b: 0, consoleColor: null)},
            {"springgreen3", (number: 35, r: 0, g: 175, b: 95, consoleColor: null)},
            {"darkcyan", (number: 36, r: 0, g: 175, b: 135, consoleColor: null)},
            {"lightseagreen", (number: 37, r: 0, g: 175, b: 175, consoleColor: null)},
            {"deepskyblue2", (number: 38, r: 0, g: 175, b: 215, consoleColor: null)},
            {"deepskyblue1", (number: 39, r: 0, g: 175, b: 255, consoleColor: null)},
            {"green3_1", (number: 40, r: 0, g: 215, b: 0, consoleColor: null)},
            {"springgreen3_1", (number: 41, r: 0, g: 215, b: 95, consoleColor: null)},
            {"springgreen2", (number: 42, r: 0, g: 215, b: 135, consoleColor: null)},
            {"cyan3", (number: 43, r: 0, g: 215, b: 175, consoleColor: null)},
            {"darkturquoise", (number: 44, r: 0, g: 215, b: 215, consoleColor: null)},
            {"turquoise2", (number: 45, r: 0, g: 215, b: 255, consoleColor: null)},
            {"green1", (number: 46, r: 0, g: 255, b: 0, consoleColor: null)},
            {"springgreen2_1", (number: 47, r: 0, g: 255, b: 95, consoleColor: null)},
            {"springgreen1", (number: 48, r: 0, g: 255, b: 135, consoleColor: null)},
            {"mediumspringgreen", (number: 49, r: 0, g: 255, b: 175, consoleColor: null)},
            {"cyan2", (number: 50, r: 0, g: 255, b: 215, consoleColor: null)},
            {"cyan1", (number: 51, r: 0, g: 255, b: 255, consoleColor: null)},
            {"darkred", (number: 52, r: 95, g: 0, b: 0, consoleColor: null)},
            {"deeppink4", (number: 53, r: 95, g: 0, b: 95, consoleColor: null)},
            {"purple4", (number: 54, r: 95, g: 0, b: 135, consoleColor: null)},
            {"purple4_1", (number: 55, r: 95, g: 0, b: 175, consoleColor: null)},
            {"purple3", (number: 56, r: 95, g: 0, b: 215, consoleColor: null)},
            {"blueviolet", (number: 57, r: 95, g: 0, b: 255, consoleColor: null)},
            {"orange4", (number: 58, r: 95, g: 95, b: 0, consoleColor: null)},
            {"grey37", (number: 59, r: 95, g: 95, b: 95, consoleColor: null)},
            {"mediumpurple4", (number: 60, r: 95, g: 95, b: 135, consoleColor: null)},
            {"slateblue3", (number: 61, r: 95, g: 95, b: 175, consoleColor: null)},
            {"slateblue3_1", (number: 62, r: 95, g: 95, b: 215, consoleColor: null)},
            {"royalblue1", (number: 63, r: 95, g: 95, b: 255, consoleColor: null)},
            {"chartreuse4", (number: 64, r: 95, g: 135, b: 0, consoleColor: null)},
            {"darkseagreen4", (number: 65, r: 95, g: 135, b: 95, consoleColor: null)},
            {"paleturquoise4", (number: 66, r: 95, g: 135, b: 135, consoleColor: null)},
            {"steelblue", (number: 67, r: 95, g: 135, b: 175, consoleColor: null)},
            {"steelblue3", (number: 68, r: 95, g: 135, b: 215, consoleColor: null)},
            {"cornflowerblue", (number: 69, r: 95, g: 135, b: 255, consoleColor: null)},
            {"chartreuse3", (number: 70, r: 95, g: 175, b: 0, consoleColor: null)},
            {"darkseagreen4_1", (number: 71, r: 95, g: 175, b: 95, consoleColor: null)},
            {"cadetblue", (number: 72, r: 95, g: 175, b: 135, consoleColor: null)},
            {"cadetblue_1", (number: 73, r: 95, g: 175, b: 175, consoleColor: null)},
            {"skyblue3", (number: 74, r: 95, g: 175, b: 215, consoleColor: null)},
            {"steelblue1", (number: 75, r: 95, g: 175, b: 255, consoleColor: null)},
            {"chartreuse3_1", (number: 76, r: 95, g: 215, b: 0, consoleColor: null)},
            {"palegreen3", (number: 77, r: 95, g: 215, b: 95, consoleColor: null)},
            {"seagreen3", (number: 78, r: 95, g: 215, b: 135, consoleColor: null)},
            {"aquamarine3", (number: 79, r: 95, g: 215, b: 175, consoleColor: null)},
            {"mediumturquoise", (number: 80, r: 95, g: 215, b: 215, consoleColor: null)},
            {"steelblue1_1", (number: 81, r: 95, g: 215, b: 255, consoleColor: null)},
            {"chartreuse2", (number: 82, r: 95, g: 255, b: 0, consoleColor: null)},
            {"seagreen2", (number: 83, r: 95, g: 255, b: 95, consoleColor: null)},
            {"seagreen1", (number: 84, r: 95, g: 255, b: 135, consoleColor: null)},
            {"seagreen1_1", (number: 85, r: 95, g: 255, b: 175, consoleColor: null)},
            {"aquamarine1", (number: 86, r: 95, g: 255, b: 215, consoleColor: null)},
            {"darkslategray2", (number: 87, r: 95, g: 255, b: 255, consoleColor: null)},
            {"darkred_1", (number: 88, r: 135, g: 0, b: 0, consoleColor: null)},
            {"deeppink4_1", (number: 89, r: 135, g: 0, b: 95, consoleColor: null)},
            {"darkmagenta", (number: 90, r: 135, g: 0, b: 135, consoleColor: null)},
            {"darkmagenta_1", (number: 91, r: 135, g: 0, b: 175, consoleColor: null)},
            {"darkviolet", (number: 92, r: 135, g: 0, b: 215, consoleColor: null)},
            {"purple_1", (number: 93, r: 135, g: 0, b: 255, consoleColor: null)},
            {"orange4_1", (number: 94, r: 135, g: 95, b: 0, consoleColor: null)},
            {"lightpink4", (number: 95, r: 135, g: 95, b: 95, consoleColor: null)},
            {"plum4", (number: 96, r: 135, g: 95, b: 135, consoleColor: null)},
            {"mediumpurple3", (number: 97, r: 135, g: 95, b: 175, consoleColor: null)},
            {"mediumpurple3_1", (number: 98, r: 135, g: 95, b: 215, consoleColor: null)},
            {"slateblue1", (number: 99, r: 135, g: 95, b: 255, consoleColor: null)},
            {"yellow4", (number: 100, r: 135, g: 135, b: 0, consoleColor: null)},
            {"wheat4", (number: 101, r: 135, g: 135, b: 95, consoleColor: null)},
            {"grey53", (number: 102, r: 135, g: 135, b: 135, consoleColor: null)},
            {"lightslategrey", (number: 103, r: 135, g: 135, b: 175, consoleColor: null)},
            {"mediumpurple", (number: 104, r: 135, g: 135, b: 215, consoleColor: null)},
            // ReSharper disable once StringLiteralTypo
            {"lightslateblue", (number: 105, r: 135, g: 135, b: 255, consoleColor: null)},
            {"yellow4_1", (number: 106, r: 135, g: 175, b: 0, consoleColor: null)},
            {"darkolivegreen3", (number: 107, r: 135, g: 175, b: 95, consoleColor: null)},
            {"darkseagreen", (number: 108, r: 135, g: 175, b: 135, consoleColor: null)},
            {"lightskyblue3", (number: 109, r: 135, g: 175, b: 175, consoleColor: null)},
            {"lightskyblue3_1", (number: 110, r: 135, g: 175, b: 215, consoleColor: null)},
            {"skyblue2", (number: 111, r: 135, g: 175, b: 255, consoleColor: null)},
            {"chartreuse2_1", (number: 112, r: 135, g: 215, b: 0, consoleColor: null)},
            {"darkolivegreen3_1", (number: 113, r: 135, g: 215, b: 95, consoleColor: null)},
            {"palegreen3_1", (number: 114, r: 135, g: 215, b: 135, consoleColor: null)},
            {"darkseagreen3", (number: 115, r: 135, g: 215, b: 175, consoleColor: null)},
            {"darkslategray3", (number: 116, r: 135, g: 215, b: 215, consoleColor: null)},
            {"skyblue1", (number: 117, r: 135, g: 215, b: 255, consoleColor: null)},
            {"chartreuse1", (number: 118, r: 135, g: 255, b: 0, consoleColor: null)},
            {"lightgreen", (number: 119, r: 135, g: 255, b: 95, consoleColor: null)},
            {"lightgreen_1", (number: 120, r: 135, g: 255, b: 135, consoleColor: null)},
            {"palegreen1", (number: 121, r: 135, g: 255, b: 175, consoleColor: null)},
            {"aquamarine1_1", (number: 122, r: 135, g: 255, b: 215, consoleColor: null)},
            {"darkslategray1", (number: 123, r: 135, g: 255, b: 255, consoleColor: null)},
            {"red3", (number: 124, r: 175, g: 0, b: 0, consoleColor: null)},
            {"deeppink4_2", (number: 125, r: 175, g: 0, b: 95, consoleColor: null)},
            {"mediumvioletred", (number: 126, r: 175, g: 0, b: 135, consoleColor: null)},
            {"magenta3", (number: 127, r: 175, g: 0, b: 175, consoleColor: null)},
            {"darkviolet_1", (number: 128, r: 175, g: 0, b: 215, consoleColor: null)},
            {"purple_2", (number: 129, r: 175, g: 0, b: 255, consoleColor: null)},
            {"darkorange3", (number: 130, r: 175, g: 95, b: 0, consoleColor: null)},
            {"indianred", (number: 131, r: 175, g: 95, b: 95, consoleColor: null)},
            {"hotpink3", (number: 132, r: 175, g: 95, b: 135, consoleColor: null)},
            {"mediumorchid3", (number: 133, r: 175, g: 95, b: 175, consoleColor: null)},
            {"mediumorchid", (number: 134, r: 175, g: 95, b: 215, consoleColor: null)},
            {"mediumpurple2", (number: 135, r: 175, g: 95, b: 255, consoleColor: null)},
            {"darkgoldenrod", (number: 136, r: 175, g: 135, b: 0, consoleColor: null)},
            {"lightsalmon3", (number: 137, r: 175, g: 135, b: 95, consoleColor: null)},
            {"rosybrown", (number: 138, r: 175, g: 135, b: 135, consoleColor: null)},
            {"grey63", (number: 139, r: 175, g: 135, b: 175, consoleColor: null)},
            {"mediumpurple2_1", (number: 140, r: 175, g: 135, b: 215, consoleColor: null)},
            {"mediumpurple1", (number: 141, r: 175, g: 135, b: 255, consoleColor: null)},
            {"gold3", (number: 142, r: 175, g: 175, b: 0, consoleColor: null)},
            {"darkkhaki", (number: 143, r: 175, g: 175, b: 95, consoleColor: null)},
            {"navajowhite3", (number: 144, r: 175, g: 175, b: 135, consoleColor: null)},
            {"grey69", (number: 145, r: 175, g: 175, b: 175, consoleColor: null)},
            {"lightsteelblue3", (number: 146, r: 175, g: 175, b: 215, consoleColor: null)},
            {"lightsteelblue", (number: 147, r: 175, g: 175, b: 255, consoleColor: null)},
            {"yellow3", (number: 148, r: 175, g: 215, b: 0, consoleColor: null)},
            {"darkolivegreen3_2", (number: 149, r: 175, g: 215, b: 95, consoleColor: null)},
            {"darkseagreen3_1", (number: 150, r: 175, g: 215, b: 135, consoleColor: null)},
            {"darkseagreen2", (number: 151, r: 175, g: 215, b: 175, consoleColor: null)},
            {"lightcyan3", (number: 152, r: 175, g: 215, b: 215, consoleColor: null)},
            {"lightskyblue1", (number: 153, r: 175, g: 215, b: 255, consoleColor: null)},
            {"greenyellow", (number: 154, r: 175, g: 255, b: 0, consoleColor: null)},
            {"darkolivegreen2", (number: 155, r: 175, g: 255, b: 95, consoleColor: null)},
            {"palegreen1_1", (number: 156, r: 175, g: 255, b: 135, consoleColor: null)},
            {"darkseagreen2_1", (number: 157, r: 175, g: 255, b: 175, consoleColor: null)},
            {"darkseagreen1", (number: 158, r: 175, g: 255, b: 215, consoleColor: null)},
            {"paleturquoise1", (number: 159, r: 175, g: 255, b: 255, consoleColor: null)},
            {"red3_1", (number: 160, r: 215, g: 0, b: 0, consoleColor: null)},
            {"deeppink3", (number: 161, r: 215, g: 0, b: 95, consoleColor: null)},
            {"deeppink3_1", (number: 162, r: 215, g: 0, b: 135, consoleColor: null)},
            {"magenta3_1", (number: 163, r: 215, g: 0, b: 175, consoleColor: null)},
            {"magenta3_2", (number: 164, r: 215, g: 0, b: 215, consoleColor: null)},
            {"magenta2", (number: 165, r: 215, g: 0, b: 255, consoleColor: null)},
            {"darkorange3_1", (number: 166, r: 215, g: 95, b: 0, consoleColor: null)},
            {"indianred_1", (number: 167, r: 215, g: 95, b: 95, consoleColor: null)},
            {"hotpink3_1", (number: 168, r: 215, g: 95, b: 135, consoleColor: null)},
            {"hotpink2", (number: 169, r: 215, g: 95, b: 175, consoleColor: null)},
            {"orchid", (number: 170, r: 215, g: 95, b: 215, consoleColor: null)},
            {"mediumorchid1", (number: 171, r: 215, g: 95, b: 255, consoleColor: null)},
            {"orange3", (number: 172, r: 215, g: 135, b: 0, consoleColor: null)},
            {"lightsalmon3_1", (number: 173, r: 215, g: 135, b: 95, consoleColor: null)},
            {"lightpink3", (number: 174, r: 215, g: 135, b: 135, consoleColor: null)},
            {"pink3", (number: 175, r: 215, g: 135, b: 175, consoleColor: null)},
            {"plum3", (number: 176, r: 215, g: 135, b: 215, consoleColor: null)},
            {"violet", (number: 177, r: 215, g: 135, b: 255, consoleColor: null)},
            {"gold3_1", (number: 178, r: 215, g: 175, b: 0, consoleColor: null)},
            // ReSharper disable once StringLiteralTypo
            {"lightgoldenrod3", (number: 179, r: 215, g: 175, b: 95, consoleColor: null)},
            {"tan", (number: 180, r: 215, g: 175, b: 135, consoleColor: null)},
            {"mistyrose3", (number: 181, r: 215, g: 175, b: 175, consoleColor: null)},
            {"thistle3", (number: 182, r: 215, g: 175, b: 215, consoleColor: null)},
            {"plum2", (number: 183, r: 215, g: 175, b: 255, consoleColor: null)},
            {"yellow3_1", (number: 184, r: 215, g: 215, b: 0, consoleColor: null)},
            {"khaki3", (number: 185, r: 215, g: 215, b: 95, consoleColor: null)},
            // ReSharper disable once StringLiteralTypo
            {"lightgoldenrod2", (number: 186, r: 215, g: 215, b: 135, consoleColor: null)},
            {"lightyellow3", (number: 187, r: 215, g: 215, b: 175, consoleColor: null)},
            {"grey84", (number: 188, r: 215, g: 215, b: 215, consoleColor: null)},
            {"lightsteelblue1", (number: 189, r: 215, g: 215, b: 255, consoleColor: null)},
            {"yellow2", (number: 190, r: 215, g: 255, b: 0, consoleColor: null)},
            {"darkolivegreen1", (number: 191, r: 215, g: 255, b: 95, consoleColor: null)},
            {"darkolivegreen1_1", (number: 192, r: 215, g: 255, b: 135, consoleColor: null)},
            {"darkseagreen1_1", (number: 193, r: 215, g: 255, b: 175, consoleColor: null)},
            {"honeydew2", (number: 194, r: 215, g: 255, b: 215, consoleColor: null)},
            {"lightcyan1", (number: 195, r: 215, g: 255, b: 255, consoleColor: null)},
            {"red1", (number: 196, r: 255, g: 255, b: 0, consoleColor: null)},
            {"deeppink2", (number: 197, r: 255, g: 0, b: 95, consoleColor: null)},
            {"deeppink1", (number: 198, r: 255, g: 0, b: 135, consoleColor: null)},
            {"deeppink1_1", (number: 199, r: 255, g: 0, b: 175, consoleColor: null)},
            {"magenta2_1", (number: 200, r: 255, g: 0, b: 215, consoleColor: null)},
            {"magenta1", (number: 201, r: 255, g: 0, b: 255, consoleColor: null)},
            {"orangered1", (number: 202, r: 255, g: 95, b: 0, consoleColor: null)},
            {"indianred1", (number: 203, r: 255, g: 95, b: 95, consoleColor: null)},
            {"indianred1_1", (number: 204, r: 255, g: 95, b: 135, consoleColor: null)},
            {"hotpink", (number: 205, r: 255, g: 95, b: 175, consoleColor: null)},
            {"hotpink_1", (number: 206, r: 255, g: 95, b: 215, consoleColor: null)},
            {"mediumorchid1_1", (number: 207, r: 255, g: 95, b: 255, consoleColor: null)},
            {"darkorange", (number: 208, r: 255, g: 135, b: 0, consoleColor: null)},
            {"salmon1", (number: 209, r: 255, g: 135, b: 95, consoleColor: null)},
            {"lightcoral", (number: 210, r: 255, g: 135, b: 135, consoleColor: null)},
            {"palevioletred1", (number: 211, r: 255, g: 135, b: 175, consoleColor: null)},
            {"orchid2", (number: 212, r: 255, g: 135, b: 215, consoleColor: null)},
            {"orchid1", (number: 213, r: 255, g: 135, b: 255, consoleColor: null)},
            {"orange1", (number: 214, r: 255, g: 175, b: 0, consoleColor: null)},
            {"sandybrown", (number: 215, r: 255, g: 175, b: 95, consoleColor: null)},
            {"lightsalmon1", (number: 216, r: 255, g: 175, b: 135, consoleColor: null)},
            {"lightpink1", (number: 217, r: 255, g: 175, b: 175, consoleColor: null)},
            {"pink1", (number: 218, r: 255, g: 175, b: 215, consoleColor: null)},
            {"plum1", (number: 219, r: 255, g: 175, b: 255, consoleColor: null)},
            {"gold1", (number: 220, r: 255, g: 215, b: 0, consoleColor: null)},
            // ReSharper disable once StringLiteralTypo
            {"lightgoldenrod2_1", (number: 221, r: 255, g: 215, b: 95, consoleColor: null)},
            // ReSharper disable once StringLiteralTypo
            {"lightgoldenrod2_2", (number: 222, r: 255, g: 215, b: 135, consoleColor: null)},
            {"navajowhite1", (number: 223, r: 255, g: 215, b: 175, consoleColor: null)},
            {"mistyrose1", (number: 224, r: 255, g: 215, b: 215, consoleColor: null)},
            {"thistle1", (number: 225, r: 255, g: 215, b: 255, consoleColor: null)},
            {"yellow1", (number: 226, r: 255, g: 255, b: 0, consoleColor: null)},
            // ReSharper disable once StringLiteralTypo
            {"lightgoldenrod1", (number: 227, r: 255, g: 255, b: 95, consoleColor: null)},
            {"khaki1", (number: 228, r: 255, g: 255, b: 135, consoleColor: null)},
            {"wheat1", (number: 229, r: 255, g: 255, b: 175, consoleColor: null)},
            {"cornsilk1", (number: 230, r: 255, g: 255, b: 215, consoleColor: null)},
            {"grey100", (number: 231, r: 255, g: 255, b: 255, consoleColor: null)},
            {"grey3", (number: 232, r: 8, g: 8, b: 8, consoleColor: null)},
            {"grey7", (number: 233, r: 18, g: 18, b: 18, consoleColor: null)},
            {"grey11", (number: 234, r: 28, g: 28, b: 28, consoleColor: null)},
            {"grey15", (number: 235, r: 38, g: 38, b: 38, consoleColor: null)},
            {"grey19", (number: 236, r: 48, g: 48, b: 48, consoleColor: null)},
            {"grey23", (number: 237, r: 58, g: 58, b: 58, consoleColor: null)},
            {"grey27", (number: 238, r: 68, g: 68, b: 68, consoleColor: null)},
            {"grey30", (number: 239, r: 78, g: 78, b: 78, consoleColor: null)},
            {"grey35", (number: 240, r: 88, g: 88, b: 88, consoleColor: null)},
            {"grey39", (number: 241, r: 98, g: 98, b: 98, consoleColor: null)},
            {"grey42", (number: 242, r: 108, g: 108, b: 108, consoleColor: null)},
            {"grey46", (number: 243, r: 118, g: 118, b: 118, consoleColor: null)},
            {"grey50", (number: 244, r: 128, g: 128, b: 128, consoleColor: null)},
            {"grey54", (number: 245, r: 138, g: 138, b: 138, consoleColor: null)},
            {"grey58", (number: 246, r: 148, g: 148, b: 148, consoleColor: null)},
            {"grey62", (number: 247, r: 158, g: 158, b: 158, consoleColor: null)},
            {"grey66", (number: 248, r: 168, g: 168, b: 168, consoleColor: null)},
            {"grey70", (number: 249, r: 178, g: 178, b: 178, consoleColor: null)},
            {"grey74", (number: 250, r: 188, g: 188, b: 188, consoleColor: null)},
            {"grey78", (number: 251, r: 198, g: 198, b: 198, consoleColor: null)},
            {"grey82", (number: 252, r: 208, g: 208, b: 208, consoleColor: null)},
            {"grey85", (number: 253, r: 218, g: 218, b: 218, consoleColor: null)},
            {"grey89", (number: 254, r: 228, g: 228, b: 228, consoleColor: null)},
            {"grey93", (number: 255, r: 238, g: 238, b: 238, consoleColor: null)},
        };

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

public class ColorReference
{
    public string Name { get; set; }
    public Vector3 Argb { get; set; }
}