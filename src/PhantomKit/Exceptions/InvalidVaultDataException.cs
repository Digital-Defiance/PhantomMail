// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class InvalidVaultDataException : PhantomKitException
{
    public const string DefaultMessage = "Invalid vault JSON";

    public InvalidVaultDataException() : base(message: DefaultMessage)
    {
    }

    public InvalidVaultDataException(string? paramName = null, string? paramValue = null,
        Exception? innerException = null) : base(message: MessageString(paramName: paramName,
            paramValue: paramValue),
        innerException: innerException)
    {
    }

    public InvalidVaultDataException(string? paramName = null, string? paramValue = null) : base(message: MessageString(
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