namespace PhantomKit.Exceptions;

public class VaultNotLoadedException : PhantomKitException
{
    public VaultNotLoadedException(string? message = null, Exception? innerException = null) : base(message: message,
        innerException: innerException)
    {
    }
}