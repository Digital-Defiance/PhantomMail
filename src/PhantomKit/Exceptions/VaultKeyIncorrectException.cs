// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class VaultKeyIncorrectException : PhantomKitException
{
    public const string DefaultMessage = "Vault key is incorrect";

    public VaultKeyIncorrectException() : base(message: DefaultMessage)
    {
    }

    public VaultKeyIncorrectException(string? paramName = null, string? paramValue = null,
        Exception? innerException = null) : base(message: MessageString(paramName: paramName,
            paramValue: paramValue),
        innerException: innerException)
    {
    }

    public VaultKeyIncorrectException(string? paramName = null, string? paramValue = null) : base(
        message: MessageString(
            paramName: paramName,
            paramValue: paramValue))
    {
    }

    public static string MessageString(string? paramName = null, string? paramValue = null)
    {
        if (paramName is null || paramValue is null)
            return DefaultMessage;

        return $"{DefaultMessage}: crc32 decrypt failed on param {paramName}={paramValue}";
    }
}