using PhantomKit.Exceptions;
using Spectre.Console;

namespace PhantomKit.Helpers;

public class Spectre
{
    public const string EndSpectreTag = "[/]";
    public const string Ellipsis = "... ";

    public static string EllipsisText => SpectreTag(
        tag: Constants.DotColor,
        content: Ellipsis);

    public static string PrettyProgramName
        => Constants.UsePrettyConsole
            ? SpectreTag(tag: Constants.ProgramColor,
                content: string.Concat(
                    str0: Constants.ProgramName,
                    str1: ":ghost:"))
            : Constants.ProgramName;

    public static string DefaultPasswordWarning
        => string.Join(
            separator: ' ',
            SpectreTag(tag: Constants.WarningColor,
                content: "WARNING:"),
            SpectreTag(tag: Constants.ProgressStringColor,
                content: "New vault is using the default password:"),
            SpectreTag(tag: Constants.ErrorColor,
                content: Constants.DefaultPassword));


    public static Color GetColor(string colorName)
    {
        if (ColorHelper.ColorNames.ContainsKey(key: colorName))
            return new Color(red: ColorHelper.ColorNames[key: colorName].r,
                green: ColorHelper.ColorNames[key: colorName].g,
                blue: ColorHelper.ColorNames[key: colorName].b);
        throw new KeyNotFoundException();
    }

    public static ConsoleColor? GetConsoleColor(string colorName)
    {
        if (ColorHelper.ColorNames.ContainsKey(key: colorName))
            return ColorHelper.ColorNames[key: colorName].consoleColor;

        return null;
    }

    public static string GetColorName(Color color)
    {
        foreach (var colorName in ColorHelper.ColorNames)
            if (colorName.Value.r == color.R && colorName.Value.g == color.G && colorName.Value.b == color.B)
                return colorName.Key;
        throw new KeyNotFoundException();
    }

    public static string SpectreTagStart(string tag)
    {
        return $"[{tag}]";
    }

    public static string SpectreTag(string tag, string content)
    {
        return string.Join(separator: string.Empty,
            SpectreTagStart(tag: tag),
            content,
            EndSpectreTag);
    }

    public static void PrintStatus(string message, bool colorWrapMessage)
    {
        AnsiConsole.MarkupLine(
            value:
            string.Concat(
                str0: EllipsisText,
                str1: colorWrapMessage ? SpectreTagStart(tag: Constants.ProgressStringColor) : string.Empty,
                str2: message,
                str3: colorWrapMessage ? EndSpectreTag : string.Empty));
    }

    public static void PrintStatus(string status, string color, string? message = null)
    {
        AnsiConsole.MarkupLine(
            value: message is null
                ? string.Concat(
                    str0: EllipsisText,
                    str1: SpectreTag(tag: color,
                        content: status))
                : string.Concat(
                    str0: EllipsisText,
                    str1: SpectreTag(tag: color,
                        content: status),
                    str2: " : ",
                    str3: SpectreTag(tag: Constants.ProgressStringColor,
                        content: message)));
    }


    public static void PrintSuccess(string? message = null)
    {
        PrintStatus(status: "SUCCESS",
            color: Constants.SuccessColor,
            message: message);
    }

    public static void PrintError(string? message = null)
    {
        PrintStatus(status: "ERROR",
            color: Constants.ErrorColor,
            message: message);
    }

    public static void PrintError(PhantomKitException pkEx)
    {
        PrintError(message: pkEx.Message);

        if (pkEx.InnerException is { } innerException) PrintError(e: innerException);
    }

    public static void PrintError(Exception e)
    {
        while (true)
        {
            PrintError(message: e.Message);

            if (e.InnerException is { } innerException)
            {
                e = innerException;
                continue;
            }

            break;
        }
    }
}