// ReSharper disable MemberCanBePrivate.Global

namespace PhantomKit.Exceptions;

public class VaultFileNotFoundException : PhantomKitException
{
    public VaultFileNotFoundException(string fileName) : base(
        message: DefaultMessage(fileName: fileName))
    {
    }

    public static string DefaultMessage(string fileName)
    {
        return $"Vault file '{fileName}' was not found";
    }
}