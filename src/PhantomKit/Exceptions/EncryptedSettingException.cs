// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class EncryptedSettingException : PhantomKitException
{
    public const string DefaultMessage = "Invalid encrypted setting";

    public EncryptedSettingException() : base(message: DefaultMessage)
    {
    }

    public EncryptedSettingException(string? paramName = null, string? paramValue = null) : base(message: MessageString(
        paramName: paramName,
        paramValue: paramValue))
    {
    }

    public EncryptedSettingException(
        string? paramName = null, string? paramValue = null, Exception? innerException = null) : base(
        message: MessageString(
            paramName: paramName,
            paramValue: paramValue),
        innerException: innerException)
    {
    }

    public static string MessageString(string? paramName = null, string? paramValue = null)
    {
        if (paramName is null || paramValue is null)
            return DefaultMessage;

        return $"{DefaultMessage}: {paramName}={paramValue}";
    }
}