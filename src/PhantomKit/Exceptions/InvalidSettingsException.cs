// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class InvalidSettingsException : PhantomKitException
{
    public const string DefaultMessage = "Invalid settings JSON";

    public InvalidSettingsException() : base(message: DefaultMessage)
    {
    }

    public InvalidSettingsException(string? paramName = null, string? paramValue = null,
        Exception? innerException = null) : base(message: MessageString(paramName: paramName,
            paramValue: paramValue),
        innerException: innerException)
    {
    }

    public InvalidSettingsException(string? paramName = null, string? paramValue = null) : base(message: MessageString(
        paramName: paramName,
        paramValue: paramValue))
    {
    }

    public static string MessageString(string? paramName = null, string? paramValue = null)
    {
        if (paramName is null || paramValue is null)
            return DefaultMessage;

        return $"{DefaultMessage}: {paramName}={paramValue}";
    }
}